using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public partial class TblMovie
    {
        public TblMovie()
        {
            TblComment = new HashSet<TblComment>();
        }

        public Guid MovieId { get; set; }
        [Required]
        public string MovieName { get; set; }
        [Required]
        public string Director { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:YYYY-MM-DD}")]
        public DateTime? Year { get; set; }
        public string Synopsis { get; set; }
        public string PosterLink { get; set; }
        public string TrailerLink { get; set; }
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }

        public ICollection<TblComment> TblComment { get; set; }
    }
}
