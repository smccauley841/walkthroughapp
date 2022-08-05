using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalkthroughApp_API.Data.Migrations
{
    public partial class addjobtitlestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeRoleId",
                table: "Walkthroughs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JobTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTitles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Walkthroughs_EmployeeRoleId",
                table: "Walkthroughs",
                column: "EmployeeRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Walkthroughs_JobTitles_EmployeeRoleId",
                table: "Walkthroughs",
                column: "EmployeeRoleId",
                principalTable: "JobTitles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Walkthroughs_JobTitles_EmployeeRoleId",
                table: "Walkthroughs");

            migrationBuilder.DropTable(
                name: "JobTitles");

            migrationBuilder.DropIndex(
                name: "IX_Walkthroughs_EmployeeRoleId",
                table: "Walkthroughs");

            migrationBuilder.DropColumn(
                name: "EmployeeRoleId",
                table: "Walkthroughs");
        }
    }
}
