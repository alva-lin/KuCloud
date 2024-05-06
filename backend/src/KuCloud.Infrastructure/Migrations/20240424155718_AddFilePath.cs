using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuCloud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "storage_nodes",
                newName: "path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "path",
                table: "storage_nodes",
                newName: "content_type");
        }
    }
}
