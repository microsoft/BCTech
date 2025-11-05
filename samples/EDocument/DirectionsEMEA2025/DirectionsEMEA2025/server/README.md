# Workshop API Server

Simple FastAPI server for the E-Document Connector Workshop.

## Overview

This server provides a simple queue-based document exchange API for workshop participants to practice E-Document integrations.

**Features**:
- User registration with API keys
- Document queue per user (isolated)
- FIFO queue operations (enqueue, peek, dequeue)
- In-memory storage (no database required)
- Simple authentication via header

**Technology**: Python FastAPI

---

## Deployment

### Azure Web App (Recommended for Workshop)

The server is deployed to Azure App Service for the workshop.

**Requirements**:
- Azure subscription
- App Service (Linux)
- Python 3.9+ runtime

**Deployment Steps**:

1. **Create App Service**:
```bash
az webapp up --name workshop-edocument-api \
  --resource-group rg-workshop \
  --runtime "PYTHON:3.9" \
  --sku B1
```

2. **Deploy Code**:
```bash
cd server
zip -r app.zip .
az webapp deployment source config-zip \
  --resource-group rg-workshop \
  --name workshop-edocument-api \
  --src app.zip
```

3. **Configure Startup**:
In Azure Portal, set startup command:
```bash
python -m uvicorn server:app --host 0.0.0.0 --port 8000
```

4. **Verify**:
```bash
curl https://workshop-edocument-api.azurewebsites.net/docs
```

---

## Local Development

### Prerequisites
- Python 3.9 or higher
- pip

### Installation

1. **Install dependencies**:
```bash
pip install -r requirements.txt
```

2. **Run server**:
```bash
python -m uvicorn server:app --reload --port 8000
```

3. **Access**:
- API: http://localhost:8000
- Interactive docs: http://localhost:8000/docs
- Alternative docs: http://localhost:8000/redoc

---

## API Endpoints

### POST /register
Register a new user and get an API key.

**Request**:
```json
{
  "name": "user-name"
}
```

**Response**:
```json
{
  "status": "ok",
  "key": "uuid-api-key"
}
```

### POST /enqueue
Add a document to your queue.

**Headers**: `X-Service-Key: your-api-key`

**Request**: Any JSON document

**Response**:
```json
{
  "status": "ok",
  "queued_count": 3
}
```

### GET /peek
View all documents in your queue (without removing).

**Headers**: `X-Service-Key: your-api-key`

**Response**:
```json
{
  "queued_count": 2,
  "items": [...]
}
```

### GET /dequeue
Retrieve and remove the first document from your queue.

**Headers**: `X-Service-Key: your-api-key`

**Response**:
```json
{
  "document": {...}
}
```

### DELETE /clear
Clear all documents from your queue.

**Headers**: `X-Service-Key: your-api-key`

**Response**:
```json
{
  "status": "cleared"
}
```

---

## Architecture

```
┌─────────────────────────────────────┐
│         FastAPI Server              │
│                                     │
│  ┌──────────────────────────────┐  │
│  │  auth_keys: Dict[str, str]   │  │  user_id -> api_key
│  └──────────────────────────────┘  │
│                                     │
│  ┌──────────────────────────────┐  │
│  │  queues: Dict[str, deque]    │  │  api_key -> queue
│  └──────────────────────────────┘  │
│                                     │
└─────────────────────────────────────┘
```

### Data Structures

**auth_keys**: Maps user names to API keys
```python
{
  "john-doe": "uuid-1",
  "jane-smith": "uuid-2"
}
```

**queues**: Maps API keys to document queues
```python
{
  "uuid-1": deque([doc1, doc2, doc3]),
  "uuid-2": deque([doc4, doc5])
}
```

---

## Testing

### Using cURL

