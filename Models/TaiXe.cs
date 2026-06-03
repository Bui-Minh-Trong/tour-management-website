using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class TaiXe
{
    public int MaTaiXe { get; set; }

    public string HoTen { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string SoGplx { get; set; } = null!;

    public string LoaiBangLai { get; set; } = null!;

    public virtual ICollection<PhancongVanchuyen> PhancongVanchuyens { get; set; } = new List<PhancongVanchuyen>();
}
