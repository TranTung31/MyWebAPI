using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using MyWebAPI.Services;

namespace MyWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HangHoaController : ControllerBase
    {
        private readonly IHangHoaRepository _hangHoaRepository;
        public HangHoaController(IHangHoaRepository hangHoaRepository)
        {
            _hangHoaRepository = hangHoaRepository;
        }

        [HttpGet]
        public IActionResult GetAll(string search, double from, double to, string sortBy, int page = 1)
        {
            try
            {
                var result = _hangHoaRepository.GetAll(search, from, to, sortBy, page);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var result = _hangHoaRepository.GetById(id);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpPost]
        public IActionResult Create(HangHoaInputModel hangHoa)
        {
            try
            {
                var result = _hangHoaRepository.Create(hangHoa);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, HangHoaInputModel hangHoa)
        {
            try
            {
                var result = _hangHoaRepository.Update(id, hangHoa);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var result = _hangHoaRepository.Delete(id);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }
    }
}
