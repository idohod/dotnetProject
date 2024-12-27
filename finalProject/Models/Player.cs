using System.ComponentModel.DataAnnotations;

namespace finalProject.Models
{
    public class Player
    {
        [Required(ErrorMessage = "Must enter name.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Minimum 2 chars")]
        [Display(Name = "Name")]
       
        public string? Name { get; set; }

        [Required(ErrorMessage = "Must enter id.")]
        [Range(1, 1000, ErrorMessage = "Invalid Id: must be 1 to 1000")]
        [Display(Name = "Id")]
        

        public int? Id { get; set; }

        [Required(ErrorMessage = "Must enter phone.")]
        [RegularExpression("[0-9]+", ErrorMessage = "digits only")]
        [Display(Name = "Phone")]


        public string? Phone { get; set; }
        [Required(ErrorMessage = "Must select country.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "select a country")]
        [Display(Name = "Country")]

        public string? Country { get; set; }

        public int NumOfGames { get; set; }

        public List<DateTime> Dates { get; set; } = new();


    }
}
