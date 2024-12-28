using Microsoft.EntityFrameworkCore;

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
        public DbSet<finalProject.Models.TblGames> TblGames { get; set; } = default!;


    }
}
