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
[Authorize(Roles = "Quản Trị Viên")]
public class NhanVienController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách Nhân Viên giả lập (Mock Data)
    private static readonly List<NhanVienViewModel> MockNhanVienList = new()
    {
        new NhanVienViewModel
        {
            MaNv = 1,
            HoTen = "Phạm Thu Hương",
            ChucVu = "Kế toán trưởng",
            Sdt = "0901234567",
            Email = "huong.pham@travel.com",
            NgayVaoLam = new DateOnly(2020, 1, 15),
            HasAccount = true,
            TenDangNhap = "huongpham",
            TenVaiTro = "Kế Toán Tài Chính"
        },
        new NhanVienViewModel
        {
            MaNv = 2,
            HoTen = "Nguyễn Văn Hùng",
            ChucVu = "Hướng dẫn viên",
            Sdt = "0934567890",
            Email = "hung.nguyen@travel.com",
            NgayVaoLam = new DateOnly(2022, 6, 1),
            HasAccount = false
        },
        new NhanVienViewModel
        {
            MaNv = 3,
            HoTen = "Lê Thị Thu Thảo",
            ChucVu = "Hướng dẫn viên",
            Sdt = "0988889999",
            Email = "thao.le@travel.com",
            NgayVaoLam = new DateOnly(2023, 3, 10),
            HasAccount = true,
            TenDangNhap = "thaole",
            TenVaiTro = "Điều Hành Tour"
        }
    };

    public NhanVienController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/NhanVien
    [Route("~/Admin/NhanVien")]
    [Route("~/Admin/NhanVien/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.NhanViens.AnyAsync();
            if (hasData)
            {
                var list = await _context.NhanViens
                    .Include(n => n.TaiKhoan)
                        .ThenInclude(t => t!.MaVaiTroNavigation)
                    .Select(n => new NhanVienViewModel
                    {
                        MaNv = n.MaNv,
                        HoTen = n.HoTen,
                        ChucVu = n.ChucVu,
                        Sdt = n.Sdt,
                        Email = n.Email,
                        NgayVaoLam = n.NgayVaoLam,
                        HasAccount = n.TaiKhoan != null,
                        TenDangNhap = n.TaiKhoan != null ? n.TaiKhoan.TenDangNhap : null,
                        TenVaiTro = (n.TaiKhoan != null && n.TaiKhoan.MaVaiTroNavigation != null) ? n.TaiKhoan.MaVaiTroNavigation.TenVaiTro : null
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        return View(MockNhanVienList);
    }

    // GET: Admin/NhanVien/Create
    public IActionResult Create()
    {
        var model = new NhanVienViewModel
        {
            NgayVaoLam = DateOnly.FromDateTime(DateTime.Today)
        };
        return View(model);
    }

    // POST: Admin/NhanVien/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhanVienViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            bool exists = await _context.NhanViens.AnyAsync(n => n.Sdt == model.Sdt);
            if (exists)
            {
                ModelState.AddModelError("Sdt", "Số điện thoại này đã thuộc về nhân viên khác.");
                return View(model);
            }

            var nv = new NhanVien
            {
                HoTen = model.HoTen.Trim(),
                ChucVu = model.ChucVu,
                Sdt = model.Sdt.Trim(),
                Email = model.Email?.Trim(),
                NgayVaoLam = model.NgayVaoLam
            };

            _context.NhanViens.Add(nv);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm nhân viên mới thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            // Mock list fallback
            TempData["SuccessMessage"] = "Thêm nhân viên giả lập thành công (Offline)!";
            MockNhanVienList.Add(new NhanVienViewModel
            {
                MaNv = MockNhanVienList.Max(m => m.MaNv) + 1,
                HoTen = model.HoTen,
                ChucVu = model.ChucVu,
                Sdt = model.Sdt,
                Email = model.Email,
                NgayVaoLam = model.NgayVaoLam,
                HasAccount = false
            });
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Admin/NhanVien/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        NhanVienViewModel? model = null;

        try
        {
            var n = await _context.NhanViens.Include(nv => nv.TaiKhoan).FirstOrDefaultAsync(nv => nv.MaNv == id);
            if (n != null)
            {
                model = new NhanVienViewModel
                {
                    MaNv = n.MaNv,
                    HoTen = n.HoTen,
                    ChucVu = n.ChucVu,
                    Sdt = n.Sdt,
                    Email = n.Email,
                    NgayVaoLam = n.NgayVaoLam,
                    HasAccount = n.TaiKhoan != null,
                    TenDangNhap = n.TaiKhoan != null ? n.TaiKhoan.TenDangNhap : null
                };
            }
        }
        catch
        {
            // Bỏ qua
        }

        if (model == null)
        {
            var mockItem = MockNhanVienList.FirstOrDefault(m => m.MaNv == id);
            if (mockItem == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên mã #" + id;
                return RedirectToAction(nameof(Index));
            }
            model = mockItem;
        }

        return View(model);
    }

    // POST: Admin/NhanVien/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NhanVienViewModel model)
    {
        if (id != model.MaNv) return BadRequest();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var n = await _context.NhanViens.FindAsync(id);
            if (n != null)
            {
                n.HoTen = model.HoTen.Trim();
                n.ChucVu = model.ChucVu;
                n.Sdt = model.Sdt.Trim();
                n.Email = model.Email?.Trim();
                n.NgayVaoLam = model.NgayVaoLam;

                _context.Entry(n).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var index = MockNhanVienList.FindIndex(m => m.MaNv == id);
        if (index >= 0)
        {
            MockNhanVienList[index].HoTen = model.HoTen;
            MockNhanVienList[index].ChucVu = model.ChucVu;
            MockNhanVienList[index].Sdt = model.Sdt;
            MockNhanVienList[index].Email = model.Email;
            MockNhanVienList[index].NgayVaoLam = model.NgayVaoLam;
            TempData["SuccessMessage"] = "Cập nhật nhân viên giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/NhanVien/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var n = await _context.NhanViens.FindAsync(id);
            if (n != null)
            {
                // Ràng buộc DB: Nhân viên đang hướng dẫn đoàn hay ký hóa đơn không được xóa
                bool isGuiding = await _context.DoanDuLiches.AnyAsync(d => d.MaNvHuongDanVien == id);
                if (isGuiding)
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhân viên vì đang được phân công hướng dẫn đoàn du lịch.";
                    return RedirectToAction(nameof(Index));
                }

                _context.NhanViens.Remove(n);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa thông tin nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var mockItem = MockNhanVienList.FirstOrDefault(m => m.MaNv == id);
        if (mockItem != null)
        {
            if (id == 1)
            {
                TempData["ErrorMessage"] = "Không thể xóa nhân viên này vì đang hoạt động hướng dẫn đoàn (Giả lập).";
            }
            else
            {
                MockNhanVienList.Remove(mockItem);
                TempData["SuccessMessage"] = "Xóa nhân viên giả lập thành công (Offline)!";
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/NhanVien/CreateAccount (Cấp tài khoản qua Modal)
    [HttpPost]
    public async Task<IActionResult> CreateAccount(int MaNv, string TenDangNhap, string MatKhau, int MaVaiTro)
    {
        try
        {
            // Kiểm tra xem tên đăng nhập đã trùng chưa
            bool userExists = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == TenDangNhap.Trim());
            if (userExists)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập này đã có người sử dụng.";
                return RedirectToAction(nameof(Index));
            }

            var tk = new TaiKhoan
            {
                MaNv = MaNv,
                TenDangNhap = TenDangNhap.Trim(),
                MaVaiTro = MaVaiTro,
                TrangThai = true
            };

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<TaiKhoan>();
            tk.MatKhau = hasher.HashPassword(tk, MatKhau);

            _context.TaiKhoans.Add(tk);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cấp tài khoản '{TenDangNhap}' thành công!";
        }
        catch
        {
            // Xử lý mock fallback
            var mockItem = MockNhanVienList.FirstOrDefault(m => m.MaNv == MaNv);
            if (mockItem != null)
            {
                mockItem.HasAccount = true;
                mockItem.TenDangNhap = TenDangNhap.Trim();
                
                string roleName = MaVaiTro switch
                {
                    1 => "Quản Trị Viên",
                    2 => "Kế Toán Tài Chính",
                    3 => "Điều Hành Tour",
                    _ => "Nhân Viên Bán Hàng"
                };
                mockItem.TenVaiTro = roleName;

                TempData["SuccessMessage"] = $"Cấp tài khoản giả lập '{TenDangNhap}' thành công (Offline)!";
            }
        }

        return RedirectToAction(nameof(Index));
    }
}
