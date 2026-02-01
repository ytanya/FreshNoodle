@echo off
echo ================================
echo Starting FreshNoodle Blazor UI
echo ================================
echo.
echo Make sure the Authentication API is running first!
echo.
echo UI will be available at:
echo   - HTTPS: https://localhost:5173 (or similar)
echo.
echo Press Ctrl+C to stop the UI
echo ================================
echo.

cd FreshNoodle.UI
dotnet run
