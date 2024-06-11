using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Models
{
    // Định nghĩa class có các thuộc tính để trả về client
    public class LoaiVM
    {
        public int MaLoai { get; set; }
        public string TenLoai { get; set; }
    }
}
