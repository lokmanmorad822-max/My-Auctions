# AuctionMVC — Admin Dashboard

**ASP.NET Core 10 MVC** admin frontend for the Auction Management System, consuming the `AuctionAPI` Clean Architecture backend.

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (or any SQL Server instance)
- The backend API must be running (see [Running the Backend](#running-the-backend))

### 1. Clone & Restore

```bash
cd lokmann/AuctionMVC
dotnet restore
```

### 2. Configure

Edit `appsettings.json`:

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
  }
}
```

> **Note:** `Auth:LocalFallback` is a temporary bridge. The backend does not yet expose `POST /api/auth/login`. When the endpoint is added, set `LocalFallback.Enabled = false`.

### 3. Run the Backend

In a separate terminal:

```bash
cd lokmann/AuctionAPI/WebAPI
dotnet run
```

The API starts at `http://localhost:5051` with Swagger UI at `http://localhost:5051/swagger`.

### 4. Run the MVC App

```bash
cd lokmann/AuctionMVC
dotnet run
```

Open `http://localhost:5000` (or the URL shown in the terminal).

### 5. Login

| Username | Password | Role |
|----------|----------|------|
| `admin`  | `Admin@123` | Admin (local fallback) |

---

## 🏗️ Architecture

```
Solution: AuctionAPI (Backend — Clean Architecture)
├── Domain          → Entities, Enums
├── Application     → DTOs, Interfaces
├── Infrastructure  → EF Core, SqlServer, Services
└── WebAPI          → Controllers, Middleware, Swagger

Solution: AuctionMVC (Frontend — ASP.NET Core MVC)
├── Controllers/        → MVC actions
├── ViewModels/         → Strongly-typed page models
├── Services/           → Business logic, API clients
├── Options/            → Configuration POCOs
├── Contracts/          → API DTOs (mirrors backend)
├── Filters/            → Exception handling
├── TagHelpers/         → Reusable Razor helpers
├── ViewComponents/     → Sidebar, Topbar, StatCard, etc.
├── Views/              → Razor pages (RTL Arabic)
└── wwwroot/            → CSS, JS, libraries
```

### Key Design Decisions

| Concern | Approach |
|---------|----------|
| **API Communication** | `IHttpClientFactory` + typed API clients (`AuctionsApiClient`, `ProductsApiClient`, etc.) |
| **Authentication** | Cookie auth with `AuthService` (fallback to local bridge when backend lacks auth endpoint) |
| **Serialization** | `System.Text.Json` with `camelCase` policy |
| **Error Handling** | `HandleApiErrorFilter` — catches API exceptions, redirects to error page or returns JSON for AJAX |
| **UI Framework** | Bootstrap 5.3 RTL, Bootstrap Icons, DataTables.js (Arabic), HTMX, Alpine.js |
| **Typography** | Almarai (Google Fonts) |
| **Localization** | Arabic (hard-coded strings, DataTables AR dictionary in `datatables.init.js`) |

---

## 📋 Admin Pages

| Page | Controller | View | Backend Endpoint |
|------|-----------|------|-----------------|
| Dashboard | `DashboardController` | `Views/Dashboard/Index.cshtml` | `GET /api/auctions`, `/api/products`, `/api/bids`, `/api/users`, `/api/winners` |
| Auctions | `AuctionsController` | `Views/Auctions/Index.cshtml` | `GET /api/auctions` |
| Auction Details | `AuctionsController` | `Views/Auctions/Details.cshtml` | `GET /api/auctions/{id}`, `/api/bids`, `/api/winners` |
| Create Auction | `AuctionsController` | `Views/Auctions/Create.cshtml` | `POST /api/auctions` + `POST /api/products` |
| Edit Auction | `AuctionsController` | `Views/Auctions/Edit.cshtml` | `PUT /api/auctions/{id}` |
| Products | `ProductsController` | `Views/Products/Index.cshtml` | `GET /api/products` |
| Create Product | `ProductsController` | `Views/Products/Create.cshtml` | `POST /api/products` |
| Edit Product | `ProductsController` | `Views/Products/Edit.cshtml` | `PUT /api/products/{id}` |
| Users | `UsersController` | `Views/Users/Index.cshtml` | `GET /api/users` |
| Create User | `UsersController` | `Views/Users/Create.cshtml` | `POST /api/users` |
| Edit User | `UsersController` | `Views/Users/Edit.cshtml` | `PUT /api/users/{id}` |
| Bids | `BidsController` | `Views/Bids/Index.cshtml` | `GET /api/bids` |
| Winners | `WinnersController` | `Views/Winners/Index.cshtml` | `GET /api/winners` |
| Categories | `CategoriesController` | `Views/Categories/Index.cshtml` | derived from `GET /api/products` |
| Login | `AccountController` | `Views/Account/Login.cshtml` | fallback (no backend auth endpoint) |

