﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace targeteo.pl.Model.Entities
{
    public partial class OrderOpinion
    {
        public int Id { get; set; }
        public int OrderDetailId { get; set; }
        public string Text { get; set; }
        public int? PictureId { get; set; }
        public int Evaluation { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
        public virtual Picture Picture { get; set; }
    }
}