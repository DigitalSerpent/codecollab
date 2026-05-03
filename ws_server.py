from fastapi import FastAPI, WebSocket, WebSocketDisconnect
import json
from typing import Dict, Set

app = FastAPI()

class ConnectionManager:
    def __init__(self):
        self.rooms: Dict[int, Dict[int, WebSocket]] = {}
        self.user_info: Dict[int, dict] = {}

    async def connect(self, room_id: int, user_id: int, websocket: WebSocket):
        await websocket.accept()
        if room_id not in self.rooms:
            self.rooms[room_id] = {}
        self.rooms[room_id][user_id] = websocket

    def disconnect(self, room_id: int, user_id: int):
        if room_id in self.rooms:
            self.rooms[room_id].pop(user_id, None)
            if not self.rooms[room_id]:
                del self.rooms[room_id]

    async def broadcast_cursor(self, room_id: int, sender_id: int, data: dict):
        if room_id not in self.rooms:
            return
        message = json.dumps({
            "type": "cursor",
            "userId": data.get("userId"),
            "userName": data.get("userName", "Anonymous"),
            "position": data.get("position", {"lineNumber": 1, "column": 1})
        })
        for uid, ws in self.rooms[room_id].items():
            if uid != sender_id:
                try:
                    await ws.send_text(message)
                except:
                    pass

manager = ConnectionManager()

@app.websocket("/ws/{room_id}/{user_id}")
async def websocket_endpoint(websocket: WebSocket, room_id: int, user_id: int):
    await manager.connect(room_id, user_id, websocket)
    try:
        while True:
            data = await websocket.receive_text()
            msg = json.loads(data)
            if msg.get("type") == "cursor":
                await manager.broadcast_cursor(room_id, user_id, msg)
    except WebSocketDisconnect:
        manager.disconnect(room_id, user_id)

@app.get("/health")
async def health():
    return {"status": "ok"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)