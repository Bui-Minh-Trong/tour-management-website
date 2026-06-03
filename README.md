# 🌍 Tour Management Website

> Hệ thống quản lý tour du lịch trực tuyến được xây dựng bằng ASP.NET Core MVC.

---

## 📋 Giới thiệu

**Tour Management Website** là ứng dụng web quản lý nghiệp vụ du lịch toàn diện, hỗ trợ các phòng ban trong một công ty lữ hành:

- **Quản trị viên**: Quản lý tài khoản, phân quyền người dùng
- **Nhân viên Kinh doanh**: Đặt tour cho khách hàng, quản lý danh mục
- **Nhân viên Điều hành**: Lập kế hoạch đoàn, phân công hướng dẫn viên & xe
- **Nhân viên Kế toán**: Lập phiếu thu/chi, quyết toán lãi lỗ tour

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Backend Framework | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Authentication | Cookie Authentication (ASP.NET Core Identity Hasher) |
| UI Framework | Bootstrap 5 + Font Awesome 6 |
| Charts | Chart.js |
| IDE | Visual Studio 2022 |

---

## 🗂️ Cấu trúc dự án

```
QuanLyTourDuLich/
├── Areas/
│   └── Admin/
│       ├── Controllers/       # Controllers cho khu vực Admin
│       └── Views/             # Giao diện Razor Views
├── Controllers/               # AuthController, HomeController
├── Models/                    # Entity Models (EF Core)
├── ViewModels/                # ViewModels cho từng form/danh sách
├── Services/                  # Business Logic Services
├── Views/                     # Shared views & Auth views
├── wwwroot/                   # Static files (CSS, JS, Images)
└── SQLQueryTourManagementUpdate.sql  # Script khởi tạo CSDL
```

---

## 🚀 Hướng dẫn cài đặt

### Yêu cầu
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2019+](https://www.microsoft.com/sql-server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (khuyên dùng)

### Các bước

1. **Clone repository**
   ```bash
   git clone https://github.com/Bui-Minh-Trong/tour-management-website.git
   cd tour-management-website
   ```

2. **Khởi tạo cơ sở dữ liệu**
   - Mở SQL Server Management Studio
   - Chạy file `SQLQueryTourManagementUpdate.sql`

3. **Cấu hình connection string**
   - Mở `appsettings.json`
   - Cập nhật `ConnectionStrings.DefaultConnection` phù hợp với SQL Server của bạn

4. **Chạy ứng dụng**
   ```bash
   dotnet run
   ```
   Hoặc mở `QuanLyTourDuLich.sln` trong Visual Studio và nhấn `F5`

---

## 📊 Sơ đồ chức năng (Use Case)

| Nhóm chức năng | Mô tả |
|---|---|
| 1.0 Quản trị hệ thống | Đăng nhập, quản lý tài khoản, phân quyền, đổi mật khẩu |
| 2.0 Quản lý danh mục | Tour, Khách hàng, Nhân viên, Đối tác & NCC |
| 3.0 Quản lý nghiệp vụ | Đặt tour, phân công HDV & xe, cập nhật đoàn |
| 4.0 Quản lý tài chính | Lập phiếu thu/chi, quyết toán lãi lỗ tour |
| 5.0 Báo cáo & Thống kê | Biểu đồ tổng quan, báo cáo doanh thu, xuất Excel/PDF |

---

## 👨‍💻 Tác giả

| Thông tin | Chi tiết |
|---|---|
| **Họ và tên** | Bùi Minh Trọng |
| **GitHub** | [@Bui-Minh-Trong](https://github.com/Bui-Minh-Trong) |
| **Môn học** | Phân Tích Thiết Kế Hệ Thống |
| **Trường** | Đại học Giao thông Vận tải TP.HCM (UTC2) |

---

## 📄 License

MIT License — Dự án học thuật, không sử dụng cho mục đích thương mại.