```bash
# Register
curl -X POST http://localhost:8000/register \
  -H "Content-Type: application/json" \
  -d '{"name": "test-user"}'

# Enqueue
curl -X POST http://localhost:8000/enqueue \
  -H "X-Service-Key: your-key" \
  -H "Content-Type: application/json" \
  -d '{"test": "document"}'

# Peek
curl -X GET http://localhost:8000/peek \
  -H "X-Service-Key: your-key"

# Dequeue
curl -X GET http://localhost:8000/dequeue \
  -H "X-Service-Key: your-key"

# Clear
curl -X DELETE http://localhost:8000/clear \
  -H "X-Service-Key: your-key"
```

### Using Python

```python
import requests

base_url = "http://localhost:8000"

# Register
response = requests.post(f"{base_url}/register", json={"name": "test-user"})
api_key = response.json()["key"]

headers = {"X-Service-Key": api_key}

# Enqueue
requests.post(f"{base_url}/enqueue", headers=headers, json={"test": "doc"})

# Peek
response = requests.get(f"{base_url}/peek", headers=headers)
print(response.json())

# Dequeue
response = requests.get(f"{base_url}/dequeue", headers=headers)
print(response.json())
```

---

## Security Considerations

⚠️ **WARNING**: This server is designed for workshop use only!

**Security Limitations**:
- ❌ No HTTPS enforcement
- ❌ No rate limiting
- ❌ No persistent storage
- ❌ Simple API key authentication
- ❌ No data encryption
- ❌ No audit logging
- ❌ All data lost on restart

**For Production Use**:
- ✅ Use proper authentication (OAuth, JWT)
- ✅ Add HTTPS/TLS encryption
- ✅ Implement rate limiting
- ✅ Use persistent database
- ✅ Add comprehensive logging
- ✅ Implement data retention policies
- ✅ Add monitoring and alerting

---

## Monitoring

### Check Server Health

```bash
curl http://localhost:8000/docs
```

If the interactive docs load, the server is running.

### View Logs

**Local**:
- Check terminal where uvicorn is running

**Azure**:
```bash
az webapp log tail --name workshop-edocument-api --resource-group rg-workshop
```

Or in Azure Portal:
- App Service → Monitoring → Log stream

---

## Troubleshooting

### Server won't start
- Check Python version: `python --version` (need 3.9+)
- Verify dependencies: `pip install -r requirements.txt`
- Check port availability: `netstat -an | grep 8000`

### Can't access from outside
- Verify firewall settings
- Check Azure networking rules
- Ensure correct URL (HTTP not HTTPS for local)

### Authentication errors
- Verify API key is correctly copied
- Check header name: `X-Service-Key` (case-sensitive)
- Re-register if key is lost

### Queue empty when it shouldn't be
- Remember: Server uses in-memory storage
- Data is lost on restart
- Each user has isolated queue

---

## Scaling Considerations

For larger workshops:

1. **Use Azure App Service Plan** with auto-scaling
2. **Monitor CPU/Memory** usage
3. **Consider Redis** for distributed queue if multiple instances needed
4. **Add rate limiting** to prevent abuse
5. **Implement pagination** for large queues

Current implementation handles ~100 concurrent users with basic App Service plan.

---

## Development Notes

### Adding New Endpoints

1. Add route to `server.py`:
```python
@app.get("/new-endpoint")
async def new_endpoint(request: Request):
    key = get_key(request)
    # Your logic here
    return {"status": "ok"}
```

2. Test locally
3. Deploy to Azure

### Modifying Data Structure

Currently uses:
- `defaultdict(deque)` for queues
- `dict` for auth keys

For persistence, consider:
- Redis for queue
- Database for auth

---

## API Documentation

The server provides automatic API documentation:

- **Swagger UI**: `/docs`
- **ReDoc**: `/redoc`
- **OpenAPI JSON**: `/openapi.json`

Use these for interactive testing and documentation.

---

## License

MIT License - See repository root for details.

---

## Support

For issues with the server:
1. Check this README
2. Review server logs
3. Test endpoints with curl
4. Contact workshop instructor

For workshop content:
- See main README.md
- Check WORKSHOP_GUIDE.md
- Ask instructor

---

**Happy API Testing!** 🚀
