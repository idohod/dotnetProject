using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using finalProject.Models;

namespace finalProject.Data
{
    public class finalProjectContext : DbContext
    {
        public finalProjectContext (DbContextOptions<finalProjectContext> options)
            : base(options)
        {
        }

        public DbSet<finalProject.Models.TblPlayers> TblPlayers { get; set; } = default!;
        public DbSet<finalProject.Models.TblDates> TblDates { get; set; } = default!;

    }
}
