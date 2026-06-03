using System.ComponentModel.DataAnnotations;

namespace QuanLyTourDuLich.ViewModels;

public class TourViewModel
{
    public int MaTour { get; set; }

    [Required(ErrorMessage = "Tên Tour không được để trống.")]
    [StringLength(100, ErrorMessage = "Tên Tour không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên Tour")]
    public string TenTour { get; set; } = null!;

    [Required(ErrorMessage = "Điểm đến không được để trống.")]
    [StringLength(200, ErrorMessage = "Điểm đến không được vượt quá 200 ký tự.")]
    [Display(Name = "Điểm đến")]
    public string DiaDiem { get; set; } = null!;

    [Required(ErrorMessage = "Thời gian không được để trống.")]
    [Range(1, 999, ErrorMessage = "Thời gian phải lớn hơn hoặc bằng 1 ngày.")]
    [Display(Name = "Thời gian (Ngày)")]
    public int ThoiGian { get; set; }

    [Required(ErrorMessage = "Phương tiện chính không được để trống.")]
    [Display(Name = "Phương tiện chính")]
    public string PhuongTienChinh { get; set; } = null!;

    [Display(Name = "Mô tả hành trình")]
    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Giá tham khảo không được để trống.")]
    [Range(1000, 999999999999, ErrorMessage = "Giá tham khảo phải lớn hơn hoặc bằng 1,000 VND.")]
    [Display(Name = "Giá tham khảo")]
    public decimal GiaThamKhao { get; set; }
}
