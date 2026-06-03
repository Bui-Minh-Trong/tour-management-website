using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class PhuongTien
{
    public int MaPhuongTien { get; set; }

    public string BienSoXe { get; set; } = null!;

    public string LoaiXe { get; set; } = null!;

    public int? SoChoNgoi { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<PhancongVanchuyen> PhancongVanchuyens { get; set; } = new List<PhancongVanchuyen>();
}
