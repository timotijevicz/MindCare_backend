# 🧠 MindCare — Backend API

ASP.NET Core 8 Web API za **MindCare**, aplikaciju za praćenje mentalnog zdravlja. Upravlja korisnicima, raspoloženjem, dnevnikom misli, komunikacijom sa terapeutom, zakazivanjem sesija i podsetnicima.

Frontend (Angular) živi u zasebnom repozitorijumu: **[MindCare_frontend](https://github.com/timotijevicz/MindCare_frontend)**.

🔗 **Live API:** [mindcarebackend.up.railway.app](https://mindcarebackend.up.railway.app) · **Live app:** [mindcaree.up.railway.app](https://mindcaree.up.railway.app)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-8-512BD4)
![JWT](https://img.shields.io/badge/Auth-JWT-000000?logo=jsonwebtokens&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-ready-2496ED?logo=docker&logoColor=white)

---

## ✨ Šta API pokriva

| Kontroler | Odgovornost |
|---|---|
| `Auth` | Registracija, prijava, reset lozinke, brisanje naloga (GDPR-stilizovano pravo na brisanje) |
| `Korisnik` | Profili klijenata i terapeuta, javna i administratorska lista terapeuta |
| `Raspolozenje` | Dnevni unosi raspoloženja i statistika po periodu |
| `DnevnikMisli` | Beleške dnevnika, deljenje/ukidanje deljenja sa terapeutom |
| `Sesija` / `Zakazivanje` | Zakazivanje termina, prihvatanje/odbijanje, istorija sesija |
| `Poruka` | Šifrovana razmena poruka klijent ↔ terapeut |
| `Podsetnik` | Dnevni podsetnici i motivaciona poruka |
| `Cilj` | Ciljevi sa koracima i praćenjem napretka |
| `Navika` | Habit tracker sa statistikom ispunjavanja |
| `SOSKontakt` | Kontakt za hitne situacije |
| `EdukativniSadrzaj` | Biblioteka članaka/videa o mentalnom zdravlju |
| `Recenzije` | Recenzije korisnika (uz administratorsko odobravanje) |

**Bezbednost:**
- JWT autentifikacija sa ulogama (Klijent / Terapeut / Administrator)
- Poruke između klijenta i terapeuta se čuvaju AES-šifrovane, sa nasumičnim IV-om po poruci
- Lozinke i JWT ključ nikad nisu u kodu — lokalno preko `dotnet user-secrets`, hostovano preko environment varijabli

---

## 🛠️ Tehnologije

| | |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Baza | PostgreSQL (Npgsql provider) |
| Autentifikacija | ASP.NET Core Identity + JWT Bearer |
| Mapiranje | AutoMapper |
| Dokumentacija | Swagger / OpenAPI (dostupno u Development okruženju) |

---

## 🚀 Pokretanje lokalno

### Opcija A — Docker Compose (preporučeno, najbliže produkciji)

```bash
docker compose up --build
```

Ovo pokreće PostgreSQL i API zajedno, primenjuje migracije automatski pri startu i izlaže API na `http://localhost:8085`. Swagger UI: `http://localhost:8085/swagger`.

### Opcija B — direktno preko `dotnet`

**Preduslovi:** .NET 8 SDK, dostupna PostgreSQL instanca (lokalna ili hostovana).

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=MentalHealthDb;Username=postgres;Password=postgres"
dotnet user-secrets set "Jwt:Key" "<bilo koja duga nasumična vrednost>"
dotnet run --launch-profile https
```

API sluša na `https://localhost:7137` i `http://localhost:5044`.

> Konekcioni string i JWT ključ se namerno ne čuvaju u `appsettings.json` — aplikacija ih traži iz `user-secrets` (lokalno) ili environment varijabli `ConnectionStrings__DefaultConnection` / `Jwt__Key` (hostovano) i **odbija da se pokrene** ako nedostaju, umesto da tiho koristi nesigurnu podrazumevanu vrednost.

---

## 🐘 Migracije

Migracije se primenjuju **automatski pri pokretanju** (`Database.MigrateAsync()` u `Program.cs`) — nije potrebno ručno pokretati `dotnet ef database update` ni lokalno ni na hostovanoj bazi.

Za dodavanje nove migracije nakon izmene modela:

```bash
dotnet ef migrations add NazivMigracije
```

---

## ☁️ Hosting (Railway)

Aplikacija je pripremljena za hosting bez dodatnih izmena koda:

- Port se čita iz `PORT` environment varijable (Railway je sam dodeljuje)
- Konekcija ka bazi prihvata i `DATABASE_URL` u standardnom `postgres://` formatu, pored `ConnectionStrings:DefaultConnection`
- Dozvoljeni CORS origin-i se čitaju iz konfiguracije (`Cors:AllowedOrigins` / `Cors__AllowedOrigins__0`), ne iz koda
- `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)` obezbeđuje da se `DateTime` vrednosti iz JSON-a (bez eksplicitne vremenske zone) ispravno čuvaju na PostgreSQL-u

Potrebne environment varijable na hosting platformi: `DATABASE_URL` (ili `ConnectionStrings__DefaultConnection`), `Jwt__Key`, `Cors__AllowedOrigins__0`.

---

## 🔗 Povezani repozitorijum

Frontend (Angular 19): **[MindCare_frontend](https://github.com/timotijevicz/MindCare_frontend)**
