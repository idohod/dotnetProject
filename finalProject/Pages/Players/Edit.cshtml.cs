using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Models;

namespace finalProject.Pages.Players
{
    public class EditModel : PageModel
    {
        private readonly finalProject.Data.finalProjectContext _context;

        public EditModel(finalProject.Data.finalProjectContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblPlayers TblPlayers { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblplayers =  await _context.TblPlayers.FirstOrDefaultAsync(m => m.Id == id);
            if (tblplayers == null)
            {
                return NotFound();
            }
            TblPlayers = tblplayers;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TblPlayers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblPlayersExists(TblPlayers.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TblPlayersExists(int? id)
        {
            return _context.TblPlayers.Any(e => e.Id == id);
        }
    }
}
