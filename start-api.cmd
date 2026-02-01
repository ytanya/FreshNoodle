@echo off
echo ================================
echo Starting FreshNoodle Authentication API
echo ================================
echo.
echo API will be available at:
echo   - HTTPS: https://localhost:7001 (Primary)
echo   - HTTP:  http://localhost:5101 (Fallback)
echo.
echo IMPORTANT: Use HTTPS URL (7001) in UI configuration
echo.
echo Press Ctrl+C to stop the API
echo ================================
echo.

cd FreshNoodle.API.Authentication
dotnet run
