using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jahez_Task.Migrations
{
    /// <inheritdoc />
    public partial class seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "296cb22a-1f0f-4928-b2ac-0fe7ec31d376", "AQAAAAIAAYagAAAAEJeT+Ngh2D5wyL3RMvNAkbR7gbc2mzIunAk2Xar45vT9lkj94JKC6F5OO6jrXCf2FA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "023f93b7-3c85-4a2c-8289-b8fe0a1de419", "AQAAAAIAAYagAAAAED7rmQFN/aVj6lUxxuVOzag//EppeAjiJknQTTO4PZ9ZMH5CetpvyKEj5lK6Vmf4vQ==" });
        }
    }
}
