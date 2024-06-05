using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Data.Models;
using HotChocolate.Language;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetBoxesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WidgetBoxesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class WidgetBoxColumnDescriptionLists
        {
            public List<string> Columns { get; set; } = new List<string>();
            public List<string> Rows { get; set; } = new List<string>();
        }

        [HttpGet]
        [Route("GetWidgetBoxColumnDescriptionLists")]
        public JsonResult GetWidgetBoxColumnDescriptionLists()
        {
            var entityType = _context.Model.FindEntityType("API.Data.Models.WidgetBox");
            var data = new WidgetBoxColumnDescriptionLists();

            // Table info 
            var tableName = entityType.GetTableName();
            var tableSchema = entityType.GetSchema();

            // Column info 
            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName();
                var columnType = property.GetColumnType();

                if (columnName != "Id" && columnName != "Description")
                {
                    data.Columns.Push(columnName);
                }
            };

            var rows = _context.WidgetBoxs.Select(x => x.Description).ToList();
            data.Rows = rows;

            return new JsonResult(data);
        }

        [HttpGet]
        [Route("GetWidgetBoxRealTime")]
        public JsonResult GetWidgetBoxRealTime(string column, string description)
        {
            var data = "";
            
            if (!String.IsNullOrEmpty(description) && !String.IsNullOrEmpty(column)) {
                var sqlData = _context.WidgetBoxs.Where(x => x.Description == description).FirstOrDefault();
                
                switch (column)
                {
                    case "ClaimedListing":
                        data = sqlData.ClaimedListing.ToString();
                        break;
                    case "ReportedListing":
                        data = sqlData.ReportedListing.ToString();
                        break;
                    case "TotalCategory":
                        data = sqlData.TotalCategory.ToString();
                        break;
                    case "TotalListing":
                        data = sqlData.TotalListing.ToString();
                        break;
                    default:
                        data = "";
                        break;
                }
            }

            return new JsonResult(data);
        }
    }
}
