from fastapi import FastAPI, WebSocket, WebSocketDisconnect
import json
from typing import Dict

app = FastAPI()

class ConnectionManager:
    def __init__(self):
        self.active_connections: Dict[int, Dict[int, WebSocket]] = {}
        self.user_info: Dict[int, dict] = {}

    async def connect(self, room_id: int, user_id: int, websocket: WebSocket, user_data: dict):
        await websocket.accept()
        if room_id not in self.active_connections:
            self.active_connections[room_id] = {}
        self.active_connections[room_id][user_id] = websocket
        self.user_info[user_id] = user_data
        await self.send_participant_list(room_id, user_id)

    def disconnect(self, room_id: int, user_id: int):
        if room_id in self.active_connections:
            self.active_connections[room_id].pop(user_id, None)
        self.user_info.pop(user_id, None)

    async def send_participant_list(self, room_id: int, current_user_id: int = None):
        if room_id not in self.active_connections:
            return
        participants = []
        for uid, ws in self.active_connections[room_id].items():
            info = self.user_info.get(uid, {})
            participants.append({
                "user_id": uid,
                "name": info.get("name", ""),
                "avatar": info.get("avatar", "👤"),
                "cursor": info.get("cursor", "⬤"),
                "online": True
            })
        # Добавляем текущего пользователя, если его нет (для страницы)
        if current_user_id and current_user_id not in [p["user_id"] for p in participants]:
            info = self.user_info.get(current_user_id, {})
            participants.append({
                "user_id": current_user_id,
                "name": info.get("name", ""),
                "avatar": info.get("avatar", "👤"),
                "cursor": info.get("cursor", "⬤"),
                "online": True
            })
        message = json.dumps({"type": "participant_list", "users": participants})
        for ws in self.active_connections[room_id].values():
            await ws.send_text(message)

manager = ConnectionManager()

@app.websocket("/ws/{room_id}/{user_id}")
async def websocket_endpoint(websocket: WebSocket, room_id: int, user_id: int):
    await websocket.accept()
    try:
        data = await websocket.receive_text()
        user_data = json.loads(data)
        await manager.connect(room_id, user_id, websocket, user_data)
        while True:
            await websocket.receive_text()
    except WebSocketDisconnect:
        manager.disconnect(room_id, user_id)
        await manager.send_participant_list(room_id)