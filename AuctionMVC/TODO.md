# AuctionMVC — Admin Dashboard Implementation Roadmap

## Stack
ASP.NET Core 10 MVC · Areas (Admin) · Clean Architecture · Bootstrap 5.3 RTL · DataTables.js (AR) · HTMX · Alpine.js · Bootstrap Icons · Almarai · Typed Http Clients (IHttpClientFactory)

---

## Steps

- [x] S1 — Scaffold project (csproj, Program.cs, appsettings, DI, Cookie auth, typed clients)
- [x] S2 — Design system: `wwwroot/css/site.css` (tokens → Bootstrap 5.3 RTL, Almarai), auth.css, dashboard.css
- [x] S3 — Client-side libs & JS: site.js, datatables.init.js (Arabic), modals.js, dashboard.js
- [x] S4 — Options + Contracts (API DTOs) + ApiClientBase + typed clients (Auctions/Products/Users/Bids/Winners/Auth)
- [x] S5 — Services layer (Auth, Dashboard, Auction, Product, User, Bid, Winner, Category)
- [x] S6 — ViewModels (Account, Dashboard, Auctions, Products, Users, Bids, Winners, Categories, Settings)
- [x] S7 — Filters (HandleApiError), TagHelpers (StatusBadge, ActiveRoute, Currency), ViewComponents (Sidebar, StatCard, Topbar, StatusFilter, PageHeader)
- [x] S8 — Shared layout: _AdminLayout, _Sidebar, _Topbar, _Alerts, _ValidationSummary, modals, Error/AccessDenied
- [x] S9 — Account (Login/Logout/AccessDenied) + Auth forwarding (JWT/cookie + local fallback)
- [x] S10 — Dashboard Home (stat cards, recent auctions, top auction, charts)
- [x] S11 — Auctions: Index (DataTable+filters), Moderation (approve/reject), Details (+bids), Create, Edit, Stop/Delete
- [x] S12 — Products: Index/Create/Edit/Delete
- [x] S13 — Users: Index/Create/Edit/Delete
- [x] S14 — Bids + Winners index pages
- [x] S15 — Categories + Settings pages (CategoriesController + View created, Category service wired in)
- [x] S16 — Final build verification: 0 warnings, 0 errors. All ViewModel property mismatches resolved.
- [x] S17 — Fixed ViewModel property mismatches: added `TotalCount`, `Initials`, `WinnerCount`, `AuctionProductName`, `BidderInitials`, `WinnerInitials`, `CreatedAt` on UserForm, `AuctionId` filter on Bids

---

## Missing Backend Endpoints (documented, TODO in code)
1. `POST /api/auth/login` + `POST /api/auth/refresh` — authentication (MVC uses `Auth:LocalFallback` bridge until available)
2. Dedicated auction moderation summary endpoint (derived from `GET /api/auctions` + status filter)
3. Category CRUD endpoint (Categories page derives distinct values from `GET /api/products`)
4. `MinIncrement` field on Auction (not in backend entity — UI renders derived hint)

