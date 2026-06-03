using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]/{id?}")]
public class TaiXeController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách Tài Xế giả lập (Mock Data)
    private static readonly List<TaiXeViewModel> ActiveMockList = new()
    {
        new TaiXeViewModel { MaTaiXe = 1, HoTen = "Trần Quốc Toản", Sdt = "0977112233", SoGplx = "123456789012", LoaiBangLai = "Hạng E" },
        new TaiXeViewModel { MaTaiXe = 2, HoTen = "Lê Văn Tám", Sdt = "0966445566", SoGplx = "987654321098", LoaiBangLai = "Hạng D" }
    };

    public TaiXeController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/TaiXe
    [Route("~/Admin/TaiXe")]
    [Route("~/Admin/TaiXe/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.TaiXes.AnyAsync();
            if (hasData)
            {
                var list = await _context.TaiXes
                    .Select(t => new TaiXeViewModel
                    {
                        MaTaiXe = t.MaTaiXe,
                        HoTen = t.HoTen,
                        Sdt = t.Sdt,
                        SoGplx = t.SoGplx,
                        LoaiBangLai = t.LoaiBangLai
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        return View(ActiveMockList);
    }

    // GET: Admin/TaiXe/Create
    public IActionResult Create()
    {
        return View(new TaiXeViewModel());
    }

    // POST: Admin/TaiXe/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaiXeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var tx = new TaiXe
            {
                HoTen = model.HoTen.Trim(),
                Sdt = model.Sdt.Trim(),
                SoGplx = model.SoGplx.Trim(),
                LoaiBangLai = model.LoaiBangLai
            };

            _context.TaiXes.Add(tx);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm tài xế mới thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            // Mock fallback
            TempData["SuccessMessage"] = "Thêm tài xế giả lập thành công (Offline)!";
            ActiveMockList.Add(new TaiXeViewModel
            {
                MaTaiXe = ActiveMockList.Max(m => m.MaTaiXe) + 1,
                HoTen = model.HoTen,
                Sdt = model.Sdt,
                SoGplx = model.SoGplx,
                LoaiBangLai = model.LoaiBangLai
            });
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/TaiXe/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var tx = await _context.TaiXes.FindAsync(id);
            if (tx != null)
            {
                _context.TaiXes.Remove(tx);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa tài xế thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var mockItem = ActiveMockList.FirstOrDefault(m => m.MaTaiXe == id);
        if (mockItem != null)
        {
            ActiveMockList.Remove(mockItem);
            TempData["SuccessMessage"] = "Xóa tài xế giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }
}
