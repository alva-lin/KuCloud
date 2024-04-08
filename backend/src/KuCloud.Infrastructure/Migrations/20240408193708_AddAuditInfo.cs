using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuCloud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "audit_info",
                table: "contributors",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "audit_info",
                table: "contributors");
        }
    }
}
