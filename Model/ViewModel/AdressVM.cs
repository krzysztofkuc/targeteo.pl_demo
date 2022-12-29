using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace targeteo.pl.Model.ViewModel
{
    public class AdressVM
    {
        public int AdressId { get; set; }

        [Required]
        [DisplayName("Imię")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Nazwisko")]
        public string Surname { get; set; }

        [Required]
        [DisplayName("Ulica")]
        public string Street { get; set; }

        [Required]
        [DisplayName("Miasto")]
        public string City { get; set; }

        [Required]
        [DisplayName("Wojewodztwo")]
        public string State { get; set; }

        [Required]
        [DisplayName("Kod pocztowy")]
        public int PostalCode { get; set; }

        [Required]
        [DisplayName("Numer telefonu")]
        public string CellPhone { get; set; }

        [Required]
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}