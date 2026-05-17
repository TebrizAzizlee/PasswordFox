using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWorkPassServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactInfo_PhoneNumber2",
                table: "Branches",
                newName: "PhoneNumber2");

            migrationBuilder.RenameColumn(
                name: "ContactInfo_PhoneNumber1",
                table: "Branches",
                newName: "PhoneNumber1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber2",
                table: "Branches",
                newName: "ContactInfo_PhoneNumber2");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber1",
                table: "Branches",
                newName: "ContactInfo_PhoneNumber1");
        }
    }
}
