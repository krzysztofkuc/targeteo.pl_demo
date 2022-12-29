using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Model.ViewModel
{
    public class CityVm
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
    }
}
