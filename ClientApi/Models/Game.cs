using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Game
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }

        public static int CurId = 100;

       
        public override string ToString()
        {
            return $"id = {Id} \nPlayerId = {PlayerId} \nStartTime= {StartTime} \nDuration = {Duration}\n";
        }
    }
}
