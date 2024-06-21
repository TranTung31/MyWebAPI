using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
using MyWebAPI.Models;
using System;
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

        public HangHoaOutputModel GetById(string id)
        {
            var query = from hangHoa in _context.HangHoa
                        join loai in _context.Loai on hangHoa.MaLoai equals loai.MaLoai
                        where hangHoa.MaHh.ToString() == id
                        select new
                        {
                            MaHh = hangHoa.MaHh,
                            TenHh = hangHoa.TenHh,
                            MoTa = hangHoa.MoTa,
                            DonGia = hangHoa.DonGia,
                            GiamGia = hangHoa.GiamGia,
                            TenLoai = loai.TenLoai
                        };

            var result = query.FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            return new HangHoaOutputModel
            {
                MaHh = result.MaHh,
                TenHh = result.TenHh,
                MoTa = result.MoTa,
                DonGia = result.DonGia,
                GiamGia = result.GiamGia,
                TenLoai = result.TenLoai
            };
        }

        public ApiResponseModel Create(HangHoaInputModel hangHoa)
        {
            var newProduct = new Data.HangHoa
            {
                MaHh = Guid.NewGuid(),
                TenHh = hangHoa.TenHangHoa,
                MoTa = hangHoa.MoTa,
                DonGia = hangHoa.DonGia,
                GiamGia = hangHoa.GiamGia,
                MaLoai = hangHoa.MaLoai,
            };

            _context.HangHoa.Add(newProduct);
            _context.SaveChanges();

            return new ApiResponseModel
            {
                IsSuccess = true,
                Message = "Create the product successfully!",
                Data = newProduct
            };
        }

        public ApiResponseModel Update(string id, HangHoaInputModel hangHoa)
        {
            var findHangHoa = _context.HangHoa.FirstOrDefault(x => x.MaHh.ToString() == id);

            if (findHangHoa == null)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "The product doesn't exist!"
                };
            }

            findHangHoa.TenHh = hangHoa.TenHangHoa;
            findHangHoa.MoTa = hangHoa.MoTa;
            findHangHoa.DonGia = hangHoa.DonGia;
            findHangHoa.GiamGia = hangHoa.GiamGia;
            findHangHoa.MaLoai = hangHoa.MaLoai;

            _context.HangHoa.Update(findHangHoa);
            _context.SaveChanges();

            return new ApiResponseModel
            {
                IsSuccess = true,
                Message = "Update the product successfully!"
            };
        }

        public ApiResponseModel Delete(string id)
        {
            var findHangHoa = _context.HangHoa.FirstOrDefault(x => x.MaHh.ToString() == id);

            if (findHangHoa == null)
            {
                return new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "The product doesn't exist!"
                };
            }

            _context.HangHoa.Remove(findHangHoa);
            _context.SaveChanges();

            return new ApiResponseModel
            {
                IsSuccess = true,
                Message = "Delete the product successfully!",
            };
        }
    }
}
