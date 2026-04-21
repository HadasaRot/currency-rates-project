using Microsoft.AspNetCore.Mvc;
using CurrencyRates.Data;
using CurrencyRates.Models;
using CurrencyRates.Services;

namespace CurrencyRates.Controllers
{
   

    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyDataService _queryService;
        private readonly CurrencyImportService _boiService;

        public CurrencyController(CurrencyDataService queryService, CurrencyImportService boiService)
        {
            _queryService = queryService;
            _boiService = boiService;
        }

        [HttpGet("week")]
        public IActionResult GetWeek() => Ok(_queryService.GetWeek());

        [HttpGet("month")]
        public IActionResult GetMonth() => Ok(_queryService.GetMonth());

        [HttpGet("half-year")]
        public IActionResult GetHalfYear() => Ok(_queryService.GetHalfYear());

        [HttpGet("year")]
        public IActionResult GetYear() => Ok(_queryService.GetYear());

        [HttpPost("import-boi")]
        public async Task<IActionResult> ImportFromBoI()
        {
            try
            {
                var result = await _boiService.FetchAndSaveRates();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}