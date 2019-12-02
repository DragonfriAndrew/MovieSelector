using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Models
{
    public static class CurrentUser
    {
        public static Dictionary<string, TblUser> Users = new Dictionary<string, TblUser>();
        public static List<Guid[]> MovieUser = new List<Guid[]>();
    }
}
