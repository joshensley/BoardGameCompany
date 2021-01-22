using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompany.Pages.StoreFront.UserCommentsViewModel
{
    public class UserCommentGroup
    {
        public string Id { get; set; }
        public int BoardGameID { get; set; }
        public string Name { get; set; }
        public string UserComment { get; set; }

        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; }
        public string UserAvatarFilePath { get; set; }
    }
}
