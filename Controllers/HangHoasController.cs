using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Services;

namespace MyWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HangHoasController : ControllerBase
    {
        private readonly IHangHoaRepository _hangHoaRepository;
        public HangHoasController(IHangHoaRepository hangHoaRepository)
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
                return BadRequest();
            }
        }
    }
}
