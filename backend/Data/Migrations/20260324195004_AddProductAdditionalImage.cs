using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAdditionalImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalImages",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalImages",
                table: "Products");
        }
    }
}
