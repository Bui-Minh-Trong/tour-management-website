using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTourDuLich.ViewModels;

public class DoanDuLichFormVM
{
    public int MaDoan { get; set; }

    [Required(ErrorMessage = "Tên đoàn không được để trống.")]
    [StringLength(150, ErrorMessage = "Tên đoàn không được vượt quá 150 ký tự.")]
    [Display(Name = "Tên Đoàn")]
    public string TenDoan { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn Tour du lịch.")]
    [Display(Name = "Tour Du Lịch")]
    public int MaTour { get; set; }

    [Required(ErrorMessage = "Ngày khởi hành không được để trống.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày Khởi Hành")]
    public DateOnly NgayKhoiHanh { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày Kết Thúc")]
    public DateOnly NgayKetThuc { get; set; }

    [Required(ErrorMessage = "Vui lòng phân công Hướng dẫn viên.")]
    [Display(Name = "Hướng Dẫn Viên")]
    public int MaNvHuongDanVien { get; set; }

    [Required(ErrorMessage = "Số lượng khách tối đa không được để trống.")]
    [Range(1, 1000, ErrorMessage = "Số lượng khách tối đa phải lớn hơn 0.")]
    [Display(Name = "Số Lượng Tối Đa")]
    public int SoLuongToiDa { get; set; }

    [Display(Name = "Số Khách Hiện Tại")]
    public int SoKhachHienTai { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái đoàn.")]
    // MaNvDieuHanh: Tương ứng với OperatorID trong Class Diagram
    // Nhân viên Điều hành phụ trách lập kế hoạch và giám sát đoàn
    [Display(Name = "Nhân Viên Điều Hành")]
    public int? MaNvDieuHanh { get; set; }

    [Display(Name = "Trạng Thái")]
    public string TrangThaiDoan { get; set; } = "Lên kế hoạch";

    // Danh sách chọn cho giao diện Dropdown
    public IEnumerable<SelectListItem>? Tours { get; set; }
    public IEnumerable<SelectListItem>? Guides { get; set; }
    public IEnumerable<SelectListItem>? Operators { get; set; }
}
