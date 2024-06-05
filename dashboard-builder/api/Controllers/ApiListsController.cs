using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiListsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiListsController(ApplicationDbContext context)
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
            public List<ApiList> Data { get; set; }
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

            IQueryable<ApiList> query = null;

            query = _context.ApiLists;
            data.Total = query.Count();
            data.Total_Page = paging.Page;

            if (!String.IsNullOrEmpty(searchForName))
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
            }

            var pageb = (paging.Page - 1) * paging.PerPage;
            var x = query.Skip(pageb).Take(paging.PerPage);
            data.Data = x.ToList();
            data.Query_Count = x.Count();

            return new JsonResult(data);
        }

        [HttpGet]
        [Route("GetVueTable")]
        public async Task<JsonResult> GetVueTable()
        {
            var data = await _context.ApiLists.ToListAsync();

            return new JsonResult(data);
        }

        [HttpGet]
        [Route("GetApiLists")]
        public async Task<ActionResult<List<ApiList>>> GetApiLists()
        {
            var apiLists = await _context.ApiLists.ToListAsync();

            if (apiLists == null)
            {
                return NotFound();
            }

            return apiLists;
        }

        [HttpGet]
        [Route("GetApiList")]
        public async Task<ActionResult<ApiList>> GetApiList(int id)
        {
            var pond = await _context.ApiLists.FindAsync(id);

            if (pond == null)
            {
                return NotFound();
            }

            return pond;
        }

        [HttpPut]
        [Route("UpdateApiList")]
        public async Task<ActionResult<ApiList>> UpdateApiList([FromBody] ApiList apiList)
        {
            _context.Entry(apiList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiListExists(apiList.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetApiList", new { id = apiList.Id }, apiList);
        }

        [HttpPost]
        [Route("CreateApiList")]
        public async Task<ActionResult<ApiList>> CreateApiList([FromBody] ApiList apiList)
        {
            _context.ApiLists.Add(apiList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApiList", new { id = apiList.Id }, apiList);
        }

        [HttpDelete]
        [Route("DeleteApiList")]
        public async Task<IActionResult> DeleteApiList(int id)
        {
            var apiList = await _context.ApiLists.FindAsync(id);
            if (apiList == null)
            {
                return NotFound();
            }

            _context.ApiLists.Remove(apiList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApiListExists(int id)
        {
            return (_context.ApiLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
