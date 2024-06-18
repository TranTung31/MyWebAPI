using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
using MyWebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using HangHoa = MyWebAPI.Models.HangHoa;

namespace MyWebAPI.Services
{
    public class HangHoaRepository : IHangHoaRepository
    {
        private readonly MyDbContext _context;
        private static int PAGE_SIZE { get; set; } = 3;

        public HangHoaRepository(MyDbContext context)
        {
            _context = context;
        }

        public HangHoa Create(HangHoaVM hangHoa)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public List<HangHoaModel> GetAll(string search, double? from, double? to, string sortBy, int page)
        {
            var allProducts = _context.HangHoa.Include(x => x.Loai).AsQueryable();

            #region Filter
            if (!string.IsNullOrEmpty(search))
            {
                allProducts = allProducts.Where(x => x.TenHh.Contains(search));
            }
            if (from.HasValue && from != 0)
            {
                allProducts = allProducts.Where(x => x.DonGia >= from.Value);
            }
            if (to.HasValue && to != 0)
            {
                allProducts = allProducts.Where(x => x.DonGia <= to.Value);
            }
            #endregion

            #region Sort
            switch (sortBy)
            {
                case "tenHH_desc":
                    allProducts = allProducts.OrderByDescending(x => x.TenHh);
                    break;
                case "price_asc":
                    allProducts = allProducts.OrderBy(x => x.DonGia);
                    break;
                case "price_desc":
                    allProducts = allProducts.OrderByDescending(x => x.DonGia);
                    break;
                default:
                    allProducts = allProducts.OrderBy(x => x.TenHh);
                    break;
            }
            #endregion

            //#region Page
            //allProducts = allProducts.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE);
            //#endregion

            //var result = allProducts.Select(x => new HangHoaModel
            //{
            //    MaHangHoa = x.MaHh,
            //    TenHangHoa = x.TenHh,
            //    DonGia = x.DonGia,
            //    TenLoai = x.Loai.TenLoai
            //});

            //return result.ToList();

            var result = PaginatedList<MyWebAPI.Data.HangHoa>.Create(allProducts, page, PAGE_SIZE);
            return result.Select(x => new HangHoaModel
            {
                MaHangHoa = x.MaHh,
                TenHangHoa = x.TenHh,
                DonGia = x.DonGia,
                TenLoai = x.Loai.TenLoai
            }).ToList();
        }

        public HangHoaModel GetById(string id)
        {
            throw new System.NotImplementedException();
        }

        public HangHoa Update(string id, HangHoaVM hangHoa)
        {
            throw new System.NotImplementedException();
        }
    }
}
