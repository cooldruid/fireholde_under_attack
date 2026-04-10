#!/bin/sh
set -e

# Start .NET backend on internal port 5000
ASPNETCORE_URLS=http://+:5000 \
ASPNETCORE_ENVIRONMENT=Production \
dotnet /app/FireholdeUnderAttack.dll &

# Give .NET a moment to bind before nginx starts accepting traffic
sleep 1

# Run nginx in foreground (keeps the container alive)
exec nginx -g 'daemon off;'
