﻿using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Inventory
{
    public class ReturnInventoryItem
    {
        public int? id { get; set; }
        [Required]
        public string serialimei { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string supplier { get; set; }
        [Required]
        public string date { get; set; }
        [Required]
        public int quantity { get; set; }
        public string? notes { get; set; }
        public bool? alvlp { get; set; }
        public bool? ul { get; set; }
        public bool? mdm { get; set; }
        public bool? reset { get; set; }
        public bool? gtg { get; set; }
        public Models.Status Status { get; set; }
        public Models.Location Location { get; set; }
    }
}