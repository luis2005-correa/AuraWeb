using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "variantesproducto",
                schema: "auradb",
                newName: "variantesproducto",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "usuarios",
                schema: "auradb",
                newName: "usuarios",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "roles",
                schema: "auradb",
                newName: "roles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "productos",
                schema: "auradb",
                newName: "productos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "pedidos",
                schema: "auradb",
                newName: "pedidos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "imagenesproducto",
                schema: "auradb",
                newName: "imagenesproducto",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "detallespedidos",
                schema: "auradb",
                newName: "detallespedidos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "categorias",
                schema: "auradb",
                newName: "categorias",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auradb");

            migrationBuilder.RenameTable(
                name: "variantesproducto",
                schema: "public",
                newName: "variantesproducto",
                newSchema: "auradb");

            migrationBuilder.RenameTable(
                name: "usuarios",
                schema: "public",
                newName: "usuarios",
                newSchema: "auradb");

            migrationBuilder.RenameTable(
                name: "roles",
                schema: "public",
                newName: "roles",
                newSchema: "auradb");

            migrationBuilder.RenameTable(
                name: "productos",
                schema: "public",
                newName: "productos",
                newSchema: "auradb");

            migrationBuilder.RenameTable(
                name: "pedidos",
                schema: "public",
                newName: "pedidos",
                newSchema: "auradb");

            migrationBuilder.RenameTable(
                name: "imagenesproducto",
                schema: "public",
                newName: "imagenesproducto",
                newSchema: "auradb");

            migrationBuilder.RenameTable(
                name: "detallespedidos",
                schema: "public",
                newName: "detallespedidos",
                newSchema: "auradb");

            migrationBuilder.RenameTable(
                name: "categorias",
                schema: "public",
                newName: "categorias",
                newSchema: "auradb");
        }
    }
}
