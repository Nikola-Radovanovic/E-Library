using Library.Models.LibraryModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.ViewModels
{
    public class CreateOrUpdateReservationViewModel
    {
        public Reservation Reservation { get; set; }
        public List<SelectListItem> BookList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UserList { get; set; } = new List<SelectListItem>();
    }
}