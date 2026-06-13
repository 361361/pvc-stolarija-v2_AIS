# PVC Stolarija — AIS 2025

Webshop aplikacija za upravljanje PVC stolarijom. Baziran na projektu Autoskola, prilagođen za temu PVC stolarije.

## Mapa pojmova (Autoskola → PVC Stolarija)

| Autoskola        | PVC Stolarija     |
|------------------|-------------------|
| Instruktor       | Radnik            |
| Kandidat         | Kupac             |
| Vozilo           | Proizvod          |
| Čas              | Narudžba          |
| Ispit            | Reklamacija       |
| TipGoriva        | TipMaterijala     |
| VoznoStanje      | Dostupnost        |
| BrojLicence      | BrojUgovora       |
| DatumUpisa       | DatumRegistracije |
| TipČasa (Teorijski/Praktičan) | TipNarudžbe (Online/Lično) |
| TipIspita (Teorijski/Praktičan) | TipReklamacije (Pisana/Terenska) |

## Potrebni alati

- Visual Studio 2022
- SQL Server Express
- .NET 8 SDK

## Pokretanje

1. Otvoriti `src/PvcStolarija.sln`
2. Proveriti connection string u `appsettings.json`
3. Package Manager Console (Default project: **PvcStolarija.DAL**):
   ```
   Add-Migration Initial
   Update-Database
   ```
4. Pokrenuti projekat (F5)

## Podrazumevani admin nalog

| Korisničko ime | Lozinka   | Rola          |
|----------------|-----------|---------------|
| admin          | Admin123! | Administrator |

## Arhitektura

- **PvcStolarija.MVC** — Prezentacioni sloj (ASP.NET Core MVC, .NET 8)
- **PvcStolarija.BLL** — Sloj poslovne logike (interfejsi + servisi)
- **PvcStolarija.DAL** — Sloj podataka (EF Core, modeli, DbContext)

## Poslovna pravila

- **Terenska reklamacija** mora imati bar jedan reklamirani proizvod
- **Pisana reklamacija** ne može imati proizvode
- Reklamacija je rešena ako je prioritet ≥ 51
- Narudžbe prikazuju samo dostupne proizvode
