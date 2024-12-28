using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Models;

namespace finalProject.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblGamesController : ControllerBase
    {
        private readonly finalProjectContext _context;

        public TblGamesController(finalProjectContext context)
        {
            _context = context;
        }

        // GET: api/TblGames
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblGames>>> GetTblGames()
        {
            return await _context.TblGames.ToListAsync();
        }

        // GET: api/TblGames/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblGames>> GetTblGames(int? id)
        {
            var tblGames = await _context.TblGames.FindAsync(id);

            if (tblGames == null)
            {
                return NotFound();
            }

            return tblGames;
        }

        // PUT: api/TblGames/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblGames(int? id, TblGames tblGames)
        {
            if (id != tblGames.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblGames).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblGamesExists(id))
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

        // POST: api/TblGames
        [HttpPost]
        public async Task<ActionResult<TblGames>> PostTblGames(TblGames tblGames)
        {
            _context.TblGames.Add(tblGames);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTblGames", new { id = tblGames.Id }, tblGames);
        }

        // DELETE: api/TblGames/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblGames(int? id)
        {
            var tblGames = await _context.TblGames.FindAsync(id);
            if (tblGames == null)
            {
                return NotFound();
            }

            _context.TblGames.Remove(tblGames);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblGamesExists(int? id)
        {
            return _context.TblGames.Any(e => e.Id == id);
        }
        

    }
}
