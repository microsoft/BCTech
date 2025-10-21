from fastapi import FastAPI, Request, HTTPException
from collections import defaultdict, deque
import uuid

app = FastAPI()

# Simple in-memory stores
auth_keys = {}            # user_id -> key
queues = defaultdict(deque)  # key -> deque

@app.post("/register")
async def register(request: Request):
    data = await request.json()
    name = data.get("name")
    if not name:
        raise HTTPException(400, "Missing name")
    key = str(uuid.uuid4())
    auth_keys[name] = key
    queues[key]  # Initialize queue
    return {"status": "ok", "key": key}

def get_key(request: Request) -> str:
    key = request.headers.get("X-Service-Key")
    if not key or key not in queues:
        raise HTTPException(401, "Unauthorized or invalid key")
    return key

@app.post("/enqueue")
async def enqueue(request: Request):
    key = get_key(request)
    doc = await request.json()
    queues[key].append(doc)
    return {"status": "ok", "queued_count": len(queues[key])}

@app.get("/dequeue")
async def dequeue(request: Request):
    key = get_key(request)
    if not queues[key]:
        raise HTTPException(404, "Queue empty")
    return {"document": queues[key].popleft()}

@app.get("/peek")
async def peek(request: Request):
    key = get_key(request)
    return {"queued_count": len(queues[key]), "items": list(queues[key])}

@app.delete("/clear")
async def clear(request: Request):
    key = get_key(request)
    queues[key].clear()
    return {"status": "cleared"}
