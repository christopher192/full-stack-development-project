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

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardBuilderDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardBuilderDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetDashboardBuilderDatas")]
        public async Task<ActionResult<IEnumerable<DashboardBuilderData>>> GetDashboardBuilderDatas()
        {
          if (_context.DashboardBuilderDatas == null)
          {
              return NotFound();
          }
          return await _context.DashboardBuilderDatas.ToListAsync();
        }

        [HttpGet]
        [Route("GetDashboardBuilderData")]
        public async Task<ActionResult<DashboardBuilderData>> GetDashboardBuilderData(int id)
        {
          if (_context.DashboardBuilderDatas == null)
          {
              return NotFound();
          }
            var dashboardBuilderData = await _context.DashboardBuilderDatas.FindAsync(id);

            if (dashboardBuilderData == null)
            {
                return NotFound();
            }

            return dashboardBuilderData;
        }

        [HttpPut]
        [Route("PutDashboardBuilderData")]
        public async Task<IActionResult> PutDashboardBuilderData(int id, DashboardBuilderData dashboardBuilderData)
        {
            if (id != dashboardBuilderData.Id)
            {
                return BadRequest();
            }

            _context.Entry(dashboardBuilderData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DashboardBuilderDataExists(id))
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

        [HttpPost]
        [Route("PostDashboardBuilderData")]
        public async Task<ActionResult<DashboardBuilderData>> PostDashboardBuilderData(DashboardBuilderData dashboardBuilderData)
        {
          if (_context.DashboardBuilderDatas == null)
          {
              return Problem("Entity set 'ApplicationDbContext.DashboardBuilderDatas'  is null.");
          }
            _context.DashboardBuilderDatas.Add(dashboardBuilderData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDashboardBuilderData", new { id = dashboardBuilderData.Id }, dashboardBuilderData);
        }

        [HttpDelete]
        [Route("DeleteDashboardBuilderData")]
        public async Task<IActionResult> DeleteDashboardBuilderData(int id)
        {
            if (_context.DashboardBuilderDatas == null)
            {
                return NotFound();
            }
            var dashboardBuilderData = await _context.DashboardBuilderDatas.FindAsync(id);
            if (dashboardBuilderData == null)
            {
                return NotFound();
            }

            _context.DashboardBuilderDatas.Remove(dashboardBuilderData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DashboardBuilderDataExists(int id)
        {
            return (_context.DashboardBuilderDatas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
