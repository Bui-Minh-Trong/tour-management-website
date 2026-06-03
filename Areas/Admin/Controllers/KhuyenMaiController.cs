using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]/{id?}")]
[Authorize(Roles = "Quản Trị Viên,Điều Hành Tour,Kế Toán Tài Chính")]
public class KhuyenMaiController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách Khuyến Mãi giả lập (Mock Data)
    private static readonly List<KhuyenMaiViewModel> MockKhuyenMaiList = new()
    {
        new KhuyenMaiViewModel
        {
            MaKm = 1,
            Code = "HE2026",
            PhanTramGiam = 10,
            TienGiamTrucTiep = 0,
            NgayBatDau = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            NgayHetHan = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            SoLuongDaDung = 25
        },
        new KhuyenMaiViewModel
        {
            MaKm = 2,
            Code = "PHUQUOC500",
            PhanTramGiam = 0,
            TienGiamTrucTiep = 500000,
            NgayBatDau = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            NgayHetHan = DateOnly.FromDateTime(DateTime.Today.AddDays(20)),
            SoLuongDaDung = 12
        },
        new KhuyenMaiViewModel
        {
            MaKm = 3,
            Code = "SALEOFF",
            PhanTramGiam = 15,
            TienGiamTrucTiep = 0,
            NgayBatDau = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            NgayHetHan = DateOnly.FromDateTime(DateTime.Today.AddDays(45)),
            SoLuongDaDung = 0
        }
    };

    public KhuyenMaiController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/KhuyenMai
    [Route("~/Admin/KhuyenMai")]
    [Route("~/Admin/KhuyenMai/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.KhuyenMais.AnyAsync();
            if (hasData)
            {
                var list = await _context.KhuyenMais
                    .Select(k => new KhuyenMaiViewModel
                    {
                        MaKm = k.MaKm,
                        Code = k.Code,
                        PhanTramGiam = k.PhanTramGiam,
                        TienGiamTrucTiep = k.TienGiamTrucTiep,
                        NgayBatDau = k.NgayBatDau,
                        NgayHetHan = k.NgayHetHan,
                        SoLuongDaDung = k.SoLuongDaDung ?? 0
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        return View(MockKhuyenMaiList);
    }

    // GET: Admin/KhuyenMai/Create
    public IActionResult Create()
    {
        var model = new KhuyenMaiViewModel
        {
            NgayBatDau = DateOnly.FromDateTime(DateTime.Today),
            NgayHetHan = DateOnly.FromDateTime(DateTime.Today.AddMonths(1))
        };
        return View(model);
    }

    // POST: Admin/KhuyenMai/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhuyenMaiViewModel model)
    {
        if (model.NgayHetHan < model.NgayBatDau)
        {
            ModelState.AddModelError("NgayHetHan", "Ngày hết hạn phải lớn hơn hoặc bằng ngày bắt đầu.");
        }

        if (model.PhanTramGiam == null && model.TienGiamTrucTiep == null)
        {
            ModelState.AddModelError("", "Vui lòng nhập phần trăm giảm hoặc số tiền giảm trực tiếp.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            bool exists = await _context.KhuyenMais.AnyAsync(k => k.Code == model.Code.Trim());
            if (exists)
            {
                ModelState.AddModelError("Code", "Mã code này đã được tạo trước đó.");
                return View(model);
            }

            var km = new KhuyenMai
            {
                Code = model.Code.Trim().ToUpper(),
                PhanTramGiam = model.PhanTramGiam,
                TienGiamTrucTiep = model.TienGiamTrucTiep,
                NgayBatDau = model.NgayBatDau,
                NgayHetHan = model.NgayHetHan,
                SoLuongDaDung = 0
            };

            _context.KhuyenMais.Add(km);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo mã khuyến mãi thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            // Mock fallback offline
            TempData["SuccessMessage"] = "Tạo mã khuyến mãi giả lập thành công (Offline)!";
            MockKhuyenMaiList.Add(new KhuyenMaiViewModel
            {
                MaKm = MockKhuyenMaiList.Max(m => m.MaKm) + 1,
                Code = model.Code.Trim().ToUpper(),
                PhanTramGiam = model.PhanTramGiam,
                TienGiamTrucTiep = model.TienGiamTrucTiep,
                NgayBatDau = model.NgayBatDau,
                NgayHetHan = model.NgayHetHan,
                SoLuongDaDung = 0
            });
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/KhuyenMai/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var km = await _context.KhuyenMais.FindAsync(id);
            if (km != null)
            {
                // Ràng buộc: Mã đã dùng cho Booking không được xóa
                bool hasBookings = await _context.KhachDiTours.AnyAsync(b => b.MaKm == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Không thể xóa mã vì đã được áp dụng trong phiếu đăng ký của khách hàng.";
                    return RedirectToAction(nameof(Index));
                }

                _context.KhuyenMais.Remove(km);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa mã khuyến mãi thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var mockItem = MockKhuyenMaiList.FirstOrDefault(m => m.MaKm == id);
        if (mockItem != null)
        {
            if (mockItem.SoLuongDaDung > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa mã vì đã có khách hàng sử dụng.";
            }
            else
            {
                MockKhuyenMaiList.Remove(mockItem);
                TempData["SuccessMessage"] = "Xóa mã giả lập thành công (Offline)!";
            }
        }

        return RedirectToAction(nameof(Index));
    }
}
