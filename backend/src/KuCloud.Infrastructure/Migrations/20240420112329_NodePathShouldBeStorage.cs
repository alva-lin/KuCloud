using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuCloud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NodePathShouldBeStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "path",
                table: "storage_node",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "path",
                table: "storage_node");
        }
    }
}
