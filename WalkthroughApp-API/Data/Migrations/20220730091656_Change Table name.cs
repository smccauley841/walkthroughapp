using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalkthroughApp_API.Data.Migrations
{
    public partial class ChangeTablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Topics_TopicId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.RenameColumn(
                name: "TopicId",
                table: "Questions",
                newName: "WalkthroughId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_TopicId",
                table: "Questions",
                newName: "IX_Questions_WalkthroughId");

            migrationBuilder.CreateTable(
                name: "Walkthroughs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WalkthroughName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Walkthroughs", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Walkthroughs_WalkthroughId",
                table: "Questions",
                column: "WalkthroughId",
                principalTable: "Walkthroughs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Walkthroughs_WalkthroughId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Walkthroughs");

            migrationBuilder.RenameColumn(
                name: "WalkthroughId",
                table: "Questions",
                newName: "TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_WalkthroughId",
                table: "Questions",
                newName: "IX_Questions_TopicId");

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TopicName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Topics_TopicId",
                table: "Questions",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
