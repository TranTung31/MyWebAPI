using MyWebAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyWebAPI.Services
{
    public class LoaiRepositoryInMemory : ILoaiRepository
    {
        static List<LoaiVM> lstLoai = new List<LoaiVM>
        {
            new LoaiVM{ MaLoai = 1, TenLoai = "TV" },
            new LoaiVM{ MaLoai = 2, TenLoai = "Laptop" },
            new LoaiVM{ MaLoai = 3, TenLoai = "Điện thoại" },
        };
        public LoaiVM Create(LoaiModel loai)
        {
            var newloai = new LoaiVM
            {
                MaLoai = lstLoai.Max(x => x.MaLoai) + 1,
                TenLoai = loai.TenLoai
            };
            lstLoai.Add(newloai);
            return newloai;
        }

        public void Delete(int id)
        {
            var loai = lstLoai.FirstOrDefault(x => x.MaLoai == id);
            if (loai != null)
            {
                lstLoai.Remove(loai);
            }
        }

        public List<LoaiVM> GetAll()
        {
            return lstLoai;
        }

        public LoaiVM GetById(int id)
        {
            var loai = lstLoai.FirstOrDefault(x => x.MaLoai == id);
            return loai;
        }

        public void Put(LoaiVM loai)
        {
            var updateLoai = lstLoai.FirstOrDefault(x => x.MaLoai == loai.MaLoai);
            if (updateLoai != null)
            {
                updateLoai.TenLoai = loai.TenLoai;
            }
        }
    }
}
