using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTourDuLich.ViewModels;

public class DispatchFormVM
{
    [Required(ErrorMessage = "Vui lòng chọn đoàn du lịch cần điều phối.")]
    [Display(Name = "Đoàn Du Lịch")]
    public int MaDoan { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn hướng dẫn viên.")]
    [Display(Name = "Hướng Dẫn Viên")]
    public int MaNvHuongDanVien { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phương tiện vận chuyển.")]
    [Display(Name = "Phương Tiện")]
    public int MaPhuongTien { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn tài xế lái xe.")]
    [Display(Name = "Tài Xế")]
    public int MaTaiXe { get; set; }

    public List<SelectListItem> DoanDuLiches { get; set; } = new();
    public List<SelectListItem> HuongDanViens { get; set; } = new();
    public List<SelectListItem> PhuongTiens { get; set; } = new();
    public List<SelectListItem> TaiXes { get; set; } = new();
}
