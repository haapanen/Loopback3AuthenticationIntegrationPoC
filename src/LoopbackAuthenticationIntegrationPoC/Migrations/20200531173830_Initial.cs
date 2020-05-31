using Microsoft.EntityFrameworkCore.Migrations;

namespace LoopbackAuthenticationIntegrationPoC.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "acl",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    model = table.Column<string>(nullable: true),
                    property = table.Column<string>(nullable: true),
                    accessType = table.Column<string>(nullable: true),
                    permission = table.Column<string>(nullable: true),
                    principalType = table.Column<string>(nullable: true),
                    principalId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    realm = table.Column<string>(nullable: true),
                    username = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: false),
                    email = table.Column<string>(nullable: false),
                    emailVerified = table.Column<bool>(nullable: true),
                    verificationToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accesstoken",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    ttl = table.Column<int>(nullable: false),
                    scopes = table.Column<string>(nullable: true),
                    created = table.Column<double>(nullable: false),
                    userId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accesstoken", x => x.id);
                    table.ForeignKey(
                        name: "FK_accesstoken_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accesstoken_userId",
                table: "accesstoken",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accesstoken");

            migrationBuilder.DropTable(
                name: "acl");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
