using System;

namespace MyWebAPI.Models
{
    public class HangHoaInputModel
    {
        public string TenHangHoa { get; set; }
        public string MoTa { get; set; }
        public double DonGia { get; set; }
        public byte GiamGia { get; set; }
        public int? MaLoai { get; set; }
    }
}
