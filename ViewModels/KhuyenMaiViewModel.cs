using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTourDuLich.ViewModels;

public class KhuyenMaiViewModel
{
    public int MaKm { get; set; }

    [Required(ErrorMessage = "Mã khuyến mãi không được để trống.")]
    [StringLength(20, ErrorMessage = "Mã khuyến mãi không được vượt quá 20 ký tự.")]
    [Display(Name = "Mã Code")]
    public string Code { get; set; } = null!;

    [Range(0, 100, ErrorMessage = "% giảm phải từ 0% đến 100%.")]
    [Display(Name = "Phần Trăm Giảm (%)")]
    public decimal? PhanTramGiam { get; set; }

    [Range(0, 999999999999, ErrorMessage = "Tiền giảm trực tiếp phải lớn hơn hoặc bằng 0.")]
    [Display(Name = "Tiền Giảm Trực Tiếp (VND)")]
    public decimal? TienGiamTrucTiep { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày Bắt Đầu")]
    public DateOnly NgayBatDau { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Ngày hết hạn không được để trống.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày Hết Hạn")]
    public DateOnly NgayHetHan { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));

    [Display(Name = "Số Lượng Đã Dùng")]
    public int SoLuongDaDung { get; set; }
}