### Moderation & Actions

- **Approve/Reject** — Pending auctions → `POST /api/auctions/{id}/approve` / `reject`
- **Stop** — Active auctions → `POST /api/auctions/{id}/stop` (confirmation modal)
- **Delete** — Auctions, Products, Users, Winners → confirmation modal with "حذف" text input

---

## 🧩 Project Structure

```
lokmann/AuctionMVC/
├── Controllers/                    # MVC controllers
│   ├── AccountController.cs        # Login/Logout/AccessDenied
│   ├── AuctionsController.cs       # CRUD + moderation
│   ├── BidsController.cs           # Read-only index
│   ├── CategoriesController.cs     # Read-only index
│   ├── DashboardController.cs      # Home page
│   ├── HomeController.cs           # Error/NotFound pages
│   ├── ProductsController.cs       # CRUD
│   ├── UsersController.cs          # CRUD
│   └── WinnersController.cs        # List + delete
├── ViewModels/                     # Strongly-typed page models
├── Services/                       # Business logic + API clients
│   ├── Api/                        # Typed HttpClient clients
│   ├── AuthService.cs              # Login orchestration
│   ├── DashboardService.cs         # Aggregated dashboard data
│   ├── AuctionManagementService.cs # Auction + bid + product logic
│   ├── ProductManagementService.cs
│   ├── UserManagementService.cs
│   ├── BidManagementService.cs
│   ├── WinnerManagementService.cs
│   └── CategoryService.cs          # Derived from products
├── Options/                        # ApiOptions, AuthOptions
├── Contracts/                      # API DTOs
├── Filters/                        # HandleApiErrorFilter
├── TagHelpers/                     # StatusBadge, ActiveRoute, Currency
├── ViewComponents/                 # Sidebar, Topbar, StatCard, PageHeader, StatusFilter
├── Views/                          # Razor pages
│   ├── Shared/                     # Layout, partials, component views
│   └── {Controller}/               # Per-controller views
└── wwwroot/                        # Static assets
    ├── css/site.css                # Design system
    └── js/                         # site.js, datatables.init.js, modals.js, dashboard.js
```

---

## 🎨 Design System (Pixel Fidelity)

The UI matches the original frontend design board with:

| Token | Original (oklch) | Bootstrap Equivalent |
|-------|-----------------|---------------------|
| Primary | `0.42 0.14 155` | `--bs-primary: #0F8A57` |
| Gold | `0.78 0.14 82` | `--bs-gold: #D9A82E` |
| Sidebar | `0.20 0.02 260` | `--bs-sidebar: #262B3B` |
| Background | `0.985 0.005 95` | `--bs-body-bg: #FBFBFA` |
| Card | `1 0 0` | `--bs-card-bg: #FFFFFF` |
| Border | `0.90 0.01 100` | `--bs-border-color: #E6E3DC` |

- **RTL Layout** matching the original `dir="rtl"` structure
- **Dark sidebar** with green accent (same as design)
- **Status badges** with color-coded pills (Active=green, Pending=gold, Finished=gray, Stopped=red, Rejected=dark)
- **Confirmation modals** with icon (gold for stop, red for delete) + optional "اكتب 'حذف'" text input
- **DataTables** with Arabic localization, export buttons (Copy, CSV, Excel, Print)

---

## 🔧 Build & Verify

```bash
cd lokmann/AuctionMVC
dotnet build
# Expected: Build succeeded. 0 Warning(s) 0 Error(s)
```

---

## ⚠️ Known Issues & Backend TODOs

### Missing Backend Endpoints

1. **`POST /api/auth/login`** + `POST /api/auth/refresh` — Authentication (MVC uses `Auth:LocalFallback` bridge until available)
2. **Category CRUD** — Categories are derived from `GET /api/products`; no dedicated category endpoints
3. **`MinIncrement` field** — Not present in `Auction` entity; UI renders a derived hint
4. **Auto-determine winners** — No backend endpoint to compute winner on auction finish; admin manages winners manually

### Runtime Notes

- The `_AdminLayout` uses `_ViewStart.cshtml` which sets `Layout = "_AdminLayout"` — all pages use the admin shell
- The Login page (`Views/Account/Login.cshtml`) sets `Layout = null` to avoid the admin shell
- `GlobalExceptionMiddleware` in the backend returns JSON errors; the MVC `HandleApiErrorFilter` catches them and redirects to `/Home/Error`
- All API calls use `CancellationToken` for proper async cancellation

---

## 📄 License

Internal project — Auction Management System.
