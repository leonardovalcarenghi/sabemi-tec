using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sabemi.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddContractStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentWebhookEvents",
                table: "PaymentWebhookEvents");

            migrationBuilder.RenameTable(
                name: "PaymentWebhookEvents",
                newName: "PaymentWebhookEvent");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ContractPayment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RetryCount",
                table: "PaymentWebhookEvent",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "PaymentWebhookEvent",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentWebhookEvent",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PaymentWebhookEvent",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ContractId",
                table: "PaymentWebhookEvent",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentWebhookEvent",
                table: "PaymentWebhookEvent",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ContractStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractStatus_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentWebhookEvent_ContractId",
                table: "PaymentWebhookEvent",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractStatus_ContractId",
                table: "ContractStatus",
                column: "ContractId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentWebhookEvent_Contract_ContractId",
                table: "PaymentWebhookEvent",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentWebhookEvent_Contract_ContractId",
                table: "PaymentWebhookEvent");

            migrationBuilder.DropTable(
                name: "ContractStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentWebhookEvent",
                table: "PaymentWebhookEvent");

            migrationBuilder.DropIndex(
                name: "IX_PaymentWebhookEvent_ContractId",
                table: "PaymentWebhookEvent");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "PaymentWebhookEvent");

            migrationBuilder.RenameTable(
                name: "PaymentWebhookEvent",
                newName: "PaymentWebhookEvents");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ContractPayment",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "RetryCount",
                table: "PaymentWebhookEvents",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "PaymentWebhookEvents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentWebhookEvents",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PaymentWebhookEvents",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentWebhookEvents",
                table: "PaymentWebhookEvents",
                column: "Id");
        }
    }
}
