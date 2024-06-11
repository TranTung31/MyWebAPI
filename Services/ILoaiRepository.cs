using MyWebAPI.Data;
using MyWebAPI.Models;
using System.Collections.Generic;

namespace MyWebAPI.Services
{
    public interface ILoaiRepository
    {
        List<LoaiVM> GetAll();
        LoaiVM GetById(int id);
        LoaiVM Create(LoaiModel loai);
        void Put(LoaiVM loai);
        void Delete(int id);
    }
}
