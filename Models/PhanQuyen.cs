using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class PhanQuyen
{
    public int MaVaiTro { get; set; }

    public string MaChucNang { get; set; } = null!;

    public bool CoTheXem { get; set; }

    public bool CoTheThem { get; set; }

    public bool CoTheSua { get; set; }

    public bool CoTheXoa { get; set; }

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
