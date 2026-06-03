using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class PhancongVanchuyen
{
    public int MaDoan { get; set; }

    public int MaPhuongTien { get; set; }

    public int? MaTaiXe { get; set; }

    public virtual DoanDuLich MaDoanNavigation { get; set; } = null!;

    public virtual PhuongTien MaPhuongTienNavigation { get; set; } = null!;

    public virtual TaiXe? MaTaiXeNavigation { get; set; }
}
