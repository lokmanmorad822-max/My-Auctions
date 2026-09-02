# 🏆 نظام إدارة المزادات - AuctionMVC

## نظرة عامة

**AuctionMVC** هو لوحة تحكم إدارية (Admin Dashboard) متكاملة لنظام إدارة المزادات، مبني بتقنية **ASP.NET Core 10 MVC** كواجهة أمامية تتصل بـ **AuctionAPI** (Backend) المبنية بـ Clean Architecture.

---

## 🎯 الهدف من المشروع

توفير واجهة إدارية عربية احترافية تمكن مدير النظام من:
- إدارة المزادات والمنتجات والمستخدمين
- متابعة العطاءات وتحديد الفائزين
- مراقبة أداء النظام عبر لوحة تحكم تفاعلية
- الموافقة على المزادات أو رفضها أو إيقافها

---

## 🛠️ التقنيات المستخدمة

### Backend (AuctionAPI)
- **Clean Architecture** (Domain → Application → Infrastructure → WebAPI)
- **Entity Framework Core** + SQL Server
- **ASP.NET Core Web API**
- **Swagger UI** للتوثيق

### Frontend (AuctionMVC)
- **ASP.NET Core 10 MVC** مع Razor Pages
- **Bootstrap 5.3 RTL** للتصميم المتجاوب
- **DataTables.js** مع الترجمة العربية للجداول التفاعلية
- **HTMX** + **Alpine.js** للتفاعلات الديناميكية
- **Bootstrap Icons** للأيقونات
- **خط Almarai** من Google Fonts للعربية
- **Cookie Authentication** مع JWT forwarding

---

## 🏗️ البنية المعمارية

```
AuctionMVC (Frontend)
├── Controllers/          → MVC Actions (Dashboard, Auctions, Products, Users, Bids, Winners)
├── Services/            → منطق الأعمال + عملاء API
│   ├── Api/             → Typed HttpClient Clients
│   ├── AuthService.cs   → إدارة المصادقة
│   ├── DashboardService.cs → تجميع بيانات لوحة التحكم
│   ├── AuctionManagementService.cs → إدارة المزادات
│   └── ...
├── ViewModels/          → نماذج الصفحات
├── Contracts/           → DTOs (تطابق الـ Backend)
├── Filters/             → معالجة الأخطاء
├── TagHelpers/          → مساعدات Razor (StatusBadge, Currency, ActiveRoute)
├── ViewComponents/      → مكونات قابلة لإعادة الاستخدام (Sidebar, Topbar, StatCard)
└── Views/               → صفحات Razor (عربي RTL)
```

---

## ✨ الميزات الرئيسية

### 1. لوحة التحكم (Dashboard)
- بطاقات إحصائية (إجمالي المزادات، المنتجات، المستخدمين، العطاءات)
- آخر المزادات النشطة
- أفضل مزاد
- رسوم بيانية تفاعلية

### 2. إدارة المزادات (Auctions)
- **قائمة المزادات** مع فلاتر (الكل، النشطة، المعلقة، المنتهية، المتوقفة، المرفوضة)
- **إنشاء مزاد** مع منتج جديد
- **تعديل مزاد**
- **تفاصيل المزاد** مع عرض العطاءات والفائز
- **إجراءات المشرف:**
  - ✅ موافقة على المزاد المعلق
  - ❌ رفض المزاد
  - ⏹️ إيقاف المزاد النشط
  - 🗑️ حذف مع تأكيد

### 3. إدارة المنتجات (Products)
- إنشاء، تعديل، حذف المنتجات
- ربط المنتجات بالمزادات

### 4. إدارة المستخدمين (Users)
- إنشاء، تعديل، حذف المستخدمين
- إدارة الأدوار والصلاحيات

### 5. العطاءات والفائزين (Bids & Winners)
- عرض جميع العطاءات
- إدارة الفائزين يدوياً

### 6. التصنيفات (Categories)
- عرض التصنيفات المشتقة من المنتجات

---

## 🎨 نظام التصميم

| العنصر | القيمة |
|--------|--------|
| اللون الأساسي | `#0F8A57` (أخضر) |
| اللون الذهبي | `#D9A82E` |
| لون الشريط الجانبي | `#262B3B` (داكن) |
| الخلفية | `#FBFBFA` |
| الخط | Almarai (عربي) |
| الاتجاه | RTL (من اليمين لليسار) |

### ميزات التصميم:
- **شارات الحالة** ملونة (نشط=أخضر، معلق=ذهبي، منتهي=رمادي، متوقف=أحمر، مرفوض=داكن)
- **نوافذ تأكيد** مع أيقونات (ذهبي للإيقاف، أحمر للحذف)
- **جداول تفاعلية** مع تصدير (Excel, CSV, Print)
- **تصميم متجاوب** يعمل على جميع الأجهزة

---

## 🔐 الأمان

- **Cookie Authentication** مع انتهاء صلاحية 8 ساعات
- **Authorization** للوصول للصفحات الإدارية
- **JWT Forwarding** للـ API
- **معالجة الأخطاء** الموحدة عبر `HandleApiErrorFilter`

---

## 📡 الاتصال مع الـ Backend

- **Typed HttpClient** عبر `IHttpClientFactory`
- **عملاء API منفصلون:** Auctions, Products, Users, Bids, Winners, Auth
- **System.Text.Json** مع سياسة `camelCase`
- **CancellationToken** للعمليات غير المتزامنة

---

## 🚀 تشغيل المشروع

```bash
# 1. استعادة الحزم
dotnet restore

# 2. تشغيل الـ Backend (في terminal منفصل)
cd lokmann/AuctionAPI/WebAPI
dotnet run

# 3. تشغيل الـ Frontend
cd lokmann/AuctionMVC
dotnet run
```

**بيانات الدخول الافتراضية:**
- Username: `admin`
- Password: `Admin@123`

---

## 📋 الصفحات الإدارية

| الصفحة | الوظيفة |
|--------|---------|
| Dashboard | نظرة عامة وإحصائيات |
| المزادات | إدارة كاملة للمزادات |
| المنتجات | إدارة المنتجات |
| المستخدمين | إدارة المستخدمين |
| العطاءات | عرض العطاءات |
| الفائزين | إدارة الفائزين |
| التصنيفات | عرض التصنيفات |

---

## ⚠️ ملاحظات تقنية

- **المصادقة:** يستخدم `LocalFallback` كجسر مؤقت حتى يتوفر endpoint `/api/auth/login` في الـ Backend
- **التصنيفات:** مشتقة من المنتجات (لا يوجد endpoint خاص)
- **حقل MinIncrement:** غير موجود في الـ Backend، يعرض كتلميح مشتق
- **تحديد الفائز:** يتم يدوياً من قبل المدير (لا يوجد endpoint تلقائي)

---

## 📄 الرخصة

مشروع داخلي — نظام إدارة المزادات.
