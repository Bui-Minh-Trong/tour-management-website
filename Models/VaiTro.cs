using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class VaiTro
{
    public int MaVaiTro { get; set; }

    public string TenVaiTro { get; set; } = null!;

    public virtual ICollection<PhanQuyen> PhanQuyens { get; set; } = new List<PhanQuyen>();

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
