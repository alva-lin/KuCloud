using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KuCloud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "storage_nodes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: true),
                    audit_info = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_storage_nodes", x => x.id);
                    table.ForeignKey(
                        name: "fk_storage_nodes_folders_parent_id",
                        column: x => x.parent_id,
                        principalTable: "storage_nodes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "folder_nesting",
                columns: table => new
                {
                    ancestor_id = table.Column<long>(type: "bigint", nullable: false),
                    descendant_id = table.Column<long>(type: "bigint", nullable: false),
                    depth = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_folder_nesting", x => new { x.ancestor_id, x.descendant_id });
                    table.ForeignKey(
                        name: "fk_folder_nesting_folders_ancestor_id",
                        column: x => x.ancestor_id,
                        principalTable: "storage_nodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_folder_nesting_folders_descendant_id",
                        column: x => x.descendant_id,
                        principalTable: "storage_nodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_folder_nesting_descendant_id",
                table: "folder_nesting",
                column: "descendant_id");

            migrationBuilder.CreateIndex(
                name: "ix_storage_nodes_parent_id",
                table: "storage_nodes",
                column: "parent_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "folder_nesting");

            migrationBuilder.DropTable(
                name: "storage_nodes");
        }
    }
}
