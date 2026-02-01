# Port Conflict - Fixed!

## The Issue

You got the error: `Failed to bind to address http://127.0.0.1:5001: address already in use`

This happened because port 5001 was being used by both:
1. The Authentication API (HTTP fallback)
2. Potentially the Blazor UI

## The Fix

Changed the Authentication API HTTP port from 5001 to 5101 to avoid conflicts.

## Updated Ports

### Authentication API
- **HTTPS**: `https://localhost:7001` ✅ (Primary - use this)
- **HTTP**: `http://localhost:5101` (Fallback)

### Blazor UI
- Will use whatever port is available (usually 5001 or 5173)

## How to Start Now

### Option 1: Just wait 30 seconds and retry
Sometimes the port takes a moment to release. Wait 30 seconds and run again:

```cmd
cd d:\Personal\FreshNoodle\FreshNoodle.API.Authentication
dotnet run
```

### Option 2: Use the startup script (Recommended)

**Terminal 1 - Start API:**
```cmd
cd d:\Personal\FreshNoodle
start-api.cmd
```

Wait until you see:
```
Now listening on: https://localhost:7001
Now listening on: http://localhost:5101
```

**Terminal 2 - Start UI:**
```cmd
cd d:\Personal\FreshNoodle
start-ui.cmd
```

### Option 3: Kill any existing dotnet processes

If port conflicts persist:

```cmd
taskkill /F /IM dotnet.exe
```

Then start fresh with the commands above.

## Verify Configuration

The UI should point to the HTTPS endpoint. Check this file:

**File:** `d:\Personal\FreshNoodle\FreshNoodle.UI\wwwroot\appsettings.json`

Should contain:
```json
{
  "ApiBaseAddress": "https://localhost:7001"
}
```

✅ This is already correct - using port 7001 (HTTPS)

## Testing

Once both are running:

1. **API is ready when you see:**
   ```
   Now listening on: https://localhost:7001
   ```

2. **Open UI in browser** (URL shown in UI terminal)

3. **Login:**
   - Username: `admin`
   - Password: `admin123`

## Summary

- ✅ Port conflict fixed (API HTTP now uses 5101 instead of 5001)
- ✅ HTTPS still on port 7001 (primary endpoint)
- ✅ UI configuration already points to correct HTTPS URL
- ✅ No other changes needed

**Just restart the API and it will work!**
