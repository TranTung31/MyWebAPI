using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Data;
using MyWebAPI.Models;
using System;
using System.Linq;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiController : ControllerBase
    {
        private readonly MyDbContext _context;

        public LoaiController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var loais = _context.Loai.ToList();
                return Ok(loais);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var loai = _context.Loai.SingleOrDefault(x => x.MaLoai == id);
                if (loai == null)
                {
                    return NotFound();
                }
                return Ok(loai);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public IActionResult Create(LoaiModel loaiModel)
        {
            var loai = new Loai
            {
                TenLoai = loaiModel.TenLoai
            };
            _context.Loai.Add(loai);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, new
            {
                status = "OK",
                message = "Create successfully!",
                data = loai
            });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, LoaiModel loaiModel)
        {
            try
            {
                var loai = _context.Loai.SingleOrDefault(x => x.MaLoai == id);
                if (loai == null)
                {
                    return NotFound();
                }
                loai.TenLoai = loaiModel.TenLoai;
                _context.SaveChanges();
                return Ok(new
                {
                    status = "OK",
                    message = "Update successfully!"
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var loai = _context.Loai.SingleOrDefault(x => x.MaLoai == id);
                if (loai == null)
                {
                    return NotFound();
                }
                _context.Loai.Remove(loai);
                _context.SaveChanges();
                return Ok(new
                {
                    status = "OK",
                    message = "Delete successfully!"
                });
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
