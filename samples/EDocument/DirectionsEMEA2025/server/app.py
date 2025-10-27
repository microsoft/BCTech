from flask import Flask, request, jsonify, abort
from collections import defaultdict, deque
import uuid

app = Flask(__name__)

# Simple in-memory stores
auth_keys = {}            # user_id -> key
queues = defaultdict(deque)  # key -> deque

@app.route("/register", methods=["POST"])
def register():
    data = request.get_json()
    name = data.get("name")
    if not name:
        abort(400, "Missing name")
    key = str(uuid.uuid4())
    auth_keys[name] = key
    queues[key]  # Initialize queue
    return jsonify({"status": "ok", "key": key})

def get_key() -> str:
    key = str.lower(request.headers.get("X-Service-Key"))
    if not key or key not in queues:
        abort(401, "Unauthorized or invalid key")
    return key

@app.route("/enqueue", methods=["POST"])
def enqueue():
    key = get_key()
    doc = request.get_json()
    queues[key].append(doc)
    return jsonify({"status": "ok", "queued_count": len(queues[key])})

@app.route("/dequeue", methods=["GET"])
def dequeue():
    key = get_key()
    if not queues[key]:
        abort(404, "Queue empty")
    return jsonify({"document": queues[key].popleft()})

@app.route("/peek", methods=["GET"])
def peek():
    key = get_key()
    return jsonify({"queued_count": len(queues[key]), "items": list(queues[key])})

@app.route("/clear", methods=["DELETE"])
def clear():
    key = get_key()
    queues[key].clear()
    return jsonify({"status": "cleared"})

@app.route("/")
def root():
    return "Hello world"

# if __name__ == "__main__":
#     app.run(debug=True, host="0.0.0.0", port=8000)

if __name__ == '__main__':
   app.run(debug=True)