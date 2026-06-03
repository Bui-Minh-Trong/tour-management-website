using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTourDuLich.ViewModels;

public class KhachHangViewModel
{
    public int MaKh { get; set; }

    [Required(ErrorMessage = "Họ tên không được để trống.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ Tên")]
    public string HoTen { get; set; } = null!;

    [Display(Name = "Giới Tính")]
    public string? GioiTinh { get; set; } = "Nam";

    [DataType(DataType.Date)]
    [Display(Name = "Ngày Sinh")]
    public DateOnly? NgaySinh { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.")]
    [Display(Name = "Số Điện Thoại")]
    public string Sdt { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [RegularExpression(@"^\d{12}$", ErrorMessage = "Số CCCD phải gồm đúng 12 chữ số.")]
    [Display(Name = "Số CCCD")]
    public string? Cccd { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
    [Display(Name = "Địa Chỉ")]
    public string? DiaChi { get; set; }
}
