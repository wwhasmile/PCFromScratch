using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCFromScratch.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddBenchmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Rams");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Psus");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "InternalDrives");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Gpus");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Cpus");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Rams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Psus",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CoolerId",
                table: "Offer",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MotherboardRenamedForOmnissiahId",
                table: "Offer",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasM2Slot",
                table: "Motherboards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Motherboards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPrice",
                table: "Motherboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinPrice",
                table: "Motherboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "InternalDrives",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Gpus",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Cpus",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Coolers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPrice",
                table: "Coolers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinPrice",
                table: "Coolers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CpuBenchmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpuBenchmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GpuBenchmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpuBenchmarks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CoolerId",
                table: "Offer",
                column: "CoolerId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_MotherboardRenamedForOmnissiahId",
                table: "Offer",
                column: "MotherboardRenamedForOmnissiahId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Coolers_CoolerId",
                table: "Offer",
                column: "CoolerId",
                principalTable: "Coolers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Motherboards_MotherboardRenamedForOmnissiahId",
                table: "Offer",
                column: "MotherboardRenamedForOmnissiahId",
                principalTable: "Motherboards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Coolers_CoolerId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Motherboards_MotherboardRenamedForOmnissiahId",
                table: "Offer");

            migrationBuilder.DropTable(
                name: "CpuBenchmarks");

            migrationBuilder.DropTable(
                name: "GpuBenchmarks");

            migrationBuilder.DropIndex(
                name: "IX_Offer_CoolerId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_MotherboardRenamedForOmnissiahId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Rams");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Psus");

            migrationBuilder.DropColumn(
                name: "CoolerId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "MotherboardRenamedForOmnissiahId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "HasM2Slot",
                table: "Motherboards");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Motherboards");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "Motherboards");

            migrationBuilder.DropColumn(
                name: "MinPrice",
                table: "Motherboards");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "InternalDrives");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Gpus");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Cpus");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Coolers");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "Coolers");

            migrationBuilder.DropColumn(
                name: "MinPrice",
                table: "Coolers");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Rams",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Psus",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "InternalDrives",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Gpus",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Cpus",
                type: "bytea",
                nullable: true);
        }
    }
}
