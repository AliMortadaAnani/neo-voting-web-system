using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovernmentSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class Change_GovernorateId_to_Governorate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Citizen_GovernorateId",
                table: "Citizens");

            migrationBuilder.RenameColumn(
                name: "GovernorateId",
                table: "Citizens",
                newName: "Governorate");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Citizen_Governorate",
                table: "Citizens",
                sql: "([Governorate] IN (1, 2, 3, 4, 5) )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Citizen_Governorate",
                table: "Citizens");

            migrationBuilder.RenameColumn(
                name: "Governorate",
                table: "Citizens",
                newName: "GovernorateId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Citizen_GovernorateId",
                table: "Citizens",
                sql: "([GovernorateId] IN (1, 2, 3, 4, 5) )");
        }
    }
}
