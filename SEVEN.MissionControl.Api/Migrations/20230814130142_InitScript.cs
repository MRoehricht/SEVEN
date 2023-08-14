using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEVEN.MissionControl.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Probes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    MeasurementsType = table.Column<int>(type: "integer", nullable: false),
                    SendingIntervalMinutes = table.Column<int>(type: "integer", nullable: false),
                    LastContact = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Probes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rovers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rovers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProbeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Measurements_Probes_ProbeId",
                        column: x => x.ProbeId,
                        principalTable: "Probes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoverTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    RoverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Command = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StatusInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoverTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoverTasks_Rovers_RoverId",
                        column: x => x.RoverId,
                        principalTable: "Rovers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_ProbeId",
                table: "Measurements",
                column: "ProbeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoverTasks_RoverId",
                table: "RoverTasks",
                column: "RoverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropTable(
                name: "RoverTasks");

            migrationBuilder.DropTable(
                name: "Probes");

            migrationBuilder.DropTable(
                name: "Rovers");
        }
    }
}
