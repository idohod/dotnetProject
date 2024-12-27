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
    public class IndexModel : PageModel
    {
        private readonly finalProject.Data.finalProjectContext _context;

        public IndexModel(finalProject.Data.finalProjectContext context)
        {
            _context = context;
        }
        public List<TblPlayers> TblPlayers { get; set; } = new();
       
      //  public List<TblPlayers> SortedPlayers { get; set; } = new();
        //public List<TblPlayers> CaseSensitiveSortedPlayers { get; set; } = new();


       public List<TblPlayers> nameAndGames { get; set; } = new();
        public List<TblDates> TblDates { get; set; } = new(); 

        public Dictionary<int, List<TblPlayers>> PlayersGroupedByGames { get; set; } = new();


        public Dictionary<string, List<TblPlayers>> PlayersGroupedByCountry { get; set; } = new();



        public async Task OnGetAsync()
        {
            TblPlayers = await _context.TblPlayers.ToListAsync();
        }

        public async Task OnPostGroupByCountryAsync()
        {
            PlayersGroupedByCountry = await _context.TblPlayers
                .GroupBy(p => p.Country)
                .ToDictionaryAsync(g => g.Key ?? "Unknown Country", g => g.ToList());
        }

        public async Task OnPostGroupByGamesAsync()
        {
            // Retrieve all players from the database and perform the grouping and sorting in memory
            var players = await _context.TblPlayers.ToListAsync();

            // Group by NumOfGames and order by NumOfGames in descending order
            PlayersGroupedByGames = players
                .GroupBy(p => p.NumOfGames)
                .OrderByDescending(g => g.Key)  // Sort by NumOfGames in descending order
                .ToDictionary(g => g.Key ?? -1, g => g.ToList()); // Handle null NumOfGames

        }
        public async Task OnPostSortAsync()
        {
            TblPlayers = await _context.TblPlayers
                .Select(p => new TblPlayers
                {
                    Id = p.Id,
                    Name = p.Name,
                    Phone = p.Phone,
                    Country = p.Country,
                    NumOfGames = p.NumOfGames,
                    
                })
                .OrderBy(p => (p.Name ?? "").ToLower())  // Case-insensitive sort by Name
                .ToListAsync();
        }

        public async Task OnPostNameAndGamesAsync()
        {
            nameAndGames = await _context.TblPlayers.ToListAsync();
        }

        public async Task OnPostCaseSenAsync()
        {
            TblDates = await (from player in _context.TblPlayers
                              join date in _context.TblDates
                              on player.Id equals date.Id
                              group date by player.Name into grouped
                              select new TblDates
                              {
                                  Name = grouped.Key,  // Player Name
                                  DateValue = grouped.Max(d => d.DateValue) // Most recent DateValue
                              })
                              .OrderBy(p => p.Name) // Case-sensitive sort by Name
                              .ToListAsync();
        }

        public async Task OnPostFirstByCountryAsync()
        {
            // Get the first player registered for each country (based on the oldest date from TblDates)
            var playersByCountry =
                await (from player in _context.TblPlayers
                       join date in _context.TblDates
                                       on player.Id equals date.Id
                       group date by player.Country into countryGroup
                       select new
                       {
                           Country = countryGroup.Key,
                           FirstPlayer = countryGroup
                               .OrderBy(d => d.DateValue) // Sort by the DateValue (oldest first)
                               .Join(_context.TblPlayers, date => date.Id, p => p.Id, (date, p) => p) // Join with TblPlayers to get the player info
                               .FirstOrDefault() // Select the first player with the earliest DateValue
                       }).ToListAsync();

            // Map the result to a list of TblPlayers with the relevant data
            var result = playersByCountry.Select(item => new TblPlayers
            {
                Country = item.Country,
                Name = item.FirstPlayer?.Name,
                Phone = item.FirstPlayer?.Phone,
                NumOfGames = item.FirstPlayer?.NumOfGames,

            }).ToList();

            // Assign the result to the model
            TblPlayers = result;
        }


    }
}
