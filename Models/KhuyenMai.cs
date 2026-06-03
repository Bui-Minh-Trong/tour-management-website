using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class KhuyenMai
{
    public int MaKm { get; set; }

    public string Code { get; set; } = null!;

    public decimal? PhanTramGiam { get; set; }

    public decimal? TienGiamTrucTiep { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayHetHan { get; set; }

    public int? SoLuongDaDung { get; set; }

    public virtual ICollection<KhachDiTour> KhachDiTours { get; set; } = new List<KhachDiTour>();
}
