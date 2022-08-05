using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalkthroughApp_API.Data.Migrations
{
    public partial class ChangeTopicToWalkthrough : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TopicName",
                table: "Walkthroughs",
                newName: "WalkthroughName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalkthroughName",
                table: "Walkthroughs",
                newName: "TopicName");
        }
    }
}
