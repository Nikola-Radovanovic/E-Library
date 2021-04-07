using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.LibraryModels
{
    public class Book
    {
        [Column(Order = 0)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite naslov knjige")]
        [Column(Order = 1, TypeName = "nvarchar(200)")]
        [MaxLength(200, ErrorMessage = "Možete uneti najviše 200 karaktera")]
        [Display(Name = "Naslov")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Unesite broj strana")]
        [Column(Order = 5)]
        [Range(0, 9999, ErrorMessage = "Broj strana može biti 0-9999")]
        [Display(Name = "Strana")]
        public int? NumberOfPages { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite ISBN broj knjige")]
        [Column(Order = 6, TypeName = "nvarchar(50)")]
        [MaxLength(50, ErrorMessage = "Možete uneti najviše 50 karaktera")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Unesite broj primeraka")]
        [Column(Order = 7)]
        [Range(0, 9999, ErrorMessage = "Broj primeraka može biti 0-999")]
        [Display(Name = "Stanje")]
        public int? NumberOfCopies { get; set; }

        [Column(Order = 8)]
        [Display(Name = "Datum unosa")]
        public DateTime EntryDate { get; set; } = DateTime.Now;

        [Column(Order = 2)]
        [Display(Name = "Autor")]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        [Column(Order = 3)]
        [Display(Name = "Izdavač")]
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        [Column(Order = 4)]
        [Display(Name = "Žanr")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}