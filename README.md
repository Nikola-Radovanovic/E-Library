# E-Library
ASP.NET Core MVC aplikacija za rezervaciju knjiga (ASP.NET Core MVC + Entity Framework Core).

Dva tipa korisnika: bibliotekar i posetilac.

Bibliotekar:
  - Može da unosi nove knjige u sistem
  - Potvrda zahteva za izdavanjem knjige (potvrda rezervacije)
  - Označavanje da je knjiga vraćena
  - Može videti sve rezervacije svih korisnika i datume do kada knjige treba da se vrate
  - Može u svakom trenutku vršiti kompletan CRUD nad rezervacijama

Posetilac (korisnik):
  - Prijavljeni i neprijavljeni posetioci mogu videti sve knjige u biblioteci i dostupnost svake od njih (koliko je primeraka trenutno na stanju)
  - Prijavljeni korisnik može videti knjige koje je rezervisao, prikazuju se prihvaćene rezervacije i rezervacije za koje se čeka potvrda bibliotekara
  - Ukoliko postoji slobodan primerak knjige, nakon potvrđene rezervacije, korisniku se automatski generiše datum izdavanja i datum do kada treba vratiti knjigu
  - Korisnik može rezervisati najviše pet primeraka jedne knjige
