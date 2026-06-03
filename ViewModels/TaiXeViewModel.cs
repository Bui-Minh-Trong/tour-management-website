using System.ComponentModel.DataAnnotations;

namespace QuanLyTourDuLich.ViewModels;

public class TaiXeViewModel
{
    public int MaTaiXe { get; set; }

    [Required(ErrorMessage = "Họ tên tài xế không được để trống.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ Tên")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số Điện Thoại")]
    public string Sdt { get; set; } = null!;

    [Required(ErrorMessage = "Số giấy phép lái xe không được để trống.")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Số GPLX phải gồm 12 chữ số.")]
    [Display(Name = "Số GPLX")]
    public string SoGplx { get; set; } = null!;

    [Required(ErrorMessage = "Loại bằng lái không được để trống.")]
    [Display(Name = "Loại Bằng Lái")]
    public string LoaiBangLai { get; set; } = "Hạng D";
}
