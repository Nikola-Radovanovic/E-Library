using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
    public class UserRoleModel
    {
        [Required(ErrorMessage = "Odaberite korisnika")]
        [Display(Name = "Korisnik")]
        public string UserId { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Odaberite rolu")]
        [Display(Name = "Rola")]
        public string RoleName { get; set; }

        public List<SelectListItem> UserList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();
    }
}