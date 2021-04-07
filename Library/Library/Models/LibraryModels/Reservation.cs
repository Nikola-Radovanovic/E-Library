using Library.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.LibraryModels
{
    public class Reservation
    {
        [Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1)]
        [Display(Name = "Naslov")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        [Column(Order = 2)]
        [Display(Name = "Član")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Column(Order = 3)]
		[Display(Name = "Datum rezervacije")]
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        [Column(Order = 4)]
        public bool? IsAccepted { get; set; }

        [Column(Order = 5)]
        [Display(Name = "Datum izdavanja")]
        public DateTime? RentDate { get; set; }

        [Column(Order = 6)]
        [Display(Name = "Datum vraćanja")]
        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "Unesite broj primeraka")]
        [Range(1, 5, ErrorMessage = "Možete rezervisati najmanje 1 a najviše 5 primeraka")]
        [Display(Name = "Primeraka")]
        public int RentedBooksNumber { get; set; } = 1;

        public bool? IsReturned { get; set; } = false;
    }
}