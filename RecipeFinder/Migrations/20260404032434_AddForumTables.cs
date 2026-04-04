using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFinder.Migrations
{
    /// <inheritdoc />
    public partial class AddForumTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForumFlairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ColorHex = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumFlairs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForumPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ForumFlairId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumPosts_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumPosts_ForumFlairs_ForumFlairId",
                        column: x => x.ForumFlairId,
                        principalTable: "ForumFlairs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ForumPosts_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ForumComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ForumPostId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentCommentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumComments_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumComments_ForumComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "ForumComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ForumComments_ForumPosts_ForumPostId",
                        column: x => x.ForumPostId,
                        principalTable: "ForumPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumPostVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ForumPostId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumPostVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumPostVotes_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumPostVotes_ForumPosts_ForumPostId",
                        column: x => x.ForumPostId,
                        principalTable: "ForumPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumCommentVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ForumCommentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumCommentVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumCommentVotes_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumCommentVotes_ForumComments_ForumCommentId",
                        column: x => x.ForumCommentId,
                        principalTable: "ForumComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumComments_CustomerId",
                table: "ForumComments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumComments_ForumPostId",
                table: "ForumComments",
                column: "ForumPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumComments_ParentCommentId",
                table: "ForumComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumCommentVotes_CustomerId_ForumCommentId",
                table: "ForumCommentVotes",
                columns: new[] { "CustomerId", "ForumCommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumCommentVotes_ForumCommentId",
                table: "ForumCommentVotes",
                column: "ForumCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_CustomerId",
                table: "ForumPosts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_ForumFlairId",
                table: "ForumPosts",
                column: "ForumFlairId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_RecipeId",
                table: "ForumPosts",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPostVotes_CustomerId_ForumPostId",
                table: "ForumPostVotes",
                columns: new[] { "CustomerId", "ForumPostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForumPostVotes_ForumPostId",
                table: "ForumPostVotes",
                column: "ForumPostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumCommentVotes");

            migrationBuilder.DropTable(
                name: "ForumPostVotes");

            migrationBuilder.DropTable(
                name: "ForumComments");

            migrationBuilder.DropTable(
                name: "ForumPosts");

            migrationBuilder.DropTable(
                name: "ForumFlairs");
        }
    }
}
