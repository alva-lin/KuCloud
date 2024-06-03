using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuCloud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFolderDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_storage_nodes_folders_parent_id",
                table: "storage_nodes");

            migrationBuilder.AddForeignKey(
                name: "fk_storage_nodes_folders_parent_id",
                table: "storage_nodes",
                column: "parent_id",
                principalTable: "storage_nodes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_storage_nodes_folders_parent_id",
                table: "storage_nodes");

            migrationBuilder.AddForeignKey(
                name: "fk_storage_nodes_folders_parent_id",
                table: "storage_nodes",
                column: "parent_id",
                principalTable: "storage_nodes",
                principalColumn: "id");
        }
    }
}
