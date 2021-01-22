using System;
using System.ComponentModel.DataAnnotations;

namespace BoardGameCompany.Models
{
    public class UserComment
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string UserBoardGameComment { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime UserBoardGameCommentDate { get; set; }

        [Required]
        [Display(Name = "User Id")]
        public string UserIdBoardGameComment { get; set; }

        public int BoardGameID { get; set; }
        public BoardGame BoardGame { get; set; }

    }
}