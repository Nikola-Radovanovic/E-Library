using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.LibraryModels
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Unesite naziv žanra")]
        [Display(Name = "Žanr")]
        [Column(TypeName = "nvarchar(40)")]
        public string GenreName { get; set; }

        //public virtual ICollection<Book> Books { get; set; }
    }
}