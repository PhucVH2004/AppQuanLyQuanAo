# Quản Lý Cửa Hàng Quần Áo - Desktop Application

Ứng dụng **quản lý cửa hàng quần áo** dành cho các cửa hàng quy mô vừa và nhỏ (100 - 10.000 sản phẩm).  
Hỗ trợ bán hàng, quản lý kho, in hóa đơn, báo cáo doanh thu và phân quyền người dùng.

---

## ✨ Tính năng chính

| Chức năng | Mô tả |
|---------|-------|
| **Đăng nhập / Đăng ký / Quên mật khẩu** | Phân quyền: `Admin` (quản lý kho, báo cáo) và `User` (bán hàng) |
| **Quản lý sản phẩm** | Thêm, sửa, xóa, tìm kiếm, quản lý loại, hỗ trợ ảnh (JPEG/PNG) |
| **Bán hàng** | Giỏ hàng, thanh toán, in hóa đơn, tự động cập nhật tồn kho |
| **Quản lý giao dịch** | Xóa mềm (soft delete), khôi phục theo khoảng thời gian |
| **Báo cáo doanh thu** | Lọc theo ngày, tổng hợp doanh thu, xuất báo cáo (RDLC), in ấn |
| **Tìm kiếm & Lọc** | Tìm sản phẩm theo mã/tên/loại, lọc giao dịch theo ngày |

---

## 🛠 Công nghệ sử dụng

| Thành phần | Công cụ |
|-----------|--------|
| **Ngôn ngữ** | C# (.NET Framework 4.8) |
| **Giao diện** | Windows Forms |
| **Cơ sở dữ liệu** | Microsoft SQL Server Express |
| **Kết nối DB** | ADO.NET |
| **Báo cáo** | RDLC Report Viewer |
| **IDE** | Visual Studio 2022 |
| **Quản lý DB** | SQL Server Management Studio (SSMS) |
| **Quản lý mã nguồn** | Git |

---

## 📋 Cấu trúc dự án



---

## 🚀 Hướng dẫn cài đặt

### Yêu cầu hệ thống
- **Hệ điều hành**: Windows 10/11 (64-bit)
- **CPU**: Intel Core i3 trở lên
- **RAM**: 4GB
- **Dung lượng trống**: 500MB
- **.NET Framework 4.8**
- **SQL Server Express 2019+**

---

### Bước 1: Cài đặt SQL Server
1. Tải [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
2. Cài đặt với **Mixed Mode Authentication**
3. Ghi nhớ **SA password**

---

### Bước 2: Khôi phục cơ sở dữ liệu
1. Mở **SSMS** → Kết nối đến `(local)\SQLEXPRESS`
2. Mở file `Database/QLQA.sql`
3. Chạy toàn bộ script

> **Lưu ý**: Sửa chuỗi kết nối trong `App.config` nếu cần

---

### Bước 3: Chạy ứng dụng
1. Mở file `.sln` bằng **Visual Studio 2022**
2. Build dự án (`Ctrl + Shift + B`)
3. Chạy (`F5`)

> **Tài khoản mẫu**:
> - **Admin**: `admin` / `admin123`
> - **User**: `user1` / `123456`

---

## 🎨 Giao diện

| Màn hình | Ảnh minh họa |
|--------|-------------|
| Đăng nhập | ![Login](Screenshots/login.png) |
| Giao diện chính | ![Main](Screenshots/main.png) |
| Quản lý kho | ![Kho](Screenshots/kho.png) |
| Báo cáo | ![Report](Screenshots/report.png) |
| Hóa đơn | ![Invoice](Screenshots/invoice.png) |

---

## 🗄 Cấu trúc cơ sở dữ liệu

### `TaiKhoan`
| Cột | Kiểu | Ghi chú |
|-----|------|--------|
| `TenDangNhap` | `nvarchar(50)` PK | Tên đăng nhập |
| `MatKhau` | `nvarchar(50)` | Mật khẩu (plaintext) |
| `Email` | `nvarchar(100)` Unique | Email |
| `VaiTro` | `nvarchar(20)` | `admin` / `user` |

### `SanPham`
| Cột | Kiểu | Ghi chú |
|-----|------|--------|
| `MaSP` | `nvarchar(10)` PK | Mã sản phẩm |
| `TenSP` | `nvarchar(100)` | Tên |
| `DonGia` | `decimal(18,2)` | Đơn giá |
| `SoLuong` | `int` | Tồn kho |
| `HinhAnh` | `image` | Ảnh (byte[]) |
| `MaLoai` | `int` FK | Loại sản phẩm |

### `GiaoDich`
| Cột | Kiểu | Ghi chú |
|-----|------|--------|
| `GiaoDichID` | `int` Identity PK | Mã giao dịch |
| `MaSP` | `nvarchar(10)` | Mã SP |
| `SoLuong` | `int` | Số lượng |
| `NgayGiaoDich` | `datetime` | Thời gian |
| `DaXoa` | `bit` | 0: còn, 1: đã xóa mềm |

---

## 🔒 Hạn chế hiện tại

- Mật khẩu lưu **dạng plaintext**
- Chưa có **2FA**
- Chưa hỗ trợ **phân trang** với dữ liệu lớn
- Chỉ chạy trên **Windows**

---

## 🚧 Hướng phát triển

| Tính năng | Mô tả |
|---------|-------|
| **Mã hóa mật khẩu** | SHA-256 / bcrypt |
| **Phiên bản Web** | ASP.NET Core / Blazor |
| **Ứng dụng di động** | Flutter / Xamarin |
| **Quản lý khách hàng** | Lưu thông tin, lịch sử mua |
| **Khuyến mãi** | Mã giảm giá, điểm thưởng |
| **Thanh toán online** | VNPay, Momo |
| **Biểu đồ doanh thu** | ChartControl |
| **Sao lưu tự động** | Backup DB định kỳ |

---

## 📄 Giấy phép

Dự án sử dụng **MIT License** — tự do sử dụng, chỉnh sửa và phân phối.

---

## ⭐ Đóng góp

Bạn có thể:
- Báo lỗi (Issue)
- Gửi Pull Request
- Đề xuất tính năng mới

> **Cảm ơn bạn đã sử dụng dự án!**  
> Hãy **Star** nếu bạn thấy hữu ích!

---
