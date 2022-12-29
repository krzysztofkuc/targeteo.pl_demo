using System.ComponentModel.DataAnnotations;

namespace targeteo.pl.Model
{
    public class ConfrimEmailVM
    {
        [Required]
        [Display(Name = "Nick")]
        public string UserName { get; set; }
    }
}