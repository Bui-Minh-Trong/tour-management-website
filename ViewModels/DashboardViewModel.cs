using System.Collections.Generic;

namespace QuanLyTourDuLich.ViewModels;

public class DashboardViewModel
{
    public int TongSoTour { get; set; }
    public decimal TongDoanhThu { get; set; }
    public int TongLuotKhach { get; set; }
    public int TongSoDoan { get; set; }
    public List<TourStatVM> TopTours { get; set; } = new();
}

public class TourStatVM
{
    public string TenTour { get; set; } = string.Empty;
    public int SoKhachDat { get; set; }
}
