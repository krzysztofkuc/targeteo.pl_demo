using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace targeteo.pl.Model.Entities
{
    public class AccountOperationStatusVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Caption { get; set; }
    }
}