using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using CurrencyRates.Data;
using CurrencyRates.Models;
using System.Collections.Generic;
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
        [HttpGet("week")]
        public IActionResult GetWeek()
        {
            var fromDate = DateTime.Today.AddDays(-6);

            var data = _context.CurrencyRates
                .Where(x => x.RateDate >= fromDate)
                .OrderBy(x => x.RateDate)
                .ToList();

            return Ok(data);
        }


        [HttpGet("month")]
        public IActionResult GetMonth()
        {
            var fromDate = DateTime.Today.AddMonths(-1);

            var data = _context.CurrencyRates
                .Where(x => x.RateDate >= fromDate)
                .OrderBy(x => x.RateDate)
                .ToList();

            return Ok(data);
        }

        [HttpGet("half-year")]
        public IActionResult GetHalfYear()
        {
            var fromDate = DateTime.Today.AddMonths(-6);

            var data = _context.CurrencyRates
                .Where(x => x.RateDate >= fromDate)
                .ToList();

            var result = data
                .GroupBy(x => new { x.CurrencyCode, x.RateDate.Year, x.RateDate.Month })
                .Select(g => g.OrderBy(x => x.RateDate).First())
                .OrderBy(x => x.CurrencyCode)
                .ThenBy(x => x.RateDate)
                .ToList();

            return Ok(result);

        }

        [HttpGet("year")]
        public IActionResult GetYear()
        {
            var today = DateTime.Today;

            var currencies = new[] { "USD", "GBP", "CHF", "SEK" };

            var data = _context.CurrencyRates.ToList();

            var result = new List<CurrencyRate>();

            foreach (var currency in currencies)
            {
                for (int i = 0; i < 12; i++)
                {
                    var targetDate = today.AddMonths(-i);

                    var record = data
                        .Where(x => x.CurrencyCode == currency &&
                                    x.RateDate.Year == targetDate.Year &&
                                    x.RateDate.Month == targetDate.Month &&
                                    x.RateDate.Day <= targetDate.Day)
                        .OrderByDescending(x => x.RateDate)
                        .FirstOrDefault();

                    // אם אין תאריך קטן או שווה – קחי הכי קרוב בחודש
                    if (record == null)
                    {
                        record = data
                            .Where(x => x.CurrencyCode == currency &&
                                        x.RateDate.Year == targetDate.Year &&
                                        x.RateDate.Month == targetDate.Month)
                            .OrderByDescending(x => x.RateDate)
                            .FirstOrDefault();
                    }

                    if (record != null)
                    {
                        result.Add(record);
                    }
                }
            }

            var orderedResult = result
                .OrderBy(x => x.CurrencyCode)
                .ThenBy(x => x.RateDate)
                .ToList();

            return Ok(orderedResult);
        }
    }
}