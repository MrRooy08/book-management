using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bai1.Migrations
{
    /// <inheritdoc />
    public partial class Fix_RemovePersonId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Persons_PersonId",
                table: "Books");

            // 2. Drop Index
            migrationBuilder.DropIndex(
                name: "IX_Books_PersonId",
                table: "Books");

            // 3. Drop Column
            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
