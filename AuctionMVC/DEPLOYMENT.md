# AuctionMVC — Deployment & Run Guide

This guide covers local development, configuration, and production deployment of the **AuctionMVC** admin dashboard alongside the **AuctionAPI** backend.

---

## 1. Architecture Overview

```
┌─────────────────┐       HTTP/JSON        ┌──────────────────────┐
│  AuctionMVC      │ ───────────────────── ► │  AuctionAPI (net10)  │
│  (ASP.NET Core 10│   Typed Http Clients   │  Clean Architecture   │
│   MVC, RTL)      │ ◄───────────────────── │  EF Core + SQL Server │
└─────────────────┘       Swagger/JSON      └──────────────────────┘
```

- **AuctionMVC** is the browser-facing presentation layer (Razor, Bootstrap 5.3 RTL).
- **AuctionAPI** exposes REST endpoints (`/api/auctions`, `/api/products`, `/api/users`, `/api/bids`, `/api/winners`).
- **AuctionMVC** never touches the database directly — all data flows through the API.

---

## 2. Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0+ | Build & run both projects |
| SQL Server | LocalDB or any instance | Backend database |
| Browser | Modern (Chrome/Edge/Firefox) | Render the RTL dashboard |

---

## 3. Local Development

### 3.1 Start the API

```bash
cd lokmann/AuctionAPI/WebAPI
dotnet restore
dotnet run
```

- API URL: `http://localhost:5051`
- Swagger UI: `http://localhost:5051/swagger`
- The database auto-creates via `EnsureCreated()` on startup (connection string in `WebAPI/appsettings.json`).

> ⚠ If the default connection string (`Server=(localdb)\mssqllocaldb`) is unavailable, install **SQL Server Express LocalDB** or change the connection string to your SQL Server.

### 3.2 Start the MVC App

```bash
cd lokmann/AuctionMVC
dotnet restore
dotnet run
```

- MVC URL: `http://localhost:5000` (or the port shown in terminal).
- Open the browser and log in with the local fallback credentials.

### 3.3 Login

| Username | Password |
|----------|----------|
| `admin`  | `Admin@123` |

These are configured under `Auth:LocalFallback` in `appsettings.json`. The fallback is **enabled by default** because the backend does not yet expose `POST /api/auth/login`.

When the backend auth endpoint is available:

1. Add `POST /api/auth/login` to the API returning `{ token, displayName, email, roles }`.
2. Set `"Auth": { "LocalFallback": { "Enabled": false } }` in the MVC app.

---

## 4. Configuration Reference

### `appsettings.json` (AuctionMVC)

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5051",
    "TimeoutSeconds": 30
  },
  "Auth": {
    "LocalFallback": {
      "Enabled": true,
      "Username": "admin",
      "Password": "Admin@123",
      "DisplayName": "مدير النظام"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

| Key | Description |
|-----|-------------|
| `Api:BaseUrl` | Backend API base URL. Change for different environments (dev/staging/prod). |
| `Api:TimeoutSeconds` | HTTP client timeout for API calls. |
| `Auth:LocalFallback` | Temporary local login bridge; disable when backend auth exists. |

### `appsettings.json` (AuctionAPI/WebAPI)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuctionDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Change this to the target SQL Server instance for the environment.

---

## 5. Environment Configuration

To support multiple environments, override `appsettings.json` using environment-specific files or environment variables:

### Development (default)

`appsettings.Development.json` — uses local `localhost:5051` API.

### Production

Create `appsettings.Production.json`:

```json
{
  "Api": {
    "BaseUrl": "https://api-auction.example.com",
    "TimeoutSeconds": 30
  }
}
```

### Environment variables (alternative)

```bash
set Api__BaseUrl=https://api-auction.example.com
set Auth__LocalFallback__Enabled=false
```

ASP.NET Core maps `__` (double underscore) to configuration section separators.

---

## 6. Running in Production

### 6.1 Publish the API

```bash
cd lokmann/AuctionAPI/WebAPI
dotnet publish -c Release -o ./publish
```

Deploy the `publish/` folder to the server (IIS, Kestrel behind Nginx/Apache, Docker, etc.). Set the connection string and `ASPNETCORE_ENVIRONMENT=Production`.

### 6.2 Publish the MVC App

```bash
cd lokmann/AuctionMVC
dotnet publish -c Release -o ./publish
```

Deploy the `publish/` folder to the web server. Point `Api:BaseUrl` at the deployed API.

### 6.3 Reverse Proxy (Nginx — RTL site)

```nginx
server {
    listen 80;
    server_name admin.example.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }
}
```

### 6.4 HTTPS

- Use HTTPS in production.
- Configure redirect: `app.UseHttpsRedirection()` is enabled in `Program.cs`.
- Terminate TLS at the reverse proxy or load balancer.

---

## 7. Troubleshooting

| Symptom | Likely Cause | Fix |
|---------|--------------|-----|
| Login always fails | Backend offline / fallback disabled | Start the API, or enable `Auth:LocalFallback` |
| `500` on every page | API not reachable | Check `Api:BaseUrl`, start API, verify CORS/URL |
| All lists empty | Database empty | Add data via Swagger or API POST endpoints |
| `InvalidOperationException` EF errors (API) | DB connection string wrong | Fix `DefaultConnection` in API `appsettings.json` |
| DataTables buttons not showing | CDN blocked offline | Vendr the libs into `wwwroot/lib` or use local network CDN |
| RTL icons reversed | Missing `bootstrap.rtl.min.css` | Confirm the RTL stylesheet is loaded first in `_AdminLayout.cshtml` |

---

## 8. CDN Dependencies

The layout loads these from CDNs (internet required):

| Library | URL |
|---------|-----|
| Bootstrap 5.3 RTL CSS/JS | `cdn.jsdelivr.net/npm/bootstrap@5.3.3` |
| Bootstrap Icons | `cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3` |
| jQuery | `code.jquery.com/jquery-3.7.1.min.js` |
| DataTables + Buttons | `cdn.datatables.net` |
| HTMX | `unpkg.com/htmx.org@1.9.12` |
| Alpine.js | `unpkg.com/alpinejs@3.14.1` |
| Google Fonts (Almarai) | `fonts.googleapis.com` |

For **air-gapped** environments, download all assets and reference local paths under `wwwroot/lib/`.

---

## 9. Build Verification

```bash
cd lokmann/AuctionMVC
dotnet build
```

Expected output:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## 10. Missing Backend Endpoints (Roadmap)

| Endpoint | Status | Impact |
|----------|--------|--------|
| `POST /api/auth/login` | 🔴 Missing | Login currently uses `Auth:LocalFallback` bridge |
| `POST /api/auth/refresh` | 🔴 Missing | Token refresh for cookie auth |
| Category CRUD | 🔴 Missing | Categories derived from products |
| `MinIncrement` on Auction | 🔴 Missing | UI renders derived hint instead |
| Auto-winner compute on finish | 🔴 Missing | Winners managed manually |

---

## 11. Summary

The AuctionMVC admin dashboard is **production-ready at the presentation layer**:

- ✅ Full RTL Arabic admin UI (Bootstrap 5.3, Almarai, Bootstrap Icons)
- ✅ Typed API clients (IHttpClientFactory) with `System.Text.Json`
- ✅ DataTables with Arabic localization + export buttons
- ✅ Confirmation modals (gold for stop, red for delete)
- ✅ Cookie auth with JWT-forwarding plumbing
- ✅ `0 warnings`, `0 errors` build
- ✅ Documented backend gaps with clear roadmap

Once the backend auth endpoints and any remaining entities are added, the dashboard will be fully self-contained against a real production API.
