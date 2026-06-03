using System.ComponentModel.DataAnnotations;

namespace QuanLyTourDuLich.ViewModels;

public class KhachSanViewModel
{
    public int MaKs { get; set; }

    [Required(ErrorMessage = "Tên khách sạn không được để trống.")]
    [StringLength(100, ErrorMessage = "Tên khách sạn không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên Khách Sạn")]
    public string TenKs { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ khách sạn không được để trống.")]
    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
    [Display(Name = "Địa Chỉ")]
    public string DiaChi { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn số sao.")]
    [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5 sao.")]
    [Display(Name = "Số Sao (1-5)")]
    public int SoSao { get; set; } = 3;

    [Required(ErrorMessage = "Số điện thoại liên hệ không được để trống.")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
    [Display(Name = "Số Điện Thoại")]
    public string Sdt { get; set; } = null!;
}
