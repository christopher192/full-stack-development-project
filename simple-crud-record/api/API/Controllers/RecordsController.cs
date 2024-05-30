using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Data.Models;
using System.Collections;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class Search
        {
            public int ColumnId { get; set; }
            public string? ColumnValue { get; set; }
        }

        public class PagingRequest
        {
            public int Page { get; set; }

            public int PerPage { get; set; }

            public int? SortCol { get; set; }

            public string? SortDir { get; set; }

            public List<Search>? Searches { get; set; }
        }

        public class Respond
        {
            public List<Record>? Data { get; set; }
            public int Total { get; set; }
            public int Total_Page { get; set; }
            public int Query_Count { get; set; }
        }

        [HttpPost]
        [Route("GetReactTable")]
        public JsonResult GetReactTable([FromBody] PagingRequest paging)
        {
            var data = new Respond();

            var searchForName = paging.Searches.Where(x => x.ColumnId == 1).FirstOrDefault().ColumnValue;
            var searchForDescription = paging.Searches.Where(x => x.ColumnId == 2).FirstOrDefault().ColumnValue;
            var searchForURL = paging.Searches.Where(x => x.ColumnId == 3).FirstOrDefault().ColumnValue;
            var searchForType = paging.Searches.Where(x => x.ColumnId == 4).FirstOrDefault().ColumnValue;

            IQueryable<Record> query = null;

            query = _context.Records;
            data.Total = query.Count();
            data.Total_Page = paging.Page;

/*            if (!String.IsNullOrEmpty(searchForName))
            {
                query = query.Where(x => x.Name != null && x.Name.ToUpper().Contains(searchForName.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchForDescription))
            {
                query = query.Where(x => x.Description != null && x.Description.ToUpper().Contains(searchForDescription.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchForURL))
            {
                query = query.Where(x => x.URL != null && x.URL.ToUpper().Contains(searchForURL.ToUpper()));
            }

            if (!String.IsNullOrEmpty(searchForType))
            {
                query = query.Where(x => x.Type != null && x.Type.ToUpper().Contains(searchForType.ToUpper()));
            }

            if (paging.SortCol != null && !String.IsNullOrEmpty(paging.SortDir))
            {
                switch (paging.SortCol)
                {
                    case 1:
                        query = paging.SortDir == "asc" ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                    case 2:
                        query = paging.SortDir == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                        break;
                    case 3:
                        query = paging.SortDir == "asc" ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description);
                        break;
                    case 4:
                        query = paging.SortDir == "asc" ? query.OrderBy(x => x.URL) : query.OrderByDescending(x => x.URL);
                        break;
                    case 5:
                        query = paging.SortDir == "asc" ? query.OrderBy(x => x.Type) : query.OrderByDescending(x => x.Type);
                        break;
                    default:
                        query = paging.SortDir == "asc" ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                        break;
                }
            }*/

            var pageb = (paging.Page - 1) * paging.PerPage;
            var x = query.Skip(pageb).Take(paging.PerPage);
            data.Data = x.ToList();
            data.Query_Count = x.Count();

            return new JsonResult(data);
        }

        // GET: api/Records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Record>>> GetRecords()
        {
            return await _context.Records.ToListAsync();
        }

        // GET: api/Records/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Record>> GetRecord(int id)
        {
            var @record = await _context.Records.FindAsync(id);

            if (@record == null)
            {
                return NotFound();
            }

            return @record;
        }

        // PUT: api/Records/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecord(int id, Record @record)
        {
            if (id != @record.Id)
            {
                return BadRequest();
            }

            _context.Entry(@record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Records
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Record>> PostRecord(Record @record)
        {
            _context.Records.Add(@record);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecord", new { id = @record.Id }, @record);
        }

        // DELETE: api/Records/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            var @record = await _context.Records.FindAsync(id);
            if (@record == null)
            {
                return NotFound();
            }

            _context.Records.Remove(@record);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecordExists(int id)
        {
            return _context.Records.Any(e => e.Id == id);
        }
    }
}
