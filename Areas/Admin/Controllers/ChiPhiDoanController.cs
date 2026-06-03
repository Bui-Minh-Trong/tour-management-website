using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]/{id?}")]
[Authorize(Roles = "Quản Trị Viên,Kế Toán Tài Chính,Điều Hành Tour")]
public class ChiPhiDoanController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách Chi Phí giả lập (Mock Data)
    private static readonly List<ChiPhiDoanListVM> MockChiPhiList = new()
    {
        new ChiPhiDoanListVM
        {
            MaChiPhi = 1,
            TenDoan = "Đoàn Du Lịch Phú Quốc Hè 2026",
            LoaiChiPhi = "Khách sạn / Lưu trú",
            NoiDungChi = "Thanh toán phòng khách sạn Sunset Phú Quốc (3 đêm)",
            SoTien = 12500000,
            NgayChi = DateTime.Now.AddDays(-2),
            TenNhanVienLap = "Phạm Thu Hương"
        },
        new ChiPhiDoanListVM
        {
            MaChiPhi = 2,
            TenDoan = "Đoàn Du Lịch Đà Lạt Ngàn Hoa",
            LoaiChiPhi = "Ăn uống",
            NoiDungChi = "Chi phí ăn trưa ngày thứ 2 cho đoàn (30 người)",
            SoTien = 4500000,
            NgayChi = DateTime.Now.AddDays(-1),
            TenNhanVienLap = "Nguyễn Văn Hùng"
        }
    };

    public ChiPhiDoanController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/ChiPhiDoan
    [Route("~/Admin/ChiPhiDoan")]
    [Route("~/Admin/ChiPhiDoan/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.ChiPhiDoans.AnyAsync();
            if (hasData)
            {
                var list = await _context.ChiPhiDoans
                    .Include(c => c.MaDoanNavigation)
                    .Include(c => c.MaNvChiNavigation)
                    .Select(c => new ChiPhiDoanListVM
                    {
                        MaChiPhi = c.MaChiPhi,
                        TenDoan = c.MaDoanNavigation.TenDoan,
                        LoaiChiPhi = c.LoaiChiPhi,
                        NoiDungChi = c.NoiDungChi,
                        SoTien = c.SoTien,
                        NgayChi = c.NgayChi,
                        TenNhanVienLap = c.MaNvChiNavigation.HoTen
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        return View(MockChiPhiList);
    }

    // GET: Admin/ChiPhiDoan/Create
    public async Task<IActionResult> Create()
    {
        var model = new ChiPhiDoanFormVM
        {
            NgayChi = DateTime.Now,
            SoTien = 1000000
        };

        await PopulateSelectLists(model);
        return View(model);
    }

    // POST: Admin/ChiPhiDoan/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ChiPhiDoanFormVM model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectLists(model);
            return View(model);
        }

        try
        {
            var cp = new ChiPhiDoan
            {
                MaDoan = model.MaDoan,
                LoaiChiPhi = model.LoaiChiPhi,
                NoiDungChi = model.NoiDungChi.Trim(),
                SoTien = model.SoTien,
                NgayChi = model.NgayChi,
                MaNvChi = model.MaNvChi
            };

            _context.ChiPhiDoans.Add(cp);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Lập phiếu chi cho đoàn thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            // Mock fallback offline
            TempData["SuccessMessage"] = "Lập phiếu chi giả lập thành công (Offline)!";
            MockChiPhiList.Add(new ChiPhiDoanListVM
            {
                MaChiPhi = MockChiPhiList.Max(m => m.MaChiPhi) + 1,
                TenDoan = "Đoàn du lịch mã #" + model.MaDoan,
                NoiDungChi = model.NoiDungChi,
                SoTien = model.SoTien,
                NgayChi = model.NgayChi,
                TenNhanVienLap = "Nhân viên lập phiếu #" + model.MaNvChi
            });
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/ChiPhiDoan/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var cp = await _context.ChiPhiDoans.FindAsync(id);
            if (cp != null)
            {
                _context.ChiPhiDoans.Remove(cp);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hủy phiếu chi thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var mockItem = MockChiPhiList.FirstOrDefault(m => m.MaChiPhi == id);
        if (mockItem != null)
        {
            MockChiPhiList.Remove(mockItem);
            TempData["SuccessMessage"] = "Hủy phiếu chi giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSelectLists(ChiPhiDoanFormVM model)
    {
        List<SelectListItem> groupsList = new();
        List<SelectListItem> staffList = new();

        try
        {
            var dbDoan = await _context.DoanDuLiches.ToListAsync();
            groupsList = dbDoan.Select(d => new SelectListItem
            {
                Value = d.MaDoan.ToString(),
                Text = d.TenDoan
            }).ToList();

            var dbStaffs = await _context.NhanViens.ToListAsync();
            staffList = dbStaffs.Select(s => new SelectListItem
            {
                Value = s.MaNv.ToString(),
                Text = $"{s.HoTen} ({s.ChucVu})"
            }).ToList();
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        if (!groupsList.Any())
        {
            groupsList.Add(new SelectListItem { Value = "1", Text = "Đoàn Du Lịch Phú Quốc Hè 2026" });
            groupsList.Add(new SelectListItem { Value = "2", Text = "Đoàn Du Lịch Đà Lạt Ngàn Hoa" });
        }

        if (!staffList.Any())
        {
            staffList.Add(new SelectListItem { Value = "1", Text = "Phạm Thu Hương (Kế toán)" });
            staffList.Add(new SelectListItem { Value = "2", Text = "Nguyễn Văn Hùng (HDV)" });
        }

        model.TourGroups = groupsList;
        model.Staffs = staffList;
    }
}
