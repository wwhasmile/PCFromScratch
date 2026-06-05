using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCFromScratch.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coolers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    Tdp = table.Column<int>(type: "integer", nullable: false),
                    IntelSockets = table.Column<string[]>(type: "text[]", nullable: false),
                    AmdSockets = table.Column<string[]>(type: "text[]", nullable: false),
                    FanCount = table.Column<int>(type: "integer", nullable: false),
                    Radius = table.Column<int>(type: "integer", nullable: false),
                    Thickness = table.Column<int>(type: "integer", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coolers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cpus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    Socket = table.Column<string>(type: "text", nullable: false),
                    Tdp = table.Column<int>(type: "integer", nullable: false),
                    RamGen = table.Column<string>(type: "text", nullable: false),
                    RamFrequency = table.Column<int>(type: "integer", nullable: false),
                    Packing = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    MinPrice = table.Column<int>(type: "integer", nullable: false),
                    MaxPrice = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cpus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gpus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    Tdp = table.Column<int>(type: "integer", nullable: false),
                    Length = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    MaxPrice = table.Column<int>(type: "integer", nullable: false),
                    MinPrice = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gpus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalDrives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    MinPrice = table.Column<int>(type: "integer", nullable: false),
                    MaxPrice = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalDrives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motherboards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    Socket = table.Column<string>(type: "text", nullable: false),
                    FormFactor = table.Column<int>(type: "integer", nullable: false),
                    Chipset = table.Column<string>(type: "text", nullable: false),
                    RamGeneration = table.Column<string>(type: "text", nullable: false),
                    RamSlots = table.Column<int>(type: "integer", nullable: false),
                    RamFrequency = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motherboards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Psus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    Power = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    PowerConnector = table.Column<string>(type: "text", nullable: false),
                    FormFactor = table.Column<int>(type: "integer", nullable: false),
                    Modularity = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    MinPrice = table.Column<int>(type: "integer", nullable: false),
                    MaxPrice = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Psus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Submodel = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Sticks = table.Column<int>(type: "integer", nullable: false),
                    Voltage = table.Column<float>(type: "real", nullable: false),
                    Generation = table.Column<string>(type: "text", nullable: false),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    MinPrice = table.Column<int>(type: "integer", nullable: false),
                    MaxPrice = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PsuConnector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    PsuId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsuConnector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsuConnector_Psus_PsuId",
                        column: x => x.PsuId,
                        principalTable: "Psus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShopName = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    CpuId = table.Column<Guid>(type: "uuid", nullable: true),
                    GpuId = table.Column<Guid>(type: "uuid", nullable: true),
                    InternalDriveId = table.Column<Guid>(type: "uuid", nullable: true),
                    PsuId = table.Column<Guid>(type: "uuid", nullable: true),
                    RamId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offer_Cpus_CpuId",
                        column: x => x.CpuId,
                        principalTable: "Cpus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Offer_Gpus_GpuId",
                        column: x => x.GpuId,
                        principalTable: "Gpus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Offer_InternalDrives_InternalDriveId",
                        column: x => x.InternalDriveId,
                        principalTable: "InternalDrives",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Offer_Psus_PsuId",
                        column: x => x.PsuId,
                        principalTable: "Psus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Offer_Rams_RamId",
                        column: x => x.RamId,
                        principalTable: "Rams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CpuId",
                table: "Offer",
                column: "CpuId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_GpuId",
                table: "Offer",
                column: "GpuId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_InternalDriveId",
                table: "Offer",
                column: "InternalDriveId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_PsuId",
                table: "Offer",
                column: "PsuId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_RamId",
                table: "Offer",
                column: "RamId");

            migrationBuilder.CreateIndex(
                name: "IX_PsuConnector_PsuId",
                table: "PsuConnector",
                column: "PsuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coolers");

            migrationBuilder.DropTable(
                name: "Motherboards");

            migrationBuilder.DropTable(
                name: "Offer");

            migrationBuilder.DropTable(
                name: "PsuConnector");

            migrationBuilder.DropTable(
                name: "Cpus");

            migrationBuilder.DropTable(
                name: "Gpus");

            migrationBuilder.DropTable(
                name: "InternalDrives");

            migrationBuilder.DropTable(
                name: "Rams");

            migrationBuilder.DropTable(
                name: "Psus");
        }
    }
}
