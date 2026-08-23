using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MentalHealthApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnonimnaRegistracija = table.Column<bool>(type: "bit", nullable: false),
                    AktivnaNalog = table.Column<bool>(type: "bit", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZadnjaAktivnost = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EdukativniSadrzaj",
                columns: table => new
                {
                    SadrzajId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naslov = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategorija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumObjave = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdukativniSadrzaj", x => x.SadrzajId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ciljevi",
                columns: table => new
                {
                    CiljId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NazivCilja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategorija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcenatNapretka = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciljevi", x => x.CiljId);
                    table.ForeignKey(
                        name: "FK_Ciljevi_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DnevnaRaspolozenja",
                columns: table => new
                {
                    RaspolozenjeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OcenaRaspolozenja = table.Column<int>(type: "int", nullable: false),
                    Emotikon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UzrociStresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Afirmacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KvalitetSna = table.Column<int>(type: "int", nullable: false),
                    KvalitetIshrane = table.Column<int>(type: "int", nullable: false),
                    NivoFizickeAktivnosti = table.Column<int>(type: "int", nullable: false),
                    DatumUnosa = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnevnaRaspolozenja", x => x.RaspolozenjeId);
                    table.ForeignKey(
                        name: "FK_DnevnaRaspolozenja_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DnevnikMisli",
                columns: table => new
                {
                    BeleskaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sadrzaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategorija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deljena = table.Column<bool>(type: "bit", nullable: false),
                    DeljenjeTerapeutId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumAzuriranja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnevnikMisli", x => x.BeleskaId);
                    table.ForeignKey(
                        name: "FK_DnevnikMisli_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KorisnickiProfili",
                columns: table => new
                {
                    ProfilId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OpisProfila = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilnaSlika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferiraniSat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimiMotivacionuPoruku = table.Column<bool>(type: "bit", nullable: false),
                    PrimiPodsetnik = table.Column<bool>(type: "bit", nullable: false),
                    Cilj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumAzuriranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnickiProfili", x => x.ProfilId);
                    table.ForeignKey(
                        name: "FK_KorisnickiProfili_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Navike",
                columns: table => new
                {
                    NavikaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NazivNavike = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategorija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ucestalost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktivna = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Navike", x => x.NavikaId);
                    table.ForeignKey(
                        name: "FK_Navike_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Podsetnici",
                columns: table => new
                {
                    PodsetnikId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tekst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    VremePodsetnika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Podsetnici", x => x.PodsetnikId);
                    table.ForeignKey(
                        name: "FK_Podsetnici_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recenzije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tekst = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Ocena = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recenzije_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sesije",
                columns: table => new
                {
                    SesijaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KlijentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TerapeutId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumSesije = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrajanjeSesijeMinuta = table.Column<int>(type: "int", nullable: false),
                    BeleskeTerapeuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeedbackKlijenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sesije", x => x.SesijaId);
                    table.ForeignKey(
                        name: "FK_Sesije_AspNetUsers_KlijentId",
                        column: x => x.KlijentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sesije_AspNetUsers_TerapeutId",
                        column: x => x.TerapeutId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SOSKontakti",
                columns: table => new
                {
                    SOSKontaktId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImeKontakta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SOSKontakti", x => x.SOSKontaktId);
                    table.ForeignKey(
                        name: "FK_SOSKontakti_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terapeuti",
                columns: table => new
                {
                    TerapeutId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Zvanje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Licenca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpisPrakse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dostupan = table.Column<bool>(type: "bit", nullable: false),
                    SatnicaKonzultacije = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatumRegistracije = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terapeuti", x => x.TerapeutId);
                    table.ForeignKey(
                        name: "FK_Terapeuti_AspNetUsers_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KoraciCiljeva",
                columns: table => new
                {
                    KorakId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CiljId = table.Column<int>(type: "int", nullable: false),
                    OpisKoraka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zavrsen = table.Column<bool>(type: "bit", nullable: false),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoraciCiljeva", x => x.KorakId);
                    table.ForeignKey(
                        name: "FK_KoraciCiljeva_Ciljevi_CiljId",
                        column: x => x.CiljId,
                        principalTable: "Ciljevi",
                        principalColumn: "CiljId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracenjaNavika",
                columns: table => new
                {
                    PracenjeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NavikaId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Zavrseno = table.Column<bool>(type: "bit", nullable: false),
                    Komentar = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracenjaNavika", x => x.PracenjeId);
                    table.ForeignKey(
                        name: "FK_PracenjaNavika_Navike_NavikaId",
                        column: x => x.NavikaId,
                        principalTable: "Navike",
                        principalColumn: "NavikaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Poruke",
                columns: table => new
                {
                    PorukaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PosiljaocId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PrimalacId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sadrzaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SadrzajSifrovane = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Procitana = table.Column<bool>(type: "bit", nullable: false),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumCitanja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SesijaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poruke", x => x.PorukaId);
                    table.ForeignKey(
                        name: "FK_Poruke_AspNetUsers_PosiljaocId",
                        column: x => x.PosiljaocId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Poruke_AspNetUsers_PrimalacId",
                        column: x => x.PrimalacId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Poruke_Sesije_SesijaId",
                        column: x => x.SesijaId,
                        principalTable: "Sesije",
                        principalColumn: "SesijaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZakazivanjaSesija",
                columns: table => new
                {
                    ZakazivanjeSesijeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KlijentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TerapeutId = table.Column<int>(type: "int", nullable: false),
                    DatumZakazane = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZakazivanjaSesija", x => x.ZakazivanjeSesijeId);
                    table.ForeignKey(
                        name: "FK_ZakazivanjaSesija_AspNetUsers_KlijentId",
                        column: x => x.KlijentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ZakazivanjaSesija_Terapeuti_TerapeutId",
                        column: x => x.TerapeutId,
                        principalTable: "Terapeuti",
                        principalColumn: "TerapeutId");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Administrator", "ADMINISTRATOR" },
                    { "2", null, "Terapeut", "TERAPEUT" },
                    { "3", null, "Klijent", "KLIJENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ciljevi_KorisnikId",
                table: "Ciljevi",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_DnevnaRaspolozenja_KorisnikId",
                table: "DnevnaRaspolozenja",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_DnevnikMisli_KorisnikId",
                table: "DnevnikMisli",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_KoraciCiljeva_CiljId",
                table: "KoraciCiljeva",
                column: "CiljId");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnickiProfili_KorisnikId",
                table: "KorisnickiProfili",
                column: "KorisnikId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Navike_KorisnikId",
                table: "Navike",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Podsetnici_KorisnikId",
                table: "Podsetnici",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_PosiljaocId",
                table: "Poruke",
                column: "PosiljaocId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_PrimalacId",
                table: "Poruke",
                column: "PrimalacId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_SesijaId",
                table: "Poruke",
                column: "SesijaId");

            migrationBuilder.CreateIndex(
                name: "IX_PracenjaNavika_NavikaId",
                table: "PracenjaNavika",
                column: "NavikaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzije_KorisnikId",
                table: "Recenzije",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Sesije_KlijentId",
                table: "Sesije",
                column: "KlijentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sesije_TerapeutId",
                table: "Sesije",
                column: "TerapeutId");

            migrationBuilder.CreateIndex(
                name: "IX_SOSKontakti_KorisnikId",
                table: "SOSKontakti",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Terapeuti_KorisnikId",
                table: "Terapeuti",
                column: "KorisnikId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZakazivanjaSesija_KlijentId",
                table: "ZakazivanjaSesija",
                column: "KlijentId");

            migrationBuilder.CreateIndex(
                name: "IX_ZakazivanjaSesija_TerapeutId",
                table: "ZakazivanjaSesija",
                column: "TerapeutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DnevnaRaspolozenja");

            migrationBuilder.DropTable(
                name: "DnevnikMisli");

            migrationBuilder.DropTable(
                name: "EdukativniSadrzaj");

            migrationBuilder.DropTable(
                name: "KoraciCiljeva");

            migrationBuilder.DropTable(
                name: "KorisnickiProfili");

            migrationBuilder.DropTable(
                name: "Podsetnici");

            migrationBuilder.DropTable(
                name: "Poruke");

            migrationBuilder.DropTable(
                name: "PracenjaNavika");

            migrationBuilder.DropTable(
                name: "Recenzije");

            migrationBuilder.DropTable(
                name: "SOSKontakti");

            migrationBuilder.DropTable(
                name: "ZakazivanjaSesija");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Ciljevi");

            migrationBuilder.DropTable(
                name: "Sesije");

            migrationBuilder.DropTable(
                name: "Navike");

            migrationBuilder.DropTable(
                name: "Terapeuti");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
