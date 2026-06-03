using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class LichTrinh
{
    public int MaLichTrinh { get; set; }

    public int MaTour { get; set; }

    public int SoNgay { get; set; }

    public string DiaDiem { get; set; } = null!;

    public string HoatDong { get; set; } = null!;

    public virtual Tour MaTourNavigation { get; set; } = null!;
}
