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
public class KhachHangController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách Khách Hàng giả lập (Mock Data)
    private static readonly List<KhachHangViewModel> MockKhachHangList = new()
    {
        new KhachHangViewModel
        {
            MaKh = 1,
            HoTen = "Nguyễn Văn Anh",
            GioiTinh = "Nam",
            NgaySinh = new DateOnly(1990, 5, 12),
            Sdt = "0987654321",
            Email = "vananh.nguyen@gmail.com",
            Cccd = "079090123456",
            DiaChi = "123 Đường Lê Lợi, Quận 1, TP. HCM"
        },
        new KhachHangViewModel
        {
            MaKh = 2,
            HoTen = "Trần Thị Bé",
            GioiTinh = "Nữ",
            NgaySinh = new DateOnly(1995, 10, 22),
            Sdt = "0912345678",
            Email = "betran95@gmail.com",
            Cccd = "079095654321",
            DiaChi = "456 Đường Nguyễn Trãi, Quận 5, TP. HCM"
        },
        new KhachHangViewModel
        {
            MaKh = 3,
            HoTen = "Phạm Minh Hoàng",
            GioiTinh = "Nam",
            NgaySinh = new DateOnly(1988, 3, 15),
            Sdt = "0905556667",
            Email = "hoangpham@yahoo.com",
            Cccd = "079088111222",
            DiaChi = "789 Đường Điện Biên Phủ, Quận Bình Thạnh, TP. HCM"
        }
    };

    public KhachHangController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/KhachHang
    [Route("~/Admin/KhachHang")]
    [Route("~/Admin/KhachHang/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.KhachHangs.AnyAsync();
            if (hasData)
            {
                var list = await _context.KhachHangs
                    .Select(k => new KhachHangViewModel
                    {
                        MaKh = k.MaKh,
                        HoTen = k.HoTen,
                        GioiTinh = k.GioiTinh,
                        NgaySinh = k.NgaySinh,
                        Sdt = k.Sdt,
                        Email = k.Email,
                        Cccd = k.Cccd,
                        DiaChi = k.DiaChi
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi DB và trả về mock
        }

        return View(MockKhachHangList);
    }

    // GET: Admin/KhachHang/Create
    public IActionResult Create()
    {
        var model = new KhachHangViewModel
        {
            NgaySinh = new DateOnly(1995, 1, 1)
        };
        return View(model);
    }

    // POST: Admin/KhachHang/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhachHangViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Kiểm tra trùng SĐT hoặc CCCD
            bool exists = await _context.KhachHangs.AnyAsync(k => k.Sdt == model.Sdt || (model.Cccd != null && k.Cccd == model.Cccd));
            if (exists)
            {
                ModelState.AddModelError("Sdt", "Số điện thoại hoặc số CCCD này đã tồn tại trong hệ thống.");
                return View(model);
            }

            var kh = new KhachHang
            {
                HoTen = model.HoTen.Trim(),
                GioiTinh = model.GioiTinh,
                NgaySinh = model.NgaySinh,
                Sdt = model.Sdt.Trim(),
                Email = model.Email?.Trim(),
                Cccd = model.Cccd?.Trim(),
                DiaChi = model.DiaChi?.Trim()
            };

            _context.KhachHangs.Add(kh);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            // Lưu vào mock list khi offline
            TempData["SuccessMessage"] = "Thêm khách hàng giả lập thành công (Offline)!";
            MockKhachHangList.Add(new KhachHangViewModel
            {
                MaKh = MockKhachHangList.Max(m => m.MaKh) + 1,
                HoTen = model.HoTen,
                GioiTinh = model.GioiTinh,
                NgaySinh = model.NgaySinh,
                Sdt = model.Sdt,
                Email = model.Email,
                Cccd = model.Cccd,
                DiaChi = model.DiaChi
            });
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Admin/KhachHang/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        KhachHangViewModel? model = null;

        try
        {
            var k = await _context.KhachHangs.FindAsync(id);
            if (k != null)
            {
                model = new KhachHangViewModel
                {
                    MaKh = k.MaKh,
                    HoTen = k.HoTen,
                    GioiTinh = k.GioiTinh,
                    NgaySinh = k.NgaySinh,
                    Sdt = k.Sdt,
                    Email = k.Email,
                    Cccd = k.Cccd,
                    DiaChi = k.DiaChi
                };
            }
        }
        catch
        {
            // Bỏ qua
        }

        if (model == null)
        {
            var mockItem = MockKhachHangList.FirstOrDefault(m => m.MaKh == id);
            if (mockItem == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng mã #" + id;
                return RedirectToAction(nameof(Index));
            }
            model = mockItem;
        }

        return View(model);
    }

    // POST: Admin/KhachHang/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, KhachHangViewModel model)
    {
        if (id != model.MaKh) return BadRequest();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var k = await _context.KhachHangs.FindAsync(id);
            if (k != null)
            {
                k.HoTen = model.HoTen.Trim();
                k.GioiTinh = model.GioiTinh;
                k.NgaySinh = model.NgaySinh;
                k.Sdt = model.Sdt.Trim();
                k.Email = model.Email?.Trim();
                k.Cccd = model.Cccd?.Trim();
                k.DiaChi = model.DiaChi?.Trim();

                _context.Entry(k).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        var index = MockKhachHangList.FindIndex(m => m.MaKh == id);
        if (index >= 0)
        {
            MockKhachHangList[index] = model;
            TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/KhachHang/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var k = await _context.KhachHangs.FindAsync(id);
            if (k != null)
            {
                // Kiểm tra xem khách đã đặt tour chưa
                bool hasBookings = await _context.KhachDiTours.AnyAsync(b => b.MaKh == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Không thể xóa khách hàng này vì đã có lịch sử đặt tour.";
                    return RedirectToAction(nameof(Index));
                }

                _context.KhachHangs.Remove(k);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa thông tin khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var mockItem = MockKhachHangList.FirstOrDefault(m => m.MaKh == id);
        if (mockItem != null)
        {
            MockKhachHangList.Remove(mockItem);
            TempData["SuccessMessage"] = "Xóa thông tin khách hàng giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }
}
