using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Idento.Domain.Migrations
{
    public partial class AddCertificateValidFromToFriendlyName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FriendlyName",
                schema: "Security",
                table: "Certificates",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "Security",
                table: "Certificates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "Security",
                table: "Certificates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendlyName",
                schema: "Security",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "Security",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "Security",
                table: "Certificates");
        }
    }
}
