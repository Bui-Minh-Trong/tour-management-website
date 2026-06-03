using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyTourDuLich.Models;

public partial class QlyTourDuLichContext : DbContext
{
    public QlyTourDuLichContext()
    {
    }

    public QlyTourDuLichContext(DbContextOptions<QlyTourDuLichContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiPhiDoan> ChiPhiDoans { get; set; }

    public virtual DbSet<ChiTietKhachDiTour> ChiTietKhachDiTours { get; set; }

    public virtual DbSet<DoanDuLich> DoanDuLiches { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachDiTour> KhachDiTours { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhachSan> KhachSans { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LichTrinh> LichTrinhs { get; set; }

    public virtual DbSet<LuuTru> LuuTrus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhanQuyen> PhanQuyens { get; set; }

    public virtual DbSet<PhancongVanchuyen> PhancongVanchuyens { get; set; }

    public virtual DbSet<PhuongTien> PhuongTiens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TaiXe> TaiXes { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiPhiDoan>(entity =>
        {
            entity.HasKey(e => e.MaChiPhi).HasName("PK__CHI_PHI___516FCAD4D4D87551");

            entity.ToTable("CHI_PHI_DOAN");

            entity.Property(e => e.MaNvChi).HasColumnName("MaNV_Chi");
            entity.Property(e => e.NgayChi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDungChi).HasMaxLength(255);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDoanNavigation).WithMany(p => p.ChiPhiDoans)
                .HasForeignKey(d => d.MaDoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_PHI_D__MaDoa__22751F6C");

            entity.HasOne(d => d.MaNvChiNavigation).WithMany(p => p.ChiPhiDoans)
                .HasForeignKey(d => d.MaNvChi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_PHI_D__MaNV___25518C17");
        });

        modelBuilder.Entity<ChiTietKhachDiTour>(entity =>
        {
            entity.HasKey(e => new { e.MaDangKy, e.MaKh }).HasName("PK__CHI_TIET__48E2ACDC4B3058C9");

            entity.ToTable("CHI_TIET_KHACH_DI_TOUR");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");

            entity.HasOne(d => d.MaDangKyNavigation).WithMany(p => p.ChiTietKhachDiTours)
                .HasForeignKey(d => d.MaDangKy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_TIET___MaDan__151B244E");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.ChiTietKhachDiTours)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHI_TIET_K__MaKH__160F4887");
        });

        modelBuilder.Entity<DoanDuLich>(entity =>
        {
            entity.HasKey(e => e.MaDoan).HasName("PK__DOAN_DU___2DC20C5FA53EC789");

            entity.ToTable("DOAN_DU_LICH");

            entity.Property(e => e.MaNvHuongDanVien).HasColumnName("MaNV_HuongDanVien");
            entity.Property(e => e.TenDoan).HasMaxLength(150);
            entity.Property(e => e.TrangThaiDoan)
                .HasMaxLength(50)
                .HasDefaultValue("Lên kế hoạch");

            entity.HasOne(d => d.MaNvHuongDanVienNavigation).WithMany(p => p.DoanDuLiches)
                .HasForeignKey(d => d.MaNvHuongDanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DOAN_DU_L__MaNV___73BA3083");

            entity.HasOne(d => d.MaTourNavigation).WithMany(p => p.DoanDuLiches)
                .HasForeignKey(d => d.MaTour)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DOAN_DU_L__MaTou__72C60C4A");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HOA_DON__835ED13B13BF0F01");

            entity.ToTable("HOA_DON");

            entity.Property(e => e.HinhThucTt)
                .HasMaxLength(50)
                .HasColumnName("HinhThucTT");
            entity.Property(e => e.LoaiHoaDon)
                .HasMaxLength(50)
                .HasDefaultValue("Toàn bộ");
            entity.Property(e => e.MaNvLap).HasColumnName("MaNV_Lap");
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDangKyNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDangKy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOA_DON__MaDangK__19DFD96B");

            entity.HasOne(d => d.MaNvLapNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNvLap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HOA_DON__MaNV_La__1F98B2C1");
        });

        modelBuilder.Entity<KhachDiTour>(entity =>
        {
            entity.HasKey(e => e.MaDangKy).HasName("PK__KHACH_DI__BA90F02D825A5194");

            entity.ToTable("KHACH_DI_TOUR");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.NgayDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoNguoiDi).HasDefaultValue(1);
            entity.Property(e => e.SoTienGiam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa thanh toán");

            entity.HasOne(d => d.MaDoanNavigation).WithMany(p => p.KhachDiTours)
                .HasForeignKey(d => d.MaDoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KHACH_DI___MaDoa__08B54D69");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.KhachDiTours)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KHACH_DI_T__MaKH__09A971A2");

            entity.HasOne(d => d.MaKmNavigation).WithMany(p => p.KhachDiTours)
                .HasForeignKey(d => d.MaKm)
                .HasConstraintName("FK__KHACH_DI_T__MaKM__0A9D95DB");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KHACH_HA__2725CF1ED0734E99");

            entity.ToTable("KHACH_HANG");

            entity.HasIndex(e => e.Cccd, "UQ__KHACH_HA__A955A0AA7B3A652B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__KHACH_HA__A9D105344B495A47").IsUnique();

            entity.HasIndex(e => e.Sdt, "UQ__KHACH_HA__CA1930A54A5D7471").IsUnique();

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<KhachSan>(entity =>
        {
            entity.HasKey(e => e.MaKs).HasName("PK__KHACH_SA__2725CF139313BDC5");

            entity.ToTable("KHACH_SAN");

            entity.Property(e => e.MaKs).HasColumnName("MaKS");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenKs)
                .HasMaxLength(100)
                .HasColumnName("TenKS");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__KHUYEN_M__2725CF154560438D");

            entity.ToTable("KHUYEN_MAI");

            entity.HasIndex(e => e.Code, "UQ__KHUYEN_M__A25C5AA744243957").IsUnique();

            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PhanTramGiam).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SoLuongDaDung).HasDefaultValue(0);
            entity.Property(e => e.TienGiamTrucTiep)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<LichTrinh>(entity =>
        {
            entity.HasKey(e => e.MaLichTrinh).HasName("PK__LICH_TRI__32E7201D9D8D8EC7");

            entity.ToTable("LICH_TRINH");

            entity.Property(e => e.DiaDiem).HasMaxLength(100);

            entity.HasOne(d => d.MaTourNavigation).WithMany(p => p.LichTrinhs)
                .HasForeignKey(d => d.MaTour)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LICH_TRIN__MaTou__60A75C0F");
        });

        modelBuilder.Entity<LuuTru>(entity =>
        {
            entity.HasKey(e => new { e.MaDoan, e.MaKs }).HasName("PK__LUU_TRU__0FB050AE8CCD0A47");

            entity.ToTable("LUU_TRU");

            entity.Property(e => e.MaKs).HasColumnName("MaKS");
            entity.Property(e => e.SoPhong).HasDefaultValue(1);

            entity.HasOne(d => d.MaDoanNavigation).WithMany(p => p.LuuTrus)
                .HasForeignKey(d => d.MaDoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LUU_TRU__MaDoan__03F0984C");

            entity.HasOne(d => d.MaKsNavigation).WithMany(p => p.LuuTrus)
                .HasForeignKey(d => d.MaKs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LUU_TRU__MaKS__04E4BC85");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NHAN_VIE__2725D70AEA700280");

            entity.ToTable("NHAN_VIEN");

            entity.HasIndex(e => e.Email, "UQ__NHAN_VIE__A9D10534134E9EB4").IsUnique();

            entity.HasIndex(e => e.Sdt, "UQ__NHAN_VIE__CA1930A51B9AB454").IsUnique();

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<PhanQuyen>(entity =>
        {
            entity.HasKey(e => new { e.MaVaiTro, e.MaChucNang }).HasName("PK__PHAN_QUY__A96A9DEAFBF6481E");

            entity.ToTable("PHAN_QUYEN");

            entity.Property(e => e.MaChucNang)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.PhanQuyens)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK__PHAN_QUYE__MaVai__3E52440B");
        });

        modelBuilder.Entity<PhancongVanchuyen>(entity =>
        {
            entity.HasKey(e => new { e.MaDoan, e.MaPhuongTien }).HasName("PK__PHANCONG__3E9960D4EA841A9A");

            entity.ToTable("PHANCONG_VANCHUYEN");

            entity.HasOne(d => d.MaDoanNavigation).WithMany(p => p.PhancongVanchuyens)
                .HasForeignKey(d => d.MaDoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHANCONG___MaDoa__7D439ABD");

            entity.HasOne(d => d.MaPhuongTienNavigation).WithMany(p => p.PhancongVanchuyens)
                .HasForeignKey(d => d.MaPhuongTien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHANCONG___MaPhu__7E37BEF6");

            entity.HasOne(d => d.MaTaiXeNavigation).WithMany(p => p.PhancongVanchuyens)
                .HasForeignKey(d => d.MaTaiXe)
                .HasConstraintName("FK__PHANCONG___MaTai__7F2BE32F");
        });

        modelBuilder.Entity<PhuongTien>(entity =>
        {
            entity.HasKey(e => e.MaPhuongTien).HasName("PK__PHUONG_T__35B6C8B03261BB2D");

            entity.ToTable("PHUONG_TIEN");

            entity.HasIndex(e => e.BienSoXe, "UQ__PHUONG_T__A78059920EDE10AC").IsUnique();

            entity.Property(e => e.BienSoXe)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.LoaiXe).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Hoạt động");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TenDangNhap).HasName("PK__TAI_KHOA__55F68FC1C78DA8E9");

            entity.ToTable("TAI_KHOAN");

            entity.HasIndex(e => e.MaNv, "UQ__TAI_KHOA__2725D70B949685EF").IsUnique();

            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaNvNavigation).WithOne(p => p.TaiKhoan)
                .HasForeignKey<TaiKhoan>(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAI_KHOAN__MaNV__47DBAE45");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAI_KHOAN__MaVai__48CFD27E");
        });

        modelBuilder.Entity<TaiXe>(entity =>
        {
            entity.HasKey(e => e.MaTaiXe).HasName("PK__TAI_XE__FA9D79BEE00D9161");

            entity.ToTable("TAI_XE");

            entity.HasIndex(e => e.Sdt, "UQ__TAI_XE__CA1930A5CBBE8CED").IsUnique();

            entity.HasIndex(e => e.SoGplx, "UQ__TAI_XE__DB93E0124733B290").IsUnique();

            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.LoaiBangLai).HasMaxLength(20);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.SoGplx)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("SoGPLX");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.MaTour).HasName("PK__TOUR__4E5557DE9BD38908");

            entity.ToTable("TOUR");

            entity.Property(e => e.DiaDiem).HasMaxLength(200);
            entity.Property(e => e.GiaThamKhao).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PhuongTienChinh).HasMaxLength(50);
            entity.Property(e => e.TenTour).HasMaxLength(100);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VAI_TRO__C24C41CFAE6AE495");

            entity.ToTable("VAI_TRO");

            entity.HasIndex(e => e.TenVaiTro, "UQ__VAI_TRO__1DA5581410EA5E3D").IsUnique();

            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
