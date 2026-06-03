using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTourDuLich.ViewModels;

public class HoaDonFormVM
{
    public int MaHoaDon { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn Phiếu đặt tour cần thanh toán.")]
    [Display(Name = "Phiếu Đặt Tour")]
    public int MaDangKy { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số tiền thu.")]
    [Range(1000, 999999999999, ErrorMessage = "Số tiền thanh toán phải lớn hơn 1,000 VND.")]
    [Display(Name = "Số Tiền Thu")]
    public decimal SoTien { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn loại hóa đơn.")]
    [Display(Name = "Loại Hóa Đơn")]
    public string LoaiHoaDon { get; set; } = "Toàn bộ";

    [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán.")]
    [Display(Name = "Hình Thức TT")]
    public string HinhThucTt { get; set; } = "Tiền mặt";

    [Required(ErrorMessage = "Vui lòng chọn nhân viên lập phiếu.")]
    [Display(Name = "Nhân Viên Lập Phiếu")]
    public int MaNvLap { get; set; }

    // GhiChu: Tương ứng với Note trong Class Diagram lớp PhieuThu
    [Display(Name = "Ghi Chú")]
    [StringLength(255, ErrorMessage = "Ghi chú không được vượt quá 255 ký tự.")]
    public string? GhiChu { get; set; }

    // Select lists
    public IEnumerable<SelectListItem>? Bookings { get; set; }
    public IEnumerable<SelectListItem>? Staffs { get; set; }
}
