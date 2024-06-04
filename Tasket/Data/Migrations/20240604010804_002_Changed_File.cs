using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasket.Data.Migrations
{
    /// <inheritdoc />
    public partial class _002_Changed_File : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Files_ProfilePictureId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Files_ImageId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Files_UploadId",
                table: "TicketAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "Uploads");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Uploads",
                table: "Uploads",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Uploads_ProfilePictureId",
                table: "AspNetUsers",
                column: "ProfilePictureId",
                principalTable: "Uploads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Uploads_ImageId",
                table: "Companies",
                column: "ImageId",
                principalTable: "Uploads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Uploads_UploadId",
                table: "TicketAttachments",
                column: "UploadId",
                principalTable: "Uploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Uploads_ProfilePictureId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Uploads_ImageId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Uploads_UploadId",
                table: "TicketAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Uploads",
                table: "Uploads");

            migrationBuilder.RenameTable(
                name: "Uploads",
                newName: "Files");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Files_ProfilePictureId",
                table: "AspNetUsers",
                column: "ProfilePictureId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Files_ImageId",
                table: "Companies",
                column: "ImageId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Files_UploadId",
                table: "TicketAttachments",
                column: "UploadId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
