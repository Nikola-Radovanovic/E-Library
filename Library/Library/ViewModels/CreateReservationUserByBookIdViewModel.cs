using Library.Models.LibraryModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.ViewModels
{
    public class CreateReservationUserByBookIdViewModel
    {
        public Reservation Reservation { get; set; }
        public Book Book { get; set; }
    }
}