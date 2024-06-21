using System;

namespace MyWebAPI.Models
{
    public class HangHoaOutputModel
    {
        public Guid MaHh { get; set; }
        public string TenHh { get; set; }
        public string MoTa { get; set; }
        public double DonGia { get; set; }
        public byte GiamGia { get; set; }
        public string TenLoai { get; set; }
    }
}
