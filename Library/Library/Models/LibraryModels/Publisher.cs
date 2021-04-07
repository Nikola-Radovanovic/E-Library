using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.LibraryModels
{
    public class Publisher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Unesite naziv izdavača")]
        [Display(Name = "Izdavač")]
        [Column(TypeName = "nvarchar(80)")]
        public string PublisherName { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}