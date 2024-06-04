using System;
using System.Collections.Generic;

namespace MyWebAPI.Data
{
    public enum TinhTrangDonHang
    {
        New = 0, Payment = 1, Complete = 2, Cancel = 0
    }
    public class DonHang
    {
        public Guid MaDh { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime? NgayGiao { get; set; }
        public TinhTrangDonHang TinhTrangDonHang { get; set; }
        public string NguoiNhan { get; set; }
        public string DiaChiGiao { get; set; }
        public string SoDienThoai { get; set; }
        public ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }
        
        public DonHang()
        {
            // Trường hợp DonHangChiTiets null thì khởi tạo list rỗng
            DonHangChiTiets = new List<DonHangChiTiet>(); 
        }
    }
}
