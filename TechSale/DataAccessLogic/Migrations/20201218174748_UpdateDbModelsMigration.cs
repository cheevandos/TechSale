﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLogic.Migrations
{
    public partial class UpdateDbModelsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Notes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "BidTimePrice",
                table: "Bids",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "BidTimePrice",
                table: "Bids");
        }
    }
}
