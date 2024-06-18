using MyWebAPI.Models;
using System.Collections.Generic;

namespace MyWebAPI.Services
{
    public interface IHangHoaRepository
    {
        List<HangHoaModel> GetAll(string search, double? from, double? to, string sortBy, int page = 1);
        HangHoaModel GetById(string id);
        HangHoa Create(HangHoaVM hangHoa);
        HangHoa Update(string id, HangHoaVM hangHoa);
        void Delete(string id);
    }
}
