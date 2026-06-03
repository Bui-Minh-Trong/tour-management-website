using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTourDuLich.ViewModels;

public class BookingFormVM
{
    public int MaDangKy { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn khách hàng đăng ký.")]
    [Display(Name = "Khách Hàng")]
    public int MaKh { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn đoàn du lịch.")]
    [Display(Name = "Đoàn Du Lịch")]
    public int MaDoan { get; set; }

    [Display(Name = "Mã Khuyến Mãi (Nếu có)")]
    [StringLength(20, ErrorMessage = "Mã khuyến mãi không được vượt quá 20 ký tự.")]
    public string? CodeKhuyenMai { get; set; }

    [Required(ErrorMessage = "Số người đi không được để trống.")]
    [Range(1, 100, ErrorMessage = "Số người đi phải từ 1 đến 100 người.")]
    [Display(Name = "Số Người Đi")]
    public int SoNguoiDi { get; set; } = 1;

    [Required(ErrorMessage = "Đơn giá không được để trống.")]
    [Range(1000, 999999999999, ErrorMessage = "Đơn giá phải từ 1,000 VND trở lên.")]
    [Display(Name = "Đơn Giá")]
    public decimal DonGia { get; set; }

    [Display(Name = "Số Tiền Giảm")]
    public decimal SoTienGiam { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái thanh toán.")]
    [Display(Name = "Trạng Thái Thanh Toán")]
    public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán";

    [Display(Name = "Ghi chú")]
    [StringLength(255, ErrorMessage = "Ghi chú không được vượt quá 255 ký tự.")]
    public string? GhiChu { get; set; }

    // MaNvKinhDoanh: Tương ứng với SaleID trong Class Diagram
    // Nhân viên kinh doanh lập phiếu đặt tour cho khách hàng
    [Display(Name = "Nhân Viên Kinh Doanh")]
    public int? MaNvKinhDoanh { get; set; }

    // Select lists
    public IEnumerable<SelectListItem>? Customers { get; set; }
    public IEnumerable<SelectListItem>? TourGroups { get; set; }
    public IEnumerable<SelectListItem>? SaleStaffs { get; set; }
}
