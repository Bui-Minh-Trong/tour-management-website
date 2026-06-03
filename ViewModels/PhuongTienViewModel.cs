using System.ComponentModel.DataAnnotations;

namespace QuanLyTourDuLich.ViewModels;

public class PhuongTienViewModel
{
    public int MaPhuongTien { get; set; }

    [Required(ErrorMessage = "Biển số xe không được để trống.")]
    [RegularExpression(@"^\d{2}[A-Z]-\d{4,5}$", ErrorMessage = "Biển số xe không đúng định dạng (VD: 59A-12345 hoặc 29B-9999).")]
    [Display(Name = "Biển Số Xe")]
    public string BienSoXe { get; set; } = null!;

    [Required(ErrorMessage = "Loại xe không được để trống.")]
    [StringLength(50, ErrorMessage = "Loại xe không được vượt quá 50 ký tự.")]
    [Display(Name = "Loại Xe")]
    public string LoaiXe { get; set; } = null!;

    [Required(ErrorMessage = "Số chỗ ngồi không được để trống.")]
    [Range(4, 80, ErrorMessage = "Số chỗ ngồi phải từ 4 đến 80 chỗ.")]
    [Display(Name = "Số Chỗ Ngồi")]
    public int? SoChoNgoi { get; set; } = 45;

    [Required(ErrorMessage = "Trạng thái xe không được để trống.")]
    [Display(Name = "Trạng Thái")]
    public string TrangThai { get; set; } = "Hoạt động";
}
