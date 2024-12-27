using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using finalProject.Models;
using Microsoft.EntityFrameworkCore;
using finalProject.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Configuration;

namespace finalProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly finalProjectContext _context; // Assuming you have a DbContext for the database


        public IndexModel(ILogger<IndexModel> logger , finalProjectContext context)
        {
            _logger = logger;
            _context = context;
        }
        [BindProperty]
        public Player Player { get; set; } = default!;

        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
           if (ModelState.IsValid)
            {
                var existingPlayer = _context.TblPlayers.FirstOrDefault(p => p.Id == Player.Id);

                if (existingPlayer != null)
                {
                  //  DeleteAllPlayersFromDB();

                    // Add error if player ID already exists
                    ModelState.AddModelError("Player.Id", "This ID is already taken. Please choose another.");
                    return Page(); // Return to the same page with the error
                }
                else
                {
                    int? id = Player.Id;
                    string? name = Player.Name;                    
                    string? phone = Player.Phone;
                    string? country = Player.Country;
                   InsertNewPlayerToDB(id, name, phone, country);
                    // Add success message to ModelState after successful insert
                    TempData["SuccessMessage"] = "Registration Successful!";

                    return RedirectToPage("./Index");
                }
            }
            return Page();
        }
        private void InsertNewPlayerToDB(int? id, string? name, string? phone, string? country)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=playersDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            // Query to insert a new player into TblPlayers
            string playerQuery = $"INSERT INTO dbo.TblPlayers (Id, Name, Phone, Country, NumOfGames) VALUES ('{id}', '{name}', '{phone}', '{country}', 0)";

            // Query to insert the current date into TblDates
            string dateQuery = $"INSERT INTO dbo.TblDates (Id, DateValue) VALUES ('{id}', GETDATE())";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Insert into TblPlayers
                using (SqlCommand playerCmd = new SqlCommand(playerQuery, connection))
                {
                    playerCmd.ExecuteNonQuery();
                }

                // Insert into TblDates
                using (SqlCommand dateCmd = new SqlCommand(dateQuery, connection))
                {
                    dateCmd.ExecuteNonQuery();
                }
            }
        }



        private void DeleteAllPlayersFromDB()
{
    string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=playersDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    // Delete all dates referencing players
    string deleteDatesQuery = "DELETE FROM dbo.TblDates WHERE Id IS NOT NULL";
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        using (SqlCommand cmd = new SqlCommand(deleteDatesQuery, connection))
        {
            cmd.ExecuteNonQuery();
        }
    }

    // Now delete players
    string deletePlayersQuery = "DELETE FROM dbo.tblPlayers";
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        using (SqlCommand cmd = new SqlCommand(deletePlayersQuery, connection))
        {
            cmd.ExecuteNonQuery();
        }
    }
}

    }
}
