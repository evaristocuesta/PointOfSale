using Microsoft.EntityFrameworkCore.Migrations;

namespace PointOfSale.WebAPI.Migrations
{
    public partial class UserIdentitySeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "cc744f7a-1e53-4a1a-887c-086c650bd37a", 0, "5c030d6d-6db9-4cfe-a7ff-0c2872b5755b", "contact@evaristocuesta.com", true, false, null, "CONTACT@EVARISTOCUESTA.COM", "EVARISTOCUESTA", "AQAAAAEAACcQAAAAEEptI+O8RRQp85Vv40kzdFWUlZGGP+XzL4O8EznpFTSY3JRajaB99L8GhZ8rvrYOJA==", null, false, "5a28dda8-1550-46bc-a4c9-b175f7265f41", false, "evaristocuesta" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cc744f7a-1e53-4a1a-887c-086c650bd37a");
        }
    }
}
