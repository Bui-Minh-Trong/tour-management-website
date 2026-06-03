using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly QlyTourDuLichContext _context;

    public AuthController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Auth/Login
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginVM());
    }

    // POST: Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        // BƯỚC 1: Nhận LoginVM. Kiểm tra ModelState.
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // BƯỚC 2: Query DB để lấy thông tin tài khoản kèm theo Vai Trò và Nhân Viên
        var account = await _context.TaiKhoans
            .Include(t => t.MaVaiTroNavigation)
            .Include(t => t.MaNvNavigation)
            .SingleOrDefaultAsync(x => x.TenDangNhap == model.TenDangNhap);

        // BƯỚC 3 (Rẽ nhánh ALT): Kiểm tra tài khoản tồn tại, mật khẩu và trạng thái hoạt động
        if (account == null || !account.TrangThai)
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa.");
            return View(model);
        }

        bool isPasswordValid = false;
        try
        {
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<TaiKhoan>();
            var verificationResult = hasher.VerifyHashedPassword(account, account.MatKhau, model.MatKhau);
            isPasswordValid = verificationResult != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;
        }
        catch (System.FormatException)
        {
            // Mật khẩu lưu trữ trong DB không phải dạng băm hợp lệ (plaintext)
            isPasswordValid = false;
        }

        // Tương thích ngược: Fallback kiểm tra mật khẩu trần cho các tài khoản seed sẵn trong DB
        if (!isPasswordValid && account.MatKhau.Trim() == model.MatKhau.Trim())
        {
            isPasswordValid = true;
        }

        if (!isPasswordValid)
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa.");
            return View(model);
        }

        // BƯỚC 4: Khởi tạo Cookie (Claims). Lưu các thông tin NameIdentifier (TenDangNhap), Name (HoTen), Role (TenVaiTro)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.TenDangNhap),
            new Claim(ClaimTypes.Name, account.MaNvNavigation.HoTen),
            new Claim(ClaimTypes.Role, account.MaVaiTroNavigation.TenVaiTro)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe
        };

        // BƯỚC 5: Gọi HttpContext.SignInAsync để đăng nhập
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity), 
            authProperties
        );

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home", new { area = "Admin" });
    }

    // POST/GET: Auth/Logout
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // BƯỚC 6: Xử lý chức năng Logout (SignOutAsync)
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
