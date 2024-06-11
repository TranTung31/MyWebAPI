using MyWebAPI.Data;
using MyWebAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyWebAPI.Services
{
    public class LoaiRepository : ILoaiRepository
    {
        private readonly MyDbContext _context;

        public LoaiRepository(MyDbContext context)
        {
            _context = context;
        }

        public LoaiVM Create(LoaiModel loai)
        {
            var _loai = new Loai
            {
                TenLoai = loai.TenLoai,
            };
            _context.Loai.Add(_loai);
            _context.SaveChanges();
            return new LoaiVM
            {
                MaLoai = _loai.MaLoai,
                TenLoai = _loai.TenLoai,
            };
        }

        public void Delete(int id)
        {
            var loai = _context.Loai.SingleOrDefault(x => x.MaLoai == id);
            if (loai != null)
            {
                _context.Loai.Remove(loai);
                _context.SaveChanges();
            }
        }

        public List<LoaiVM> GetAll()
        {
            var loais = _context.Loai.Select(x => new LoaiVM
            {
                MaLoai = x.MaLoai,
                TenLoai = x.TenLoai,
            });
            return loais.ToList();
        }

        public LoaiVM GetById(int id)
        {
            var loai = _context.Loai.SingleOrDefault(x => x.MaLoai == id);

            if (loai == null)
            {
                return null;
            }

            return new LoaiVM
            {
                MaLoai = loai.MaLoai,
                TenLoai = loai.TenLoai,
            };
        }

        public void Put(LoaiVM loai)
        {
            var _loai = _context.Loai.SingleOrDefault(x => x.MaLoai == loai.MaLoai);
            if (_loai != null)
            {
                _loai.TenLoai = loai.TenLoai;
                _context.SaveChanges();
            }
        }
    }
}
