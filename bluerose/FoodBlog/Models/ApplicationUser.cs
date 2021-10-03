using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FoodBlog.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string Nickname { get; set; }
        
    }
}