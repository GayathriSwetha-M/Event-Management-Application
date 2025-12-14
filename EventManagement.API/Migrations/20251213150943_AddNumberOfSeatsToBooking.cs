using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberOfSeatsToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfSeats",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfSeats",
                table: "Bookings");
        }
    }
}
