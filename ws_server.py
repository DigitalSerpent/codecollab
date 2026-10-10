from fastapi import FastAPI, WebSocket, WebSocketDisconnect
import json
from typing import Dict
app = FastAPI()
class ConnectionManager:
    def __init__(self):
        self.rooms: Dict[str, Dict[str, WebSocket]] = {}
    async def connect(self, room_id: str, user_id: str, websocket: WebSocket):
        await websocket.accept()
        if room_id not in self.rooms: self.rooms[room_id] = {}
        self.rooms[room_id][user_id] = websocket
    def disconnect(self, room_id: str, user_id: str):
        if room_id in self.rooms:
            self.rooms[room_id].pop(user_id, None)
            if not self.rooms[room_id]: del self.rooms[room_id]
    async def broadcast_cursor(self, room_id: str, sender_id: str, message: str):
        if room_id not in self.rooms: return
        for uid, ws in self.rooms[room_id].items():
            if uid != sender_id:
                try: await ws.send_text(message)
                except Exception: pass
manager = ConnectionManager()
@app.websocket("/ws/{room_id}/{user_id}")
async def websocket_endpoint(websocket: WebSocket, room_id: str, user_id: str):
    rid, uid = str(room_id), str(user_id)
    await manager.connect(rid, uid, websocket)
    try:
        while True:
            data = await websocket.receive_text()
            await manager.broadcast_cursor(rid, uid, data)
    except WebSocketDisconnect: manager.disconnect(rid, uid)
@app.get("/health")
async def health(): return {"status": "ok"}
