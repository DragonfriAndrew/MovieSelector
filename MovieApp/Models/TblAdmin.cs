using System;
using System.Collections.Generic;

namespace MovieApp.Models
{
    public partial class TblAdmin
    {
        public Guid AdminId { get; set; }
        public Guid UserId { get; set; }

        public TblUser User { get; set; }
    }
}
