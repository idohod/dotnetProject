namespace finalProject.Models
{
    public class MoveRequest
    {
        public int GameId { get; set; }
        public int FromRow { get; set; } // Source row
        public int FromCol { get; set; } // Source column
        public int ToRow { get; set; }   // Destination row
        public int ToCol { get; set; }   // Destination column
    }
}
