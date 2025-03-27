using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebChatSignalR.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "groupmembers_GroupId_fkey",
                table: "groupmembers");

            migrationBuilder.DropForeignKey(
                name: "groupmembers_UserId_fkey",
                table: "groupmembers");

            migrationBuilder.DropForeignKey(
                name: "groups_CreatorId_fkey",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "messages_GroupId_fkey",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "messages_RoomId_fkey",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "messages_UserId_fkey",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "rooms_CreatorId_fkey",
                table: "rooms");

            migrationBuilder.DropForeignKey(
                name: "rooms_UserId_fkey",
                table: "rooms");

            migrationBuilder.DropForeignKey(
                name: "voicemessages_GroupId_fkey",
                table: "voicemessages");

            migrationBuilder.DropForeignKey(
                name: "voicemessages_RoomId_fkey",
                table: "voicemessages");

            migrationBuilder.DropForeignKey(
                name: "voicemessages_UserId_fkey",
                table: "voicemessages");

            migrationBuilder.DropPrimaryKey(
                name: "voicemessages_pkey",
                table: "voicemessages");

            migrationBuilder.DropPrimaryKey(
                name: "rooms_pkey",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "messages_pkey",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "groups_pkey",
                table: "groups");

            migrationBuilder.DropPrimaryKey(
                name: "groupmembers_pkey",
                table: "groupmembers");

            migrationBuilder.DropPrimaryKey(
                name: "appusers_pkey",
                table: "appusers");

            migrationBuilder.DropIndex(
                name: "appusers_email_key",
                table: "appusers");

            migrationBuilder.DropIndex(
                name: "appusers_normalizedemail_key",
                table: "appusers");

            migrationBuilder.DropIndex(
                name: "appusers_username_key",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "appusers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "appusers");

            migrationBuilder.RenameTable(
                name: "voicemessages",
                newName: "VoiceMessages");

            migrationBuilder.RenameTable(
                name: "rooms",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "messages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "groups",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "groupmembers",
                newName: "GroupMembers");

            migrationBuilder.RenameTable(
                name: "appusers",
                newName: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "VoiceMessages",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VoiceMessages",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_voicemessages_UserId",
                table: "VoiceMessages",
                newName: "IX_VoiceMessages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_voicemessages_RoomId",
                table: "VoiceMessages",
                newName: "IX_VoiceMessages_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_voicemessages_GroupId",
                table: "VoiceMessages",
                newName: "IX_VoiceMessages_GroupId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Rooms",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Rooms",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_rooms_UserId",
                table: "Rooms",
                newName: "IX_Rooms_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_rooms_CreatorId",
                table: "Rooms",
                newName: "IX_Rooms_CreatorId");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "Messages",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "file",
                table: "Messages",
                newName: "File");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Messages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Messages",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_UserId",
                table: "Messages",
                newName: "IX_Messages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_messages_RoomId",
                table: "Messages",
                newName: "IX_Messages_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_messages_GroupId",
                table: "Messages",
                newName: "IX_Messages_GroupId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Groups",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Groups",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_groups_CreatorId",
                table: "Groups",
                newName: "IX_Groups_CreatorId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "GroupMembers",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_groupmembers_UserId",
                table: "GroupMembers",
                newName: "IX_GroupMembers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_groupmembers_GroupId",
                table: "GroupMembers",
                newName: "IX_GroupMembers_GroupId");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "AspNetUsers",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "twofactorenabled",
                table: "AspNetUsers",
                newName: "TwoFactorEnabled");

            migrationBuilder.RenameColumn(
                name: "securitystamp",
                table: "AspNetUsers",
                newName: "SecurityStamp");

            migrationBuilder.RenameColumn(
                name: "phonenumberconfirmed",
                table: "AspNetUsers",
                newName: "PhoneNumberConfirmed");

            migrationBuilder.RenameColumn(
                name: "phonenumber",
                table: "AspNetUsers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "passwordhash",
                table: "AspNetUsers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "normalizedusername",
                table: "AspNetUsers",
                newName: "NormalizedUserName");

            migrationBuilder.RenameColumn(
                name: "normalizedemail",
                table: "AspNetUsers",
                newName: "NormalizedEmail");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AspNetUsers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "lockoutend",
                table: "AspNetUsers",
                newName: "LockoutEnd");

            migrationBuilder.RenameColumn(
                name: "lockoutenabled",
                table: "AspNetUsers",
                newName: "LockoutEnabled");

            migrationBuilder.RenameColumn(
                name: "emailconfirmed",
                table: "AspNetUsers",
                newName: "EmailConfirmed");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "AspNetUsers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "concurrencystamp",
                table: "AspNetUsers",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "avatar",
                table: "AspNetUsers",
                newName: "Avatar");

            migrationBuilder.RenameColumn(
                name: "accessfailedcount",
                table: "AspNetUsers",
                newName: "AccessFailedCount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetUsers",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "appusers_normalizedusername_key",
                table: "AspNetUsers",
                newName: "UserNameIndex");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "VoiceMessages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "RoomId1",
                table: "VoiceMessages",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Rooms",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "UnreadCount",
                table: "Rooms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReported",
                table: "Rooms",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsBlocked",
                table: "Rooms",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Rooms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId1",
                table: "Rooms",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "RoomId1",
                table: "Messages",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReported",
                table: "Groups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsBlocked",
                table: "Groups",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "TwoFactorEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "LockoutEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoiceMessages",
                table: "VoiceMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoiceMessages_RoomId1",
                table: "VoiceMessages",
                column: "RoomId1");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AppUserId",
                table: "Rooms",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AppUserId1",
                table: "Rooms",
                column: "AppUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RoomId1",
                table: "Messages",
                column: "RoomId1");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_AspNetUsers_UserId",
                table: "GroupMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Groups_GroupId",
                table: "GroupMembers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_CreatorId",
                table: "Groups",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Groups_GroupId",
                table: "Messages",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Rooms_RoomId1",
                table: "Messages",
                column: "RoomId1",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_AppUserId",
                table: "Rooms",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_AppUserId1",
                table: "Rooms",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_CreatorId",
                table: "Rooms",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AspNetUsers_UserId",
                table: "Rooms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceMessages_AspNetUsers_UserId",
                table: "VoiceMessages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceMessages_Groups_GroupId",
                table: "VoiceMessages",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceMessages_Rooms_RoomId",
                table: "VoiceMessages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceMessages_Rooms_RoomId1",
                table: "VoiceMessages",
                column: "RoomId1",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_AspNetUsers_UserId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Groups_GroupId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_CreatorId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Groups_GroupId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Rooms_RoomId1",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_AppUserId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_AppUserId1",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_CreatorId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AspNetUsers_UserId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_VoiceMessages_AspNetUsers_UserId",
                table: "VoiceMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_VoiceMessages_Groups_GroupId",
                table: "VoiceMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_VoiceMessages_Rooms_RoomId",
                table: "VoiceMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_VoiceMessages_Rooms_RoomId1",
                table: "VoiceMessages");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoiceMessages",
                table: "VoiceMessages");

            migrationBuilder.DropIndex(
                name: "IX_VoiceMessages_RoomId1",
                table: "VoiceMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AppUserId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AppUserId1",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RoomId1",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupMembers",
                table: "GroupMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "VoiceMessages");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "VoiceMessages",
                newName: "voicemessages");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "rooms");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "messages");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "groups");

            migrationBuilder.RenameTable(
                name: "GroupMembers",
                newName: "groupmembers");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "appusers");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "voicemessages",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "voicemessages",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_VoiceMessages_UserId",
                table: "voicemessages",
                newName: "IX_voicemessages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_VoiceMessages_RoomId",
                table: "voicemessages",
                newName: "IX_voicemessages_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_VoiceMessages_GroupId",
                table: "voicemessages",
                newName: "IX_voicemessages_GroupId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rooms",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "rooms",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_UserId",
                table: "rooms",
                newName: "IX_rooms_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_CreatorId",
                table: "rooms",
                newName: "IX_rooms_CreatorId");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "messages",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "File",
                table: "messages",
                newName: "file");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "messages",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "messages",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_UserId",
                table: "messages",
                newName: "IX_messages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RoomId",
                table: "messages",
                newName: "IX_messages_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_GroupId",
                table: "messages",
                newName: "IX_messages_GroupId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "groups",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "groups",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_CreatorId",
                table: "groups",
                newName: "IX_groups_CreatorId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "groupmembers",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMembers_UserId",
                table: "groupmembers",
                newName: "IX_groupmembers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMembers_GroupId",
                table: "groupmembers",
                newName: "IX_groupmembers_GroupId");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "appusers",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "TwoFactorEnabled",
                table: "appusers",
                newName: "twofactorenabled");

            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                table: "appusers",
                newName: "securitystamp");

            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmed",
                table: "appusers",
                newName: "phonenumberconfirmed");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "appusers",
                newName: "phonenumber");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "appusers",
                newName: "passwordhash");

            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                table: "appusers",
                newName: "normalizedusername");

            migrationBuilder.RenameColumn(
                name: "NormalizedEmail",
                table: "appusers",
                newName: "normalizedemail");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "appusers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LockoutEnd",
                table: "appusers",
                newName: "lockoutend");

            migrationBuilder.RenameColumn(
                name: "LockoutEnabled",
                table: "appusers",
                newName: "lockoutenabled");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                table: "appusers",
                newName: "emailconfirmed");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "appusers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "appusers",
                newName: "concurrencystamp");

            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "appusers",
                newName: "avatar");

            migrationBuilder.RenameColumn(
                name: "AccessFailedCount",
                table: "appusers",
                newName: "accessfailedcount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "appusers",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "UserNameIndex",
                table: "appusers",
                newName: "appusers_normalizedusername_key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "timestamp",
                table: "voicemessages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "rooms",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "UnreadCount",
                table: "rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReported",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBlocked",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "timestamp",
                table: "messages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReported",
                table: "groups",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBlocked",
                table: "groups",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "appusers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "twofactorenabled",
                table: "appusers",
                type: "boolean",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "phonenumberconfirmed",
                table: "appusers",
                type: "boolean",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "normalizedusername",
                table: "appusers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "normalizedemail",
                table: "appusers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "appusers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "lockoutend",
                table: "appusers",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "lockoutenabled",
                table: "appusers",
                type: "boolean",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "emailconfirmed",
                table: "appusers",
                type: "boolean",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "appusers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "accessfailedcount",
                table: "appusers",
                type: "integer",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "appusers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "appusers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "appusers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "appusers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "appusers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "appusers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "appusers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "appusers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "appusers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "appusers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "appusers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "appusers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "appusers",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "voicemessages_pkey",
                table: "voicemessages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "rooms_pkey",
                table: "rooms",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "messages_pkey",
                table: "messages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "groups_pkey",
                table: "groups",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "groupmembers_pkey",
                table: "groupmembers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "appusers_pkey",
                table: "appusers",
                column: "id");

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
                name: "appusers_username_key",
                table: "appusers",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "groupmembers_GroupId_fkey",
                table: "groupmembers",
                column: "GroupId",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "groupmembers_UserId_fkey",
                table: "groupmembers",
                column: "UserId",
                principalTable: "appusers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "groups_CreatorId_fkey",
                table: "groups",
                column: "CreatorId",
                principalTable: "appusers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "messages_GroupId_fkey",
                table: "messages",
                column: "GroupId",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "messages_RoomId_fkey",
                table: "messages",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "messages_UserId_fkey",
                table: "messages",
                column: "UserId",
                principalTable: "appusers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "rooms_CreatorId_fkey",
                table: "rooms",
                column: "CreatorId",
                principalTable: "appusers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "rooms_UserId_fkey",
                table: "rooms",
                column: "UserId",
                principalTable: "appusers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "voicemessages_GroupId_fkey",
                table: "voicemessages",
                column: "GroupId",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "voicemessages_RoomId_fkey",
                table: "voicemessages",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "voicemessages_UserId_fkey",
                table: "voicemessages",
                column: "UserId",
                principalTable: "appusers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
