using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibriApi.Migrations
{
    /// <inheritdoc />
    public partial class Iniziale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Libri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titolo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Autore = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AnnoPubblicazione = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libri", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Libri",
                columns: new[] { "Id", "AnnoPubblicazione", "Autore", "Titolo" },
                values: new object[,]
                {
                    { 1, null, "Umberto Eco", "Il nome della rosa" },
                    { 2, null, "Primo Levi", "Se questo è un uomo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Libri");
        }
    }
}
