using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebChatSignalR.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appusers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    normalizedusername = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    normalizedemail = table.Column<string>(type: "text", nullable: true),
                    emailconfirmed = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    passwordhash = table.Column<string>(type: "text", nullable: true),
                    securitystamp = table.Column<string>(type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true),
                    phonenumber = table.Column<string>(type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    twofactorenabled = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    lockoutend = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    accessfailedcount = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    name = table.Column<string>(type: "text", nullable: false),
                    avatar = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("appusers_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsReported = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BlockedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("groups_pkey", x => x.id);
                    table.ForeignKey(
                        name: "groups_CreatorId_fkey",
                        column: x => x.CreatorId,
                        principalTable: "appusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsReported = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BlockedBy = table.Column<int>(type: "integer", nullable: false),
                    UnreadCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rooms_pkey", x => x.id);
                    table.ForeignKey(
                        name: "rooms_CreatorId_fkey",
                        column: x => x.CreatorId,
                        principalTable: "appusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "rooms_UserId_fkey",
                        column: x => x.UserId,
                        principalTable: "appusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "groupmembers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("groupmembers_pkey", x => x.id);
                    table.ForeignKey(
                        name: "groupmembers_GroupId_fkey",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "groupmembers_UserId_fkey",
                        column: x => x.UserId,
                        principalTable: "appusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: true),
                    file = table.Column<byte[]>(type: "bytea", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("messages_pkey", x => x.id);
                    table.ForeignKey(
                        name: "messages_GroupId_fkey",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "messages_RoomId_fkey",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "messages_UserId_fkey",
                        column: x => x.UserId,
                        principalTable: "appusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "voicemessages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    FilePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("voicemessages_pkey", x => x.id);
                    table.ForeignKey(
                        name: "voicemessages_GroupId_fkey",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "voicemessages_RoomId_fkey",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "voicemessages_UserId_fkey",
                        column: x => x.UserId,
                        principalTable: "appusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "appusers_email_key",
                table: "appusers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "appusers_normalizedemail_key",
                table: "appusers",
                column: "normalizedemail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "appusers_normalizedusername_key",
                table: "appusers",
                column: "normalizedusername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "appusers_username_key",
                table: "appusers",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_groupmembers_GroupId",
                table: "groupmembers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_groupmembers_UserId",
                table: "groupmembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_CreatorId",
                table: "groups",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_GroupId",
                table: "messages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_RoomId",
                table: "messages",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_UserId",
                table: "messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_CreatorId",
                table: "rooms",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_UserId",
                table: "rooms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_voicemessages_GroupId",
                table: "voicemessages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_voicemessages_RoomId",
                table: "voicemessages",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_voicemessages_UserId",
                table: "voicemessages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "groupmembers");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "voicemessages");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "appusers");
        }
    }
}
