using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class Tour
{
    public int MaTour { get; set; }

    public string TenTour { get; set; } = null!;

    public string DiaDiem { get; set; } = null!;

    public int ThoiGian { get; set; }

    public string PhuongTienChinh { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal GiaThamKhao { get; set; }

    public virtual ICollection<DoanDuLich> DoanDuLiches { get; set; } = new List<DoanDuLich>();

    public virtual ICollection<LichTrinh> LichTrinhs { get; set; } = new List<LichTrinh>();
}
