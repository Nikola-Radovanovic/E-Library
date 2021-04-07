using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Library.Models.LibraryModels;
using Microsoft.AspNetCore.Identity;

namespace Library.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(30)")]
        [Display(Name = "Ime")]
        public string Firstname { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(30)")]
        [Display(Name = "Prezime")]
        public string Lastname { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}