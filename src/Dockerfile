# ─── Stage 1: Build Angular ───────────────────────────────────────────────────
FROM node:20-alpine AS angular-build
WORKDIR /app
COPY client/fireholde-under-attack/package*.json ./
RUN npm ci
COPY client/fireholde-under-attack/ ./
# Replace the deploy-time placeholder with an empty string so Angular uses
# relative URLs — all API calls are proxied by nginx within the same container.
RUN sed -i "s|'%%API_URL%%'|''|g" src/environments/environment.prod.ts
RUN npm run build -- --configuration production

# ─── Stage 2: Build .NET ──────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet-build
WORKDIR /src
COPY server/FireholdeUnderAttack/FireholdeUnderAttack/FireholdeUnderAttack.csproj FireholdeUnderAttack/
RUN dotnet restore "FireholdeUnderAttack/FireholdeUnderAttack.csproj"
COPY server/FireholdeUnderAttack/FireholdeUnderAttack/ FireholdeUnderAttack/
RUN dotnet publish "FireholdeUnderAttack/FireholdeUnderAttack.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

# ─── Stage 3: Runtime image ───────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update \
    && apt-get install -y --no-install-recommends nginx \
    && rm -rf /var/lib/apt/lists/*

# Angular static files served by nginx
COPY --from=angular-build /app/dist/fireholde-under-attack/browser /usr/share/nginx/html

# nginx site config (replaces the default)
COPY nginx.conf /etc/nginx/sites-enabled/default

# .NET application
COPY --from=dotnet-build /app/publish /app

# Startup script — launches .NET then nginx
COPY start.sh /start.sh
RUN chmod +x /start.sh

EXPOSE 8080
CMD ["/start.sh"]
