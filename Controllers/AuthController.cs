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

    // GET: Auth/ChangePassword
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordVM());
    }

    // POST: Auth/ChangePassword
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction(nameof(Login));
        }

        var account = await _context.TaiKhoans.SingleOrDefaultAsync(x => x.TenDangNhap == username);
        if (account == null)
        {
            ModelState.AddModelError("", "Không tìm thấy tài khoản người dùng.");
            return View(model);
        }

        // Xác thực mật khẩu cũ
        bool isOldPasswordValid = false;
        try
        {
            var hasher = new PasswordHasher<TaiKhoan>();
            var verificationResult = hasher.VerifyHashedPassword(account, account.MatKhau, model.CurrentPassword);
            isOldPasswordValid = verificationResult != PasswordVerificationResult.Failed;
        }
        catch (System.FormatException)
        {
            isOldPasswordValid = false;
        }

        // Tương thích ngược mật khẩu trần
        if (!isOldPasswordValid && account.MatKhau.Trim() == model.CurrentPassword.Trim())
        {
            isOldPasswordValid = true;
        }

        if (!isOldPasswordValid)
        {
            ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không chính xác.");
            return View(model);
        }

        // Cập nhật mật khẩu mới bằng cách băm
        var passwordHasher = new PasswordHasher<TaiKhoan>();
        account.MatKhau = passwordHasher.HashPassword(account, model.NewPassword);

        _context.Entry(account).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
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
