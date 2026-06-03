using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTourDuLich.ViewModels;

public class ChiPhiDoanFormVM
{
    public int MaChiPhi { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn đoàn du lịch phát sinh chi phí.")]
    [Display(Name = "Đoàn Du Lịch")]
    public int MaDoan { get; set; }

    [Required(ErrorMessage = "Nội dung chi không được để trống.")]
    [StringLength(255, ErrorMessage = "Nội dung chi không được vượt quá 255 ký tự.")]
    [Display(Name = "Nội Dung Chi")]
    public string NoiDungChi { get; set; } = null!;

    // LoaiChiPhi: Tương ứng với ExpenseType trong Class Diagram
    [Required(ErrorMessage = "Vui lòng chọn loại chi phí.")]
    [Display(Name = "Loại Chi Phí")]
    public string LoaiChiPhi { get; set; } = "Khác";

    public static readonly string[] DanhSachLoaiChiPhi = 
    {
        "Khách sạn / Lưu trú",
        "Ăn uống",
        "Vận chuyển",
        "Vé tham quan",
        "Phí hướng dẫn viên",
        "Bảo hiểm",
        "Chi phí khẩn cấp",
        "Khác"
    };

    [Required(ErrorMessage = "Số tiền chi không được để trống.")]
    [Range(1000, 999999999999, ErrorMessage = "Số tiền chi phải từ 1,000 VND trở lên.")]
    [Display(Name = "Số Tiền Chi")]
    public decimal SoTien { get; set; }

    [Required(ErrorMessage = "Ngày lập phiếu chi không được để trống.")]
    [Display(Name = "Ngày Chi")]
    public DateTime NgayChi { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Vui lòng chọn nhân viên thực hiện chi.")]
    [Display(Name = "Người Lập Phiếu")]
    public int MaNvChi { get; set; }

    // Dropdown lists
    public IEnumerable<SelectListItem>? TourGroups { get; set; }
    public IEnumerable<SelectListItem>? Staffs { get; set; }
}
