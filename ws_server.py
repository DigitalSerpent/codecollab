from fastapi import FastAPI, WebSocket, WebSocketDisconnect, Request
from fastapi.responses import HTMLResponse
import json
import asyncio
from typing import Dict, Set

app = FastAPI()

# Хранилище активных комнат
rooms: Dict[int, Dict[int, WebSocket]] = {}  # roomId -> {userId: websocket}
users_info: Dict[int, dict] = {}  # userId -> {name, avatar, cursor}

@app.post("/room_join")
async def room_join(request: Request):
    data = await request.json()
    room_id = data["roomId"]
    user_id = data["userId"]
    user_info = {
        "name": data["userName"],
        "avatar": data["avatar"],
        "cursor": data["cursor"],
        "online": True
    }
    users_info[user_id] = user_info
    
    # Пока просто логируем, WebSocket клиент сам появится позже
    print(f"User {user_id} joined room {room_id}")
    return {"status": "ok"}

@app.post("/room_leave")
async def room_leave(request: Request):
    data = await request.json()
    user_id = data["userId"]
    if user_id in users_info:
        del users_info[user_id]
    print(f"User {user_id} left")
    return {"status": "ok"}

@app.websocket("/ws/{room_id}/{user_id}")
async def websocket_endpoint(websocket: WebSocket, room_id: int, user_id: int):
    await websocket.accept()
    
    # Сохраняем соединение
    if room_id not in rooms:
        rooms[room_id] = {}
    rooms[room_id][user_id] = websocket
    
    # Отправляем новому пользователю список всех участников
    participants = []
    for uid, ws in rooms[room_id].items():
        if uid != user_id and uid in users_info:
            participants.append({
                "userId": uid,
                "name": users_info[uid]["name"],
                "avatar": users_info[uid]["avatar"],
                "cursor": users_info[uid]["cursor"],
                "online": True
            })
    await websocket.send_json({"type": "participant_list", "users": participants})
    
    # Оповещаем всех о новом участнике
    for uid, ws in rooms[room_id].items():
        if uid != user_id:
            await ws.send_json({
                "type": "user_joined",
                "user": {
                    "userId": user_id,
                    "name": users_info[user_id]["name"],
                    "avatar": users_info[user_id]["avatar"],
                    "cursor": users_info[user_id]["cursor"]
                }
            })
    
    try:
        while True:
            data = await websocket.receive_text()
            # Можно обработать курсоры, но пока игнорируем
    except WebSocketDisconnect:
        # Удаляем соединение
        if room_id in rooms and user_id in rooms[room_id]:
            del rooms[room_id][user_id]
        if not rooms[room_id]:
            del rooms[room_id]
        
        # Оповещаем остальных о выходе
        if room_id in rooms:
            for uid, ws in rooms[room_id].items():
                await ws.send_json({"type": "user_left", "userId": user_id})