﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBase_API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using StackExchange.Profiling;

namespace DataBase.Controllers
{
    /// <summary>
    /// 测试类型数据
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 静态数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //using (MiniProfiler.Current.Step("开始加载数据"))
            //{
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            //}
        }

        /// <summary>
        /// 获取html片段
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("GetHtml")]
        //public IActionResult GetHtml()
        //{
        //    var html = MiniProfiler.Current.RenderIncludes(HttpContext);
        //    return Ok(html.Value);
        //}
    }
}
