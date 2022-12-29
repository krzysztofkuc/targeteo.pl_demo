using System.ComponentModel.DataAnnotations;

namespace targeteo.pl.Model.ViewModel
{
    public class RolaVM
    {
        public string Id { get; set; }

        [DisplayFormat(NullDisplayText = "Nie ma roli")]
        public string Name { get; set; }
    }
}