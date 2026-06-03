using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class KhachSan
{
    public int MaKs { get; set; }

    public string TenKs { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public int SoSao { get; set; }

    public string Sdt { get; set; } = null!;

    public virtual ICollection<LuuTru> LuuTrus { get; set; } = new List<LuuTru>();
}
