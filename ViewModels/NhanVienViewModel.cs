using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTourDuLich.ViewModels;

public class NhanVienViewModel
{
    public int MaNv { get; set; }

    [Required(ErrorMessage = "Họ tên nhân viên không được để trống.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ Tên")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Chức vụ không được để trống.")]
    [StringLength(50, ErrorMessage = "Chức vụ không được vượt quá 50 ký tự.")]
    [Display(Name = "Chức Vụ")]
    public string ChucVu { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.")]
    [Display(Name = "Số Điện Thoại")]
    public string Sdt { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Ngày vào làm không được để trống.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày Vào Làm")]
    public DateOnly NgayVaoLam { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    // Thuộc tính phục vụ phần tài khoản người dùng liên kết
    [Display(Name = "Đã có tài khoản")]
    public bool HasAccount { get; set; }

    [Display(Name = "Tên Đăng Nhập")]
    public string? TenDangNhap { get; set; }

    [Display(Name = "Vai Trò")]
    public string? TenVaiTro { get; set; }
}
