# FreshNoodle - Quick Reference Card

## 🚀 Start Commands

### Start API (Terminal 1)
```cmd
cd d:\Personal\FreshNoodle
start-api.cmd
```
Wait for: `Now listening on: https://localhost:7001`

### Start UI (Terminal 2)
```cmd
cd d:\Personal\FreshNoodle
start-ui.cmd
```
Open browser to URL shown (e.g., `https://localhost:5173`)

## 🔑 Login Credentials

| Username | Password |
|----------|----------|
| admin | admin123 |
| testuser | test123 |

## 🌐 Ports

| Service | HTTPS | HTTP |
|---------|-------|------|
| Auth API | 7001 | 5101 |
| Product API | 7003 | 5003 |
| User API | 7002 | 5002 |
| Blazor UI | 5173 | varies |

**Always use HTTPS (port 7001) for API calls**

## 📋 Sample Products (Pre-seeded)

- Laptop - $999.99
- Mouse - $29.99
- Keyboard - $79.99
- Monitor - $299.99

## ✅ Success Checklist

- [ ] API running on https://localhost:7001
- [ ] UI running and accessible in browser
- [ ] Can login with admin/admin123
- [ ] Products page shows 4 items
- [ ] Can create/edit/delete products

## 🔧 Troubleshooting

### Port Conflict Error
```
Error: Failed to bind to address... address already in use
```
**Fix:** Wait 30 seconds and retry, or kill dotnet processes:
```cmd
taskkill /F /IM dotnet.exe
```

### CORS Error in Browser
**Check:** UI config file points to https://localhost:7001
**File:** `FreshNoodle.UI/wwwroot/appsettings.json`

### Login Returns "Invalid credentials"
**Verify:** Database was seeded. Check API console output on startup.

### Can't See Products After Login
**Check:**
1. You're logged in (check browser localStorage for authToken)
2. API is running
3. No errors in browser console (F12)

## 📚 Documentation Files

- `README.md` - Full documentation
- `QUICK_START.md` - Getting started guide
- `TESTING_GUIDE.md` - Detailed testing steps
- `LOGIN_FIX_SUMMARY.md` - Login issue resolution
- `PORT_CONFLICT_FIX.md` - Port conflict resolution
- `QUICK_REFERENCE.md` - This file

## 🧪 Test API Directly (Optional)

Test login endpoint:
```bash
curl -k -X POST https://localhost:7001/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"username\":\"admin\",\"password\":\"admin123\"}"
```

Should return:
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJ...",
  "user": { ... }
}
```

## 💡 Tips

- Use HTTPS (port 7001) for API calls
- HTTP (port 5101) is fallback only
- Kill all dotnet processes before restart if having issues
- Check browser console (F12) for client-side errors
- Check API terminal for server-side errors
- Database auto-creates on first API run
- Database located at: `(localdb)\mssqllocaldb\FreshNoodleDb`

## 🎯 Common Tasks

### Clear Database and Reseed
1. Stop API
2. Delete database: `sqllocaldb delete mssqllocaldb`
3. Recreate: `sqllocaldb create mssqllocaldb`
4. Restart API (auto-seeds)

### Add New User via API
```bash
curl -k -X POST https://localhost:7001/api/auth/register ^
  -H "Content-Type: application/json" ^
  -d "{\"username\":\"newuser\",\"email\":\"new@test.com\",\"password\":\"pass123\"}"
```

### Rebuild Solution
```cmd
cd d:\Personal\FreshNoodle
dotnet build FreshNoodle.sln
```

---

**Need Help?** See [TESTING_GUIDE.md](TESTING_GUIDE.md) for detailed troubleshooting.
