@ECHO OFF

start start "" /D "D:\nats-server" nats-server.exe

cd ../RankCalculator
start cmd /k dotnet run

cd ../Valuator
start cmd /k dotnet run --urls "http://0.0.0.0:5001"
start cmd /k dotnet run --urls "http://0.0.0.0:5002"

cd D:/nginx
start cmd /k nginx.exe