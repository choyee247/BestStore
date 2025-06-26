using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTBToCancle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Products");
        }
    }
}
