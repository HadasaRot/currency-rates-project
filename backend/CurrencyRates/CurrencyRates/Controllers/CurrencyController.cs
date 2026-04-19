using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using CurrencyRates.Data;
namespace CurrencyRates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CurrencyController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var data = _context.CurrencyRates.ToList();
            return Ok(data);
        }
    }
}