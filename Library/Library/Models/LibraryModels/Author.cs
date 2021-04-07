using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.LibraryModels
{
    public class Author
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite ime autora")]
        [Display(Name = "Autor")]
        [Column(TypeName = "nvarchar(60)")]
        public string AuthorName { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}