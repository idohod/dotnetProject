using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using finalProject.Models;

namespace finalProject.Pages.Players
{
    public class DetailsModel : PageModel
    {
        private readonly finalProject.Data.finalProjectContext _context;

        public DetailsModel(finalProject.Data.finalProjectContext context)
        {
            _context = context;
        }

        public TblPlayers TblPlayers { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblplayers = await _context.TblPlayers.FirstOrDefaultAsync(m => m.Id == id);
            if (tblplayers == null)
            {
                return NotFound();
            }
            else
            {
                TblPlayers = tblplayers;
            }
            return Page();
        }
    }
}
