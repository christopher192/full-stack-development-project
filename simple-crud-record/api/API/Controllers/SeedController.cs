using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using API.Data;
using API.Data.Models;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // prevent non-development environment from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");

            var existingRecords = _context.Records
                .AsNoTracking().Select(x => x.Id).ToList();

            if (existingRecords.Count() > 0) {
                return new JsonResult("skip import, there is existing record");
            }

            var path = System.IO.Path.Combine(_env.ContentRootPath, "Data/Source/SalesRecords.xlsx");

            using var stream = System.IO.File.OpenRead(path);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var excelPackage = new ExcelPackage(stream);

            // get the first worksheet 
            var worksheet = excelPackage.Workbook.Worksheets[0];

            // define how many rows we want to process 
            var nEndRow = worksheet.Dimension.End.Row;

            // initialize the record counters
            var numberOfRecordsAdded = 0;

            // iterates through all rows, skipping the first one 
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                    nRow, 1, nRow, worksheet.Dimension.End.Column];

                var Region = row[nRow, 1].GetValue<string>();
                var Country = row[nRow, 2].GetValue<string>();
                var ItemType = row[nRow, 3].GetValue<string>();
                var SalesChannel = row[nRow, 4].GetValue<string>();
                var OrderPriority = row[nRow, 5].GetValue<string>();
                var OrderDate = row[nRow, 6].GetValue<string>();
                var OrderID = row[nRow, 7].GetValue<string>();
                var ShipDate = row[nRow, 8].GetValue<string>();
                var UnitsSold = row[nRow, 9].GetValue<string>();
                var UnitPrice = row[nRow, 10].GetValue<string>();
                var UnitCost = row[nRow, 11].GetValue<string>();
                var TotalRevenue = row[nRow, 12].GetValue<string>();
                var TotalCost = row[nRow, 13].GetValue<string>();
                var TotalProfit = row[nRow, 14].GetValue<string>();

                // create the record entity and fill it with xlsx data 
                var record = new Record
                {
                    Region = Region,
                    Country = Country,
                    ItemType = ItemType,
                    SalesChannel = SalesChannel,
                    OrderPriority = OrderPriority,
                    OrderDate = DateTime.Parse(OrderDate),
                    OrderID = long.Parse(OrderID),
                    ShipDate = DateTime.Parse(ShipDate),
                    UnitsSold = int.Parse(UnitsSold),
                    UnitPrice = decimal.Parse(UnitPrice),
                    UnitCost = decimal.Parse(UnitCost),
                    TotalRevenue = decimal.Parse(TotalRevenue),
                    TotalCost = decimal.Parse(TotalCost),
                    TotalProfit = decimal.Parse(TotalProfit),
                };

                // add the new record to the db context 
                await _context.Records.AddAsync(record);

                // increment the number of record 
                numberOfRecordsAdded++;
            }

            // save all the records into the database 
            if (numberOfRecordsAdded > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                Records = numberOfRecordsAdded,
            });
        }
    }
}
