using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Aura.Migrations
{
    /// <inheritdoc />
    public partial class ResetAPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auradb");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "categorias",
                schema: "auradb",
                columns: table => new
                {
                    categoriaID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__F353C1C5F92839EF", x => x.categoriaID);
                });

            migrationBuilder.CreateTable(
                name: "Product_Images",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    image_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    image_url_2 = table.Column<string>(type: "text", nullable: true),
                    precio = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "auradb",
                columns: table => new
                {
                    rolid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__F92302D1B35F2F7F", x => x.rolid);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                schema: "auradb",
                columns: table => new
                {
                    ProductoID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", unicode: false, maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Precio = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CategoriaID = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__A430AE83F05C551B", x => x.ProductoID);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias",
                        column: x => x.CategoriaID,
                        principalSchema: "auradb",
                        principalTable: "categorias",
                        principalColumn: "categoriaID");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                schema: "auradb",
                columns: table => new
                {
                    usuarioid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", unicode: false, maxLength: 150, nullable: false),
                    passwordhash = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    telefono = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    direccion = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: true),
                    rolid = table.Column<int>(type: "integer", nullable: false),
                    fecharegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__2B3DE798E3925673", x => x.usuarioid);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles",
                        column: x => x.rolid,
                        principalSchema: "auradb",
                        principalTable: "roles",
                        principalColumn: "rolid");
                });

            migrationBuilder.CreateTable(
                name: "imagenesproducto",
                schema: "auradb",
                columns: table => new
                {
                    ImagenID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoID = table.Column<int>(type: "integer", nullable: false),
                    UrlImagen = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Imagenes__0C7D20D786CE4664", x => x.ImagenID);
                    table.ForeignKey(
                        name: "FK_Imagenes_Productos",
                        column: x => x.ProductoID,
                        principalSchema: "auradb",
                        principalTable: "productos",
                        principalColumn: "ProductoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "variantesproducto",
                schema: "auradb",
                columns: table => new
                {
                    varianteID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    productoID = table.Column<int>(type: "integer", nullable: false),
                    talla = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    color = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Variante__FE18F96B38FB7B30", x => x.varianteID);
                    table.ForeignKey(
                        name: "FK_Variantes_Productos",
                        column: x => x.productoID,
                        principalSchema: "auradb",
                        principalTable: "productos",
                        principalColumn: "ProductoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                schema: "auradb",
                columns: table => new
                {
                    PedidoID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioID = table.Column<int>(type: "integer", nullable: false),
                    FechaPedido = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    DireccionEnvio = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    MetodoPago = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pedidos__09BA1410833E652C", x => x.PedidoID);
                    table.ForeignKey(
                        name: "FK_Pedidos_Usuarios",
                        column: x => x.UsuarioID,
                        principalSchema: "auradb",
                        principalTable: "usuarios",
                        principalColumn: "usuarioid");
                });

            migrationBuilder.CreateTable(
                name: "detallespedidos",
                schema: "auradb",
                columns: table => new
                {
                    DetalleID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoID = table.Column<int>(type: "integer", nullable: false),
                    VarianteID = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Detalles__6E19D6FA4075F2C3", x => x.DetalleID);
                    table.ForeignKey(
                        name: "FK_Detalles_Pedidos",
                        column: x => x.PedidoID,
                        principalSchema: "auradb",
                        principalTable: "pedidos",
                        principalColumn: "PedidoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Detalles_Variantes",
                        column: x => x.VarianteID,
                        principalSchema: "auradb",
                        principalTable: "variantesproducto",
                        principalColumn: "varianteID");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Categori__75E3EFCF9ED487FE",
                schema: "auradb",
                table: "categorias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_detallespedidos_PedidoID",
                schema: "auradb",
                table: "detallespedidos",
                column: "PedidoID");

            migrationBuilder.CreateIndex(
                name: "IX_detallespedidos_VarianteID",
                schema: "auradb",
                table: "detallespedidos",
                column: "VarianteID");

            migrationBuilder.CreateIndex(
                name: "IX_imagenesproducto_ProductoID",
                schema: "auradb",
                table: "imagenesproducto",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_UsuarioID",
                schema: "auradb",
                table: "pedidos",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_productos_CategoriaID",
                schema: "auradb",
                table: "productos",
                column: "CategoriaID");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__75E3EFCFB9733A1F",
                schema: "auradb",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rolid",
                schema: "auradb",
                table: "usuarios",
                column: "rolid");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuarios__A9D10534E139ABDA",
                schema: "auradb",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_variantesproducto_productoID",
                schema: "auradb",
                table: "variantesproducto",
                column: "productoID");

            migrationBuilder.CreateIndex(
                name: "UQ__Variante__CA1FD3C5C7A30CCD",
                schema: "auradb",
                table: "variantesproducto",
                column: "sku",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detallespedidos",
                schema: "auradb");

            migrationBuilder.DropTable(
                name: "imagenesproducto",
                schema: "auradb");

            migrationBuilder.DropTable(
                name: "Product_Images",
                schema: "public");

            migrationBuilder.DropTable(
                name: "pedidos",
                schema: "auradb");

            migrationBuilder.DropTable(
                name: "variantesproducto",
                schema: "auradb");

            migrationBuilder.DropTable(
                name: "usuarios",
                schema: "auradb");

            migrationBuilder.DropTable(
                name: "productos",
                schema: "auradb");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "auradb");

            migrationBuilder.DropTable(
                name: "categorias",
                schema: "auradb");
        }
    }
}
