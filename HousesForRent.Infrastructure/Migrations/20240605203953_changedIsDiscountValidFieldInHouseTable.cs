using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HousesForRent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changedIsDiscountValidFieldInHouseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDiscountValid",
                table: "Houses",
                newName: "IsDiscountActivated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDiscountActivated",
                table: "Houses",
                newName: "IsDiscountValid");
        }
    }
}
