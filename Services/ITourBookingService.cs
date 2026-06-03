using System;
using System.Threading.Tasks;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Services;

public interface ITourBookingService
{
    Task<bool> IsTourNameDuplicateAsync(string name, int? excludeTourId = null);
    Task<bool> IsGuideOverlappedAsync(int guideId, DateOnly start, DateOnly end, int? excludeGroupId = null);
    Task<(bool isSuccess, string message)> CreateBookingAsync(BookingFormVM model);
    Task<(bool isSuccess, string message)> CreateInvoiceAsync(HoaDonFormVM model);
    Task<DashboardViewModel> GetDashboardStatsAsync();
}
