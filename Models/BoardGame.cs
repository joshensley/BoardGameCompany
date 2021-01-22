using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BoardGameCompany.Models
{
    public class BoardGame
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "UPC")]
        [StringLength(12, MinimumLength = 12)]
        public string UPC { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(256)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Players")]
        [Range(1, 20)]
        public int PlayerNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Image Path")]
        public string BoardGameImageFilePath { get; set; }

        [Required]
        public int BrandID { get; set; }
        public Brand Brand { get; set; }

        public ICollection<InventoryItem> InventoryItems { get; set; }

        public ICollection<UserComment> UserComments { get; set; }

    }
}