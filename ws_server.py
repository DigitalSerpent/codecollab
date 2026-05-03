from fastapi import FastAPI, WebSocket, WebSocketDisconnect
import json
from typing import Dict, Set
import asyncio

app = FastAPI()

class ConnectionManager:
    def __init__(self):
        self.active_connections: Dict[int, Dict[int, WebSocket]] = {}
        self.user_info: Dict[int, dict] = {}
        self.room_participants: Dict[int, Set[int]] = {}

    async def connect(self, room_id: int, user_id: int, websocket: WebSocket):
        await websocket.accept()
        
        if room_id not in self.active_connections:
            self.active_connections[room_id] = {}
            self.room_participants[room_id] = set()
            
        self.active_connections[room_id][user_id] = websocket
        self.room_participants[room_id].add(user_id)
        
        await self.send_participant_list(room_id, user_id)

    def disconnect(self, room_id: int, user_id: int):
        if room_id in self.active_connections:
            self.active_connections[room_id].pop(user_id, None)
            self.room_participants[room_id].discard(user_id)
            
            if not self.active_connections[room_id]:
                del self.active_connections[room_id]
                del self.room_participants[room_id]
        
        self.user_info.pop(user_id, None)

    async def send_participant_list(self, room_id: int, current_user_id: int = None):
        if room_id not in self.active_connections:
            return
            
        participants = []
        for uid in self.room_participants.get(room_id, set()):
            info = self.user_info.get(uid, {})
            participants.append({
                "user_id": uid,
                "name": info.get("name", ""),
                "avatar": info.get("avatar", "👤"),
                "cursor": info.get("cursor", "⬤"),
                "online": True
            })
        
        message = json.dumps({"type": "participant_list", "users": participants})
        
        for ws in self.active_connections[room_id].values():
            try:
                await ws.send_text(message)
            except:
                pass

    async def broadcast_cursor(self, room_id: int, sender_id: int, cursor_data: dict):
        if room_id not in self.active_connections:
            return
            
        message = json.dumps({
            "type": "cursor",
            "userId": cursor_data.get("userId"),
            "userName": cursor_data.get("userName", "Unknown"),
            "position": cursor_data.get("position", {"lineNumber": 1, "column": 1})
        })
        
        for user_id, ws in self.active_connections[room_id].items():
            if user_id != sender_id:
                try:
                    await ws.send_text(message)
                except:
                    pass

    async def update_user_info(self, room_id: int, user_id: int, user_data: dict):
        self.user_info[user_id] = {
            "name": user_data.get("name", ""),
            "avatar": user_data.get("avatar", "👤"),
            "cursor": user_data.get("cursor", "⬤")
        }
        await self.send_participant_list(room_id)

manager = ConnectionManager()

@app.websocket("/ws/{room_id}/{user_id}")
async def websocket_endpoint(websocket: WebSocket, room_id: int, user_id: int):
    await manager.connect(room_id, user_id, websocket)
    
    try:
        data = await websocket.receive_text()
        user_data = json.loads(data)
        await manager.update_user_info(room_id, user_id, user_data)
        
        while True:
            try:
                data = await websocket.receive_text()
                message = json.loads(data)
                
                if message.get("type") == "cursor":
                    await manager.broadcast_cursor(room_id, user_id, message)
                elif message.get("type") == "ping":
                    await websocket.send_text(json.dumps({"type": "pong"}))
                    
            except json.JSONDecodeError:
                continue
                
    except WebSocketDisconnect:
        manager.disconnect(room_id, user_id)
        await manager.send_participant_list(room_id)

@app.get("/health")
async def health_check():
    return {"status": "healthy"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)