using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class LuuTru
{
    public int MaDoan { get; set; }

    public int MaKs { get; set; }

    public DateOnly NgayNhanPhong { get; set; }

    public DateOnly NgayTraPhong { get; set; }

    public int SoPhong { get; set; }

    public virtual DoanDuLich MaDoanNavigation { get; set; } = null!;

    public virtual KhachSan MaKsNavigation { get; set; } = null!;
}
