using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public partial class TblComment
    {
        public Guid CommentId { get; set; }
        public Guid MovieId { get; set; }
        public Guid UserId { get; set; }
        [Required]
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public TblUser User { get; set; }
        public TblMovie Movie { get; set; }
    }
}
