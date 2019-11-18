using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public partial class TblUser
    {
        public TblUser()
        {
            TblAdmin = new HashSet<TblAdmin>();
            TblComment = new HashSet<TblComment>();
        }

        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Username must contain 4 to 15 characters")]
        [MaxLength(15, ErrorMessage = "Username must contain 4 to 15 characters")]
        [Remote("IsUniqueUserName", "User", ErrorMessage = "Username already exists")]
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$"
        , ErrorMessage = "Password should have at least one upper case, one lower case<br />one number<br />one special character<br />8 or more characters")]
        public string Password { get; set; }

        public ICollection<TblAdmin> TblAdmin { get; set; }
        public ICollection<TblComment> TblComment { get; set; }
    }
}
