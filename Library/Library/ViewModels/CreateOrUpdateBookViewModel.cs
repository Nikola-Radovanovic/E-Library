using Library.Models.LibraryModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.ViewModels
{
    public class CreateOrUpdateBookViewModel
    {
        public Book Book { get; set; }
        public List<SelectListItem> AuthorList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PublisherList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> GenreList { get; set; } = new List<SelectListItem>();
    }
}
