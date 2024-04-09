using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUDPeliculas.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS(Select Id from AspNetRoles where Id = 'ab65662c-c579-4e0c-9543-a9e4638f2dc9')
BEGIN
    INSERT AspNetRoles (Id, [Name], [NormalizedName])
    VALUES ('ab65662c-c579-4e0c-9543-a9e4638f2dc9','admin','ADMIN')
END

");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles Where Id = 'ab65662c-c579-4e0c-9543-a9e4638f2dc9'");
        }
    }
}
