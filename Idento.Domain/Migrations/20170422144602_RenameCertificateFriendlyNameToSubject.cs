using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Idento.Domain.Migrations
{
    public partial class RenameCertificateFriendlyNameToSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FriendlyName",
                schema: "Security",
                table: "Certificates",
                newName: "Subject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                schema: "Security",
                table: "Certificates",
                newName: "FriendlyName");
        }
    }
}
