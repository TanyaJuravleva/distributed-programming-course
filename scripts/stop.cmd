@ECHO OFF

taskkill /F /IM nats-server.exe
taskkill /f /IM dotnet.exe
taskkill /f /IM nginx.exe