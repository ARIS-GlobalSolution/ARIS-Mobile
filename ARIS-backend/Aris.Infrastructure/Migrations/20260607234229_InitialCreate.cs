using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aris.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOG_ACOES",
                columns: table => new
                {
                    ID_LOG = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ACAO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOG_ACOES", x => x.ID_LOG);
                });

            migrationBuilder.CreateTable(
                name: "TIPOS_SENSOR",
                columns: table => new
                {
                    ID_TIPO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    UNIDADE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPOS_SENSOR", x => x.ID_TIPO);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    SENHA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "ESTUFAS",
                columns: table => new
                {
                    ID_ESTUFA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LOCALIZACAO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESTUFAS", x => x.ID_ESTUFA);
                    table.ForeignKey(
                        name: "FK_ESTUFAS_USUARIOS_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ALERTAS",
                columns: table => new
                {
                    ID_ALERTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MENSAGEM = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    NIVEL_RISCO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_ESTUFA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALERTAS", x => x.ID_ALERTA);
                    table.ForeignKey(
                        name: "FK_ALERTAS_ESTUFAS_ID_ESTUFA",
                        column: x => x.ID_ESTUFA,
                        principalTable: "ESTUFAS",
                        principalColumn: "ID_ESTUFA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CULTURAS",
                columns: table => new
                {
                    ID_CULTURA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ID_ESTUFA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CULTURAS", x => x.ID_CULTURA);
                    table.ForeignKey(
                        name: "FK_CULTURAS_ESTUFAS_ID_ESTUFA",
                        column: x => x.ID_ESTUFA,
                        principalTable: "ESTUFAS",
                        principalColumn: "ID_ESTUFA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IRRIGACAO",
                columns: table => new
                {
                    ID_IRRIGACAO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    ID_ESTUFA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRRIGACAO", x => x.ID_IRRIGACAO);
                    table.ForeignKey(
                        name: "FK_IRRIGACAO_ESTUFAS_ID_ESTUFA",
                        column: x => x.ID_ESTUFA,
                        principalTable: "ESTUFAS",
                        principalColumn: "ID_ESTUFA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SENSORES",
                columns: table => new
                {
                    ID_SENSOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_TIPO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_ESTUFA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SENSORES", x => x.ID_SENSOR);
                    table.ForeignKey(
                        name: "FK_SENSORES_ESTUFAS_ID_ESTUFA",
                        column: x => x.ID_ESTUFA,
                        principalTable: "ESTUFAS",
                        principalColumn: "ID_ESTUFA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SENSORES_TIPOS_SENSOR_ID_TIPO",
                        column: x => x.ID_TIPO,
                        principalTable: "TIPOS_SENSOR",
                        principalColumn: "ID_TIPO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PARAMETROS_CULTURA",
                columns: table => new
                {
                    ID_PARAMETRO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CULTURA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TEMP_MIN = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    TEMP_MAX = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    UMIDADE_MIN = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    UMIDADE_MAX = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PARAMETROS_CULTURA", x => x.ID_PARAMETRO);
                    table.ForeignKey(
                        name: "FK_PARAMETROS_CULTURA_CULTURAS_ID_CULTURA",
                        column: x => x.ID_CULTURA,
                        principalTable: "CULTURAS",
                        principalColumn: "ID_CULTURA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TELEMETRIA",
                columns: table => new
                {
                    ID_TELEMETRIA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    VALOR = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_SENSOR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Active = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TELEMETRIA", x => x.ID_TELEMETRIA);
                    table.ForeignKey(
                        name: "FK_TELEMETRIA_SENSORES_ID_SENSOR",
                        column: x => x.ID_SENSOR,
                        principalTable: "SENSORES",
                        principalColumn: "ID_SENSOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALERTAS_ID_ESTUFA",
                table: "ALERTAS",
                column: "ID_ESTUFA");

            migrationBuilder.CreateIndex(
                name: "IX_CULTURAS_ID_ESTUFA",
                table: "CULTURAS",
                column: "ID_ESTUFA");

            migrationBuilder.CreateIndex(
                name: "IX_ESTUFAS_ID_USUARIO",
                table: "ESTUFAS",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_IRRIGACAO_ID_ESTUFA",
                table: "IRRIGACAO",
                column: "ID_ESTUFA");

            migrationBuilder.CreateIndex(
                name: "IX_PARAMETROS_CULTURA_ID_CULTURA",
                table: "PARAMETROS_CULTURA",
                column: "ID_CULTURA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SENSORES_ID_ESTUFA",
                table: "SENSORES",
                column: "ID_ESTUFA");

            migrationBuilder.CreateIndex(
                name: "IX_SENSORES_ID_TIPO",
                table: "SENSORES",
                column: "ID_TIPO");

            migrationBuilder.CreateIndex(
                name: "IX_TELEMETRIA_ID_SENSOR",
                table: "TELEMETRIA",
                column: "ID_SENSOR");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_EMAIL",
                table: "USUARIOS",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALERTAS");

            migrationBuilder.DropTable(
                name: "IRRIGACAO");

            migrationBuilder.DropTable(
                name: "LOG_ACOES");

            migrationBuilder.DropTable(
                name: "PARAMETROS_CULTURA");

            migrationBuilder.DropTable(
                name: "TELEMETRIA");

            migrationBuilder.DropTable(
                name: "CULTURAS");

            migrationBuilder.DropTable(
                name: "SENSORES");

            migrationBuilder.DropTable(
                name: "ESTUFAS");

            migrationBuilder.DropTable(
                name: "TIPOS_SENSOR");

            migrationBuilder.DropTable(
                name: "USUARIOS");
        }
    }
}
