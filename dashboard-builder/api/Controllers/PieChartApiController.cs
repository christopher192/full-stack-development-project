﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PieChartApiController : ControllerBase
    {
        public class JSONData
        {
            public List<int>? Data { get; set; }
            public List<string>? Labels { get; set; }
        }

        [HttpGet]
        [Route("GetPieChartApiFirst")]
        public JsonResult GetPieChartApiFirst()
        {
            var jsonData = new JSONData();
            jsonData.Data = new List<int>() { 900, 300, 600, 659, 150 };
            jsonData.Labels = new List<string>() { "Africa", "Asia", "Europe", "Latin America", "North America" };

            return new JsonResult(jsonData);
        }

        [HttpGet]
        [Route("GetPieChartApiSecond")]
        public JsonResult GetPieChartApiSecond()
        {
            var jsonData = new JSONData();
            jsonData.Data = new List<int>() { 400, 300, 350, 900, 150, 770, 489 };
            jsonData.Labels = new List<string>() { "Latin America", "North America", "Malysia", "Norland", "Asia", "Africa", "Europe" };

            return new JsonResult(jsonData);
        }

        [HttpGet]
        [Route("GetPieChartApiThird")]
        public JsonResult GetPieChartApiThird()
        {
            var jsonData = new JSONData();
            jsonData.Data = new List<int>() { 200, 270, 350, 379, 450, 970, 1089 };
            jsonData.Labels = new List<string>() { "Malysia", "Norland", "Latin America", "Asia", "North America", "Africa", "Europe" };

            return new JsonResult(jsonData);
        }

        [HttpGet]
        [Route("GetPieChartApiForth")]
        public JsonResult GetPieChartApiForth()
        {
            var jsonData = new JSONData();
            jsonData.Data = new List<int>() { 730, 470, 650, 479, 650, 870, 350, 1000, 550 };
            jsonData.Labels = new List<string>() { "China", "Latin America", "Asia", "Malysia", "Europe", "North America", "Africa", "Philipine", "Korean" };

            return new JsonResult(jsonData);
        }

        [HttpGet]
        [Route("GetPieChartApiFifth")]
        public JsonResult GetPieChartApiFifth()
        {
            var jsonData = new JSONData();
            jsonData.Data = new List<int>() { 730, 470, 650, 479 };
            jsonData.Labels = new List<string>() { "Japan", "America", "Indonesia", "England" };

            return new JsonResult(jsonData);
        }
    }
}