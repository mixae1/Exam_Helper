using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Exam_Helper.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          /*  migrationBuilder.DropForeignKey(
                name: "atest_pack_fkey",
                table: "aTest");
          */
          /*
            migrationBuilder.DropForeignKey(
                name: "atest_ques_fkey",
                table: "aTest");
          */
          /*
            migrationBuilder.DropIndex(
                name: "IX_aTest_object_id",
                table: "aTest");
          */
            migrationBuilder.AlterColumn<string>(
                name: "proof",
                table: "Question",
                type: "character varying",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying",
                oldMaxLength: 1024);
            /*
            migrationBuilder.AddColumn<bool>(
                name: "isprivate",
                table: "Question",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            */
            /*
            migrationBuilder.AddColumn<bool>(
                name: "isprivate",
                table: "Pack",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            */
            migrationBuilder.AddColumn<int>(
                name: "PackId",
                table: "aTest",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "aTest",
                nullable: true);
            /*
            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "aTest",
                type: "character varying",
                nullable: false,
                defaultValue: "");
            */
            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(nullable: true),
                    QuestionId = table.Column<int>(nullable: false),
                    ServiceInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aTest_PackId",
                table: "aTest",
                column: "PackId");

            migrationBuilder.CreateIndex(
                name: "IX_aTest_QuestionId",
                table: "aTest",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_aTest_Pack_PackId",
                table: "aTest",
                column: "PackId",
                principalTable: "Pack",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_aTest_Question_QuestionId",
                table: "aTest",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aTest_Pack_PackId",
                table: "aTest");

            migrationBuilder.DropForeignKey(
                name: "FK_aTest_Question_QuestionId",
                table: "aTest");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropIndex(
                name: "IX_aTest_PackId",
                table: "aTest");

            migrationBuilder.DropIndex(
                name: "IX_aTest_QuestionId",
                table: "aTest");

            migrationBuilder.DropColumn(
                name: "isprivate",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "isprivate",
                table: "Pack");

            migrationBuilder.DropColumn(
                name: "PackId",
                table: "aTest");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "aTest");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "aTest");

            migrationBuilder.AlterColumn<string>(
                name: "proof",
                table: "Question",
                type: "character varying",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying",
                oldMaxLength: 1024,
                oldNullable: true);
            /*
            migrationBuilder.CreateIndex(
                name: "IX_aTest_object_id",
                table: "aTest",
                column: "object_id");
/*
            migrationBuilder.AddForeignKey(
                name: "atest_pack_fkey",
                table: "aTest",
                column: "object_id",
                principalTable: "Pack",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
*/
/*
            migrationBuilder.AddForeignKey(
                name: "atest_ques_fkey",
                table: "aTest",
                column: "object_id",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);*/
        } 

    }
}
