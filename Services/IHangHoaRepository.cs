using MyWebAPI.Models;
using System.Collections.Generic;

namespace MyWebAPI.Services
{
    public interface IHangHoaRepository
    {
        List<HangHoaModel> GetAll(string search, double? from, double? to, string sortBy, int page = 1);
        HangHoaOutputModel GetById(string id);
        ApiResponseModel Create(HangHoaInputModel hangHoa);
        ApiResponseModel Update(string id, HangHoaInputModel hangHoa);
        ApiResponseModel Delete(string id);
    }
}
