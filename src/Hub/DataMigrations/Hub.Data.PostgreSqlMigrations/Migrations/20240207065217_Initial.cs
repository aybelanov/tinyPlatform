using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

#nullable disable

namespace Hub.Data.PostgreSqlMigrations.Migrations
{
   /// <inheritdoc />
   public partial class Initial : Migration
   {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
         migrationBuilder.CreateTable(
             name: "ActivityLogs",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ActivityLogTypeId = table.Column<long>(type: "bigint", nullable: false),
                EntityId = table.Column<long>(type: "bigint", nullable: true),
                EntityName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                SubjectId = table.Column<long>(type: "bigint", nullable: false),
                SubjectName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                Comment = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IpAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ActivityLogs", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "ActivityLogTypes",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SystemKeyword = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Enabled = table.Column<bool>(type: "boolean", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ActivityLogTypes", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "AddressAttributes",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                AttributeControlTypeId = table.Column<int>(type: "integer", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                AttributeControlType = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_AddressAttributes", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "AddressAttributeValues",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AddressAttributeId = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                IsPreSelected = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_AddressAttributeValues", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Countries",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                AllowsBilling = table.Column<bool>(type: "boolean", nullable: false),
                AllowsShipping = table.Column<bool>(type: "boolean", nullable: false),
                TwoLetterIsoCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                ThreeLetterIsoCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                NumericIsoCode = table.Column<int>(type: "integer", nullable: false),
                SubjectToVat = table.Column<bool>(type: "boolean", nullable: false),
                Published = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Countries", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Currencies",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CurrencyCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                Rate = table.Column<decimal>(type: "numeric(12,5)", precision: 12, scale: 5, nullable: false),
                DisplayLocale = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                CustomFormatting = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Published = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                RoundingTypeId = table.Column<int>(type: "integer", nullable: false),
                RoundingType = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Currencies", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Downloads",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                DownloadGuid = table.Column<Guid>(type: "uuid", nullable: false),
                UseDownloadUrl = table.Column<bool>(type: "boolean", nullable: false),
                DownloadUrl = table.Column<string>(type: "text", nullable: true),
                DownloadBinary = table.Column<byte[]>(type: "bytea", nullable: true),
                ContentType = table.Column<string>(type: "text", nullable: true),
                Filename = table.Column<string>(type: "text", nullable: true),
                Extension = table.Column<string>(type: "text", nullable: true),
                IsNew = table.Column<bool>(type: "boolean", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Downloads", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "EmailAccounts",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                Host = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Port = table.Column<int>(type: "integer", nullable: false),
                Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                EnableSsl = table.Column<bool>(type: "boolean", nullable: false),
                UseDefaultCredentials = table.Column<bool>(type: "boolean", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_EmailAccounts", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "ForumGroups",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ForumGroups", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "GdprConsents",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Message = table.Column<string>(type: "text", nullable: false),
                IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                RequiredMessage = table.Column<string>(type: "text", nullable: true),
                DisplayDuringRegistration = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOnUserInfoPage = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_GdprConsents", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "GdprLogs",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                ConsentId = table.Column<long>(type: "bigint", nullable: false),
                UserInfo = table.Column<string>(type: "text", nullable: true),
                RequestTypeId = table.Column<int>(type: "integer", nullable: false),
                RequestDetails = table.Column<string>(type: "text", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                RequestType = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_GdprLogs", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "GenericAttributes",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EntityId = table.Column<long>(type: "bigint", nullable: false),
                KeyGroup = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                Key = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                Value = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                CreatedOrUpdatedDateUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_GenericAttributes", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Languages",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                LanguageCulture = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                UniqueSeoCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                FlagImageFileName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Rtl = table.Column<bool>(type: "boolean", nullable: false),
                DefaultCurrencyId = table.Column<long>(type: "bigint", nullable: false),
                Published = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Languages", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Logs",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                LogLevelId = table.Column<int>(type: "integer", nullable: false),
                ShortMessage = table.Column<string>(type: "text", nullable: true),
                FullMessage = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                IpAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                EntityId = table.Column<long>(type: "bigint", nullable: true),
                EntityName = table.Column<string>(type: "text", nullable: true),
                PageUrl = table.Column<string>(type: "text", nullable: true),
                ReferrerUrl = table.Column<string>(type: "text", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LogLevel = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Logs", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "MeasureDimensions",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                SystemKeyword = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Ratio = table.Column<decimal>(type: "numeric(12,5)", precision: 12, scale: 5, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_MeasureDimensions", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "MeasureWeights",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                SystemKeyword = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Ratio = table.Column<decimal>(type: "numeric(12,5)", precision: 12, scale: 5, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_MeasureWeights", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "MessageTemplates",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                BccEmailAddresses = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Subject = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                Body = table.Column<string>(type: "text", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                DelayBeforeSend = table.Column<int>(type: "integer", nullable: true),
                DelayPeriodId = table.Column<int>(type: "integer", nullable: false),
                AttachedDownloadId = table.Column<long>(type: "bigint", nullable: false),
                EmailAccountId = table.Column<long>(type: "bigint", nullable: false),
                DelayPeriod = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_MessageTemplates", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "NewsComments",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CommentTitle = table.Column<string>(type: "text", nullable: true),
                CommentText = table.Column<string>(type: "text", nullable: true),
                NewsItemId = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_NewsComments", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "NewsItems",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                LanguageId = table.Column<long>(type: "bigint", nullable: false),
                Title = table.Column<string>(type: "text", nullable: true),
                Short = table.Column<string>(type: "text", nullable: true),
                Full = table.Column<string>(type: "text", nullable: true),
                Published = table.Column<bool>(type: "boolean", nullable: false),
                StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EndDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                AllowComments = table.Column<bool>(type: "boolean", nullable: false),
                MetaKeywords = table.Column<string>(type: "text", nullable: true),
                MetaDescription = table.Column<string>(type: "text", nullable: true),
                MetaTitle = table.Column<string>(type: "text", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_NewsItems", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "NewsLetterSubscriptions",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                NewsLetterSubscriptionGuid = table.Column<Guid>(type: "uuid", nullable: false),
                Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Active = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_NewsLetterSubscriptions", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "PermissionRecords",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_PermissionRecords", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Pictures",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MimeType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                SeoFilename = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                AltAttribute = table.Column<string>(type: "text", nullable: true),
                TitleAttribute = table.Column<string>(type: "text", nullable: true),
                IsNew = table.Column<bool>(type: "boolean", nullable: false),
                VirtualPath = table.Column<string>(type: "text", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Pictures", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "QueuedEmails",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PriorityId = table.Column<int>(type: "integer", nullable: false),
                From = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                FromName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                To = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ToName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ReplyTo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ReplyToName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CC = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Bcc = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Body = table.Column<string>(type: "text", nullable: true),
                AttachmentFilePath = table.Column<string>(type: "text", nullable: true),
                AttachmentFileName = table.Column<string>(type: "text", nullable: true),
                AttachedDownloadId = table.Column<long>(type: "bigint", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DontSendBeforeDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                SentTries = table.Column<int>(type: "integer", nullable: false),
                SentOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EmailAccountId = table.Column<long>(type: "bigint", nullable: false),
                Priority = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_QueuedEmails", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "ScheduleTasks",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                Seconds = table.Column<int>(type: "integer", nullable: false),
                Type = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                LastEnabledUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Enabled = table.Column<bool>(type: "boolean", nullable: false),
                StopOnError = table.Column<bool>(type: "boolean", nullable: false),
                LastStartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastEndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastSuccessUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ScheduleTasks", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "SearchTerms",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Keyword = table.Column<string>(type: "text", nullable: true),
                Count = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_SearchTerms", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Settings",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Value = table.Column<string>(type: "character varying(6000)", maxLength: 6000, nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Settings", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "StateProvinces",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CountryId = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Abbreviation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Published = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_StateProvinces", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "TopicTemplates",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                ViewPath = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_TopicTemplates", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "UrlRecords",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EntityId = table.Column<long>(type: "bigint", nullable: false),
                EntityName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                Slug = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                LanguageId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UrlRecords", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "UserAttributes",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                AttributeControlTypeId = table.Column<int>(type: "integer", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                AttributeControlType = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserAttributes", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "UserRoles",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Active = table.Column<bool>(type: "boolean", nullable: false),
                IsSystemRole = table.Column<bool>(type: "boolean", nullable: false),
                SystemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                EnablePasswordLifetime = table.Column<bool>(type: "boolean", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserRoles", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Users",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserGuid = table.Column<Guid>(type: "uuid", nullable: false),
                Username = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                Email = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                EmailToRevalidate = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                AdminComment = table.Column<string>(type: "text", nullable: true),
                AffiliateId = table.Column<long>(type: "bigint", nullable: false),
                RequireReLogin = table.Column<bool>(type: "boolean", nullable: false),
                FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                CannotLoginUntilDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                IsSystemAccount = table.Column<bool>(type: "boolean", nullable: false),
                SystemName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                LastIpAddress = table.Column<string>(type: "text", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastLoginUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastActivityUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                BillingAddressId = table.Column<long>(type: "bigint", nullable: true),
                ShippingAddressId = table.Column<long>(type: "bigint", nullable: true),
                DeviceCountLimit = table.Column<int>(type: "integer", nullable: false),
                AvatarPictureId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Users", x => x.Id);
             });

         migrationBuilder.CreateTable(
             name: "Forums",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ForumGroupId = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                NumTopics = table.Column<int>(type: "integer", nullable: false),
                NumPosts = table.Column<int>(type: "integer", nullable: false),
                LastTopicId = table.Column<long>(type: "bigint", nullable: false),
                LastPostId = table.Column<long>(type: "bigint", nullable: false),
                LastPostUserId = table.Column<long>(type: "bigint", nullable: false),
                LastPostTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Forums", x => x.Id);
                table.ForeignKey(
                       name: "FK_Forums_ForumGroups_ForumGroupId",
                       column: x => x.ForumGroupId,
                       principalTable: "ForumGroups",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "BlogPosts",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                LanguageId = table.Column<long>(type: "bigint", nullable: false),
                IncludeInSitemap = table.Column<bool>(type: "boolean", nullable: false),
                Title = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                Body = table.Column<string>(type: "text", nullable: false),
                BodyOverview = table.Column<string>(type: "text", nullable: true),
                AllowComments = table.Column<bool>(type: "boolean", nullable: false),
                Tags = table.Column<string>(type: "text", nullable: true),
                StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EndDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                MetaKeywords = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                MetaDescription = table.Column<string>(type: "text", nullable: true),
                MetaTitle = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_BlogPosts", x => x.Id);
                table.ForeignKey(
                       name: "FK_BlogPosts_Languages_LanguageId",
                       column: x => x.LanguageId,
                       principalTable: "Languages",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "LocaleStringResources",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                LanguageId = table.Column<long>(type: "bigint", nullable: false),
                ResourceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ResourceValue = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_LocaleStringResources", x => x.Id);
                table.ForeignKey(
                       name: "FK_LocaleStringResources_Languages_LanguageId",
                       column: x => x.LanguageId,
                       principalTable: "Languages",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "LocalizedProperties",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EntityId = table.Column<long>(type: "bigint", nullable: false),
                LanguageId = table.Column<long>(type: "bigint", nullable: false),
                LocaleKeyGroup = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                LocaleKey = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                LocaleValue = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_LocalizedProperties", x => x.Id);
                table.ForeignKey(
                       name: "FK_LocalizedProperties_Languages_LanguageId",
                       column: x => x.LanguageId,
                       principalTable: "Languages",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "Polls",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                LanguageId = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                SystemKeyword = table.Column<string>(type: "text", nullable: true),
                Published = table.Column<bool>(type: "boolean", nullable: false),
                ShowOnHomepage = table.Column<bool>(type: "boolean", nullable: false),
                AllowGuestsToVote = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EndDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Polls", x => x.Id);
                table.ForeignKey(
                       name: "FK_Polls_Languages_LanguageId",
                       column: x => x.LanguageId,
                       principalTable: "Languages",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "PictureBinaries",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                BinaryData = table.Column<byte[]>(type: "bytea", nullable: true),
                PictureId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_PictureBinaries", x => x.Id);
                table.ForeignKey(
                       name: "FK_PictureBinaries_Pictures_PictureId",
                       column: x => x.PictureId,
                       principalTable: "Pictures",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "Addresses",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FirstName = table.Column<string>(type: "text", nullable: true),
                LastName = table.Column<string>(type: "text", nullable: true),
                Email = table.Column<string>(type: "text", nullable: true),
                Company = table.Column<string>(type: "text", nullable: true),
                CountryId = table.Column<long>(type: "bigint", nullable: true),
                StateProvinceId = table.Column<long>(type: "bigint", nullable: true),
                County = table.Column<string>(type: "text", nullable: true),
                City = table.Column<string>(type: "text", nullable: true),
                Address1 = table.Column<string>(type: "text", nullable: true),
                Address2 = table.Column<string>(type: "text", nullable: true),
                ZipPostalCode = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                FaxNumber = table.Column<string>(type: "text", nullable: true),
                CustomAttributes = table.Column<string>(type: "text", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Addresses", x => x.Id);
                table.ForeignKey(
                       name: "FK_Addresses_StateProvinces_StateProvinceId",
                       column: x => x.StateProvinceId,
                       principalTable: "StateProvinces",
                       principalColumn: "Id");
             });

         migrationBuilder.CreateTable(
             name: "CurrencyStateProvinces",
             columns: table => new
             {
                CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                StateProvinceId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_CurrencyStateProvinces", x => new { x.CurrencyId, x.StateProvinceId });
                table.ForeignKey(
                       name: "FK_CurrencyStateProvinces_Currencies_CurrencyId",
                       column: x => x.CurrencyId,
                       principalTable: "Currencies",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_CurrencyStateProvinces_StateProvinces_StateProvinceId",
                       column: x => x.StateProvinceId,
                       principalTable: "StateProvinces",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "Topics",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SystemName = table.Column<string>(type: "text", nullable: true),
                IncludeInSitemap = table.Column<bool>(type: "boolean", nullable: false),
                IncludeInTopMenu = table.Column<bool>(type: "boolean", nullable: false),
                IncludeInFooterColumn1 = table.Column<bool>(type: "boolean", nullable: false),
                IncludeInFooterColumn2 = table.Column<bool>(type: "boolean", nullable: false),
                IncludeInFooterColumn3 = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                AccessibleWhenPlatformClosed = table.Column<bool>(type: "boolean", nullable: false),
                IsPasswordProtected = table.Column<bool>(type: "boolean", nullable: false),
                Password = table.Column<string>(type: "text", nullable: true),
                Title = table.Column<string>(type: "text", nullable: true),
                Body = table.Column<string>(type: "text", nullable: true),
                Published = table.Column<bool>(type: "boolean", nullable: false),
                TopicTemplateId = table.Column<long>(type: "bigint", nullable: false),
                MetaKeywords = table.Column<string>(type: "text", nullable: true),
                MetaDescription = table.Column<string>(type: "text", nullable: true),
                MetaTitle = table.Column<string>(type: "text", nullable: true),
                SubjectToAcl = table.Column<bool>(type: "boolean", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Topics", x => x.Id);
                table.ForeignKey(
                       name: "FK_Topics_TopicTemplates_TopicTemplateId",
                       column: x => x.TopicTemplateId,
                       principalTable: "TopicTemplates",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "UserAttributeValues",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserAttributeId = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                IsPreSelected = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserAttributeValues", x => x.Id);
                table.ForeignKey(
                       name: "FK_UserAttributeValues_UserAttributes_UserAttributeId",
                       column: x => x.UserAttributeId,
                       principalTable: "UserAttributes",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "AclRecords",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EntityId = table.Column<long>(type: "bigint", nullable: false),
                EntityName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                UserRoleId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_AclRecords", x => x.Id);
                table.ForeignKey(
                       name: "FK_AclRecords_UserRoles_UserRoleId",
                       column: x => x.UserRoleId,
                       principalTable: "UserRoles",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "Campaigns",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Subject = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                Body = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                UserRoleId = table.Column<long>(type: "bigint", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DontSendBeforeDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Campaigns", x => x.Id);
                table.ForeignKey(
                       name: "FK_Campaigns_UserRoles_UserRoleId",
                       column: x => x.UserRoleId,
                       principalTable: "UserRoles",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "PermissionRecordUserRoles",
             columns: table => new
             {
                PermissionRecordId = table.Column<long>(type: "bigint", nullable: false),
                UserRoleId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_PermissionRecordUserRoles", x => new { x.UserRoleId, x.PermissionRecordId });
                table.ForeignKey(
                       name: "FK_PermissionRecordUserRoles_PermissionRecords_PermissionRecor~",
                       column: x => x.PermissionRecordId,
                       principalTable: "PermissionRecords",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_PermissionRecordUserRoles_UserRoles_UserRoleId",
                       column: x => x.UserRoleId,
                       principalTable: "UserRoles",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "Devices",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Guid = table.Column<Guid>(type: "uuid", nullable: false),
                SystemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                PictureId = table.Column<long>(type: "bigint", nullable: false),
                Configuration = table.Column<string>(type: "text", nullable: true),
                AdminComment = table.Column<string>(type: "text", nullable: true),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                Enabled = table.Column<bool>(type: "boolean", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                IsMobile = table.Column<bool>(type: "boolean", nullable: false),
                Lon = table.Column<double>(type: "double precision", nullable: false),
                Lat = table.Column<double>(type: "double precision", nullable: false),
                ShowOnMain = table.Column<bool>(type: "boolean", nullable: false),
                LastIpAddress = table.Column<string>(type: "text", nullable: true),
                CountDataRows = table.Column<int>(type: "integer", nullable: false),
                DataSendingDelay = table.Column<int>(type: "integer", nullable: false),
                DataflowReconnectDelay = table.Column<int>(type: "integer", nullable: false),
                DataPacketSize = table.Column<int>(type: "integer", nullable: false),
                ClearDataDelay = table.Column<int>(type: "integer", nullable: false),
                LastActivityOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                CannotLoginUntilDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                OwnerId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Devices", x => x.Id);
                table.ForeignKey(
                       name: "FK_Devices_Users_OwnerId",
                       column: x => x.OwnerId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "DownloadTracker",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                FileName = table.Column<string>(type: "text", nullable: true),
                TaskDateTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ReadyDateTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                DelayUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CurrentState = table.Column<int>(type: "integer", nullable: false),
                Size = table.Column<long>(type: "bigint", nullable: false),
                QueryString = table.Column<string>(type: "text", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_DownloadTracker", x => x.Id);
                table.ForeignKey(
                       name: "FK_DownloadTracker_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "ExternalAuthenticationRecord",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                Email = table.Column<string>(type: "text", nullable: true),
                ExternalIdentifier = table.Column<string>(type: "text", nullable: true),
                ExternalDisplayIdentifier = table.Column<string>(type: "text", nullable: true),
                OAuthToken = table.Column<string>(type: "text", nullable: true),
                OAuthAccessToken = table.Column<string>(type: "text", nullable: true),
                ProviderSystemName = table.Column<string>(type: "text", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ExternalAuthenticationRecord", x => x.Id);
                table.ForeignKey(
                       name: "FK_ExternalAuthenticationRecord_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "ForumPostVotes",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ForumPostId = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                IsUp = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ForumPostVotes", x => x.Id);
                table.ForeignKey(
                       name: "FK_ForumPostVotes_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "ForumSubscriptions",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SubscriptionGuid = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                ForumId = table.Column<long>(type: "bigint", nullable: false),
                TopicId = table.Column<long>(type: "bigint", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ForumSubscriptions", x => x.Id);
                table.ForeignKey(
                       name: "FK_ForumSubscriptions_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);
             });

         migrationBuilder.CreateTable(
             name: "Monitors",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PictureId = table.Column<long>(type: "bigint", nullable: false),
                AdminComment = table.Column<string>(type: "text", nullable: true),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                ShowInMenu = table.Column<bool>(type: "boolean", nullable: false),
                SubjectToAcl = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OwnerId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Monitors", x => x.Id);
                table.ForeignKey(
                       name: "FK_Monitors_Users_OwnerId",
                       column: x => x.OwnerId,
                       principalTable: "Users",
                       principalColumn: "Id");
             });

         migrationBuilder.CreateTable(
             name: "PrivateMessages",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FromUserId = table.Column<long>(type: "bigint", nullable: false),
                ToUserId = table.Column<long>(type: "bigint", nullable: false),
                Subject = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                Text = table.Column<string>(type: "text", nullable: false),
                IsRead = table.Column<bool>(type: "boolean", nullable: false),
                IsDeletedByAuthor = table.Column<bool>(type: "boolean", nullable: false),
                IsDeletedByRecipient = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_PrivateMessages", x => x.Id);
                table.ForeignKey(
                       name: "FK_PrivateMessages_Users_FromUserId",
                       column: x => x.FromUserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                       name: "FK_PrivateMessages_Users_ToUserId",
                       column: x => x.ToUserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);
             });

         migrationBuilder.CreateTable(
             name: "UserPasswords",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                Password = table.Column<string>(type: "text", nullable: true),
                PasswordFormatId = table.Column<long>(type: "bigint", nullable: false),
                PasswordSalt = table.Column<string>(type: "text", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                PasswordFormat = table.Column<int>(type: "integer", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserPasswords", x => x.Id);
                table.ForeignKey(
                       name: "FK_UserPasswords_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "UserUserRoles",
             columns: table => new
             {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                UserRoleId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserUserRoles", x => new { x.UserId, x.UserRoleId });
                table.ForeignKey(
                       name: "FK_UserUserRoles_UserRoles_UserRoleId",
                       column: x => x.UserRoleId,
                       principalTable: "UserRoles",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_UserUserRoles_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "Widgets",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PictureId = table.Column<long>(type: "bigint", nullable: false),
                Enabled = table.Column<bool>(type: "boolean", nullable: false),
                AdminComment = table.Column<string>(type: "text", nullable: true),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                SubjectToAcl = table.Column<bool>(type: "boolean", nullable: false),
                WidgetType = table.Column<int>(type: "integer", nullable: false),
                LiveSchemePictureId = table.Column<long>(type: "bigint", nullable: false),
                Adjustment = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                UserId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Widgets", x => x.Id);
                table.ForeignKey(
                       name: "FK_Widgets_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "ForumTopics",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ForumId = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                TopicTypeId = table.Column<int>(type: "integer", nullable: false),
                Subject = table.Column<string>(type: "text", nullable: true),
                NumPosts = table.Column<int>(type: "integer", nullable: false),
                Views = table.Column<int>(type: "integer", nullable: false),
                LastPostId = table.Column<long>(type: "bigint", nullable: false),
                LastPostUserId = table.Column<long>(type: "bigint", nullable: false),
                LastPostTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ForumTopicType = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ForumTopics", x => x.Id);
                table.ForeignKey(
                       name: "FK_ForumTopics_Forums_ForumId",
                       column: x => x.ForumId,
                       principalTable: "Forums",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_ForumTopics_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);
             });

         migrationBuilder.CreateTable(
             name: "BlogComments",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                CommentText = table.Column<string>(type: "text", nullable: true),
                IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                BlogPostId = table.Column<long>(type: "bigint", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_BlogComments", x => x.Id);
                table.ForeignKey(
                       name: "FK_BlogComments_BlogPosts_BlogPostId",
                       column: x => x.BlogPostId,
                       principalTable: "BlogPosts",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_BlogComments_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "PollAnswers",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PollId = table.Column<long>(type: "bigint", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                NumberOfVotes = table.Column<int>(type: "integer", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_PollAnswers", x => x.Id);
                table.ForeignKey(
                       name: "FK_PollAnswers_Polls_PollId",
                       column: x => x.PollId,
                       principalTable: "Polls",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "UserAddresses",
             columns: table => new
             {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                AddressId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserAddresses", x => new { x.UserId, x.AddressId });
                table.ForeignKey(
                       name: "FK_UserAddresses_Addresses_AddressId",
                       column: x => x.AddressId,
                       principalTable: "Addresses",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_UserAddresses_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "DeviceCommands",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Arguments = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                DeviceId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_DeviceCommands", x => x.Id);
                table.ForeignKey(
                       name: "FK_DeviceCommands_Devices_DeviceId",
                       column: x => x.DeviceId,
                       principalTable: "Devices",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "DeviceCredentials",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                PasswordSalt = table.Column<string>(type: "text", nullable: true),
                DeviceId = table.Column<long>(type: "bigint", nullable: false),
                PasswordFormatId = table.Column<long>(type: "bigint", nullable: false),
                PasswordFormat = table.Column<int>(type: "integer", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_DeviceCredentials", x => x.Id);
                table.ForeignKey(
                       name: "FK_DeviceCredentials_Devices_DeviceId",
                       column: x => x.DeviceId,
                       principalTable: "Devices",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "Sensors",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SystemName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                PictureId = table.Column<long>(type: "bigint", nullable: false),
                DeviceId = table.Column<long>(type: "bigint", nullable: false),
                AdminComment = table.Column<string>(type: "text", nullable: true),
                SubjectToAcl = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                Enabled = table.Column<bool>(type: "boolean", nullable: false),
                Configuration = table.Column<string>(type: "text", nullable: true),
                SensorType = table.Column<int>(type: "integer", nullable: false),
                PriorityType = table.Column<int>(type: "integer", nullable: false),
                ShowInCommonLog = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_Sensors", x => x.Id);
                table.ForeignKey(
                       name: "FK_Sensors_Devices_DeviceId",
                       column: x => x.DeviceId,
                       principalTable: "Devices",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "UserDevices",
             columns: table => new
             {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                DeviceId = table.Column<long>(type: "bigint", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserDevices", x => new { x.UserId, x.DeviceId });
                table.ForeignKey(
                       name: "FK_UserDevices_Devices_DeviceId",
                       column: x => x.DeviceId,
                       principalTable: "Devices",
                       principalColumn: "Id");
                table.ForeignKey(
                       name: "FK_UserDevices_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id");
             });

         migrationBuilder.CreateTable(
             name: "UserMonitors",
             columns: table => new
             {
                UserId = table.Column<long>(type: "bigint", nullable: false),
                MonitorId = table.Column<long>(type: "bigint", nullable: false),
                ShowInMenu = table.Column<bool>(type: "boolean", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_UserMonitors", x => new { x.UserId, x.MonitorId });
                table.ForeignKey(
                       name: "FK_UserMonitors_Monitors_MonitorId",
                       column: x => x.MonitorId,
                       principalTable: "Monitors",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_UserMonitors_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "ForumPosts",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ForumTopicId = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                Text = table.Column<string>(type: "text", nullable: false),
                IPAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                VoteCount = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_ForumPosts", x => x.Id);
                table.ForeignKey(
                       name: "FK_ForumPosts_ForumTopics_ForumTopicId",
                       column: x => x.ForumTopicId,
                       principalTable: "ForumTopics",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_ForumPosts_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);
             });

         migrationBuilder.CreateTable(
             name: "PollVotingRecords",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PollAnswerId = table.Column<long>(type: "bigint", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_PollVotingRecords", x => x.Id);
                table.ForeignKey(
                       name: "FK_PollVotingRecords_PollAnswers_PollAnswerId",
                       column: x => x.PollAnswerId,
                       principalTable: "PollAnswers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_PollVotingRecords_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "SensorRecords",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SensorId = table.Column<long>(type: "bigint", nullable: false),
                Value = table.Column<double>(type: "double precision", nullable: false),
                Bytes = table.Column<byte[]>(type: "bytea", nullable: true),
                JsonValue = table.Column<string>(type: "text", nullable: true),
                Metadata = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Timestamp = table.Column<long>(type: "bigint", nullable: false),
                EventTimestamp = table.Column<long>(type: "bigint", nullable: false),
                CreatedTimeOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_SensorRecords", x => x.Id);
                table.ForeignKey(
                       name: "FK_SensorRecords_Sensors_SensorId",
                       column: x => x.SensorId,
                       principalTable: "Sensors",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "SensorWidgets",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SensorId = table.Column<long>(type: "bigint", nullable: false),
                WidgetId = table.Column<long>(type: "bigint", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_SensorWidgets", x => x.Id);
                table.ForeignKey(
                       name: "FK_SensorWidgets_Sensors_SensorId",
                       column: x => x.SensorId,
                       principalTable: "Sensors",
                       principalColumn: "Id");
                table.ForeignKey(
                       name: "FK_SensorWidgets_Widgets_WidgetId",
                       column: x => x.WidgetId,
                       principalTable: "Widgets",
                       principalColumn: "Id");
             });

         migrationBuilder.CreateTable(
             name: "VideoSegments",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Extinf = table.Column<double>(type: "double precision", nullable: false),
                InboundName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Guid = table.Column<Guid>(type: "uuid", nullable: false),
                IpcamId = table.Column<long>(type: "bigint", nullable: false),
                OnCreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                OnReceivedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Resolution = table.Column<string>(type: "text", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_VideoSegments", x => x.Id);
                table.ForeignKey(
                       name: "FK_VideoSegments_Sensors_IpcamId",
                       column: x => x.IpcamId,
                       principalTable: "Sensors",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "MonitorSensorWidgets",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MonitorId = table.Column<long>(type: "bigint", nullable: false),
                SensorWidgetId = table.Column<long>(type: "bigint", nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_MonitorSensorWidgets", x => x.Id);
                table.ForeignKey(
                       name: "FK_MonitorSensorWidgets_Monitors_MonitorId",
                       column: x => x.MonitorId,
                       principalTable: "Monitors",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                       name: "FK_MonitorSensorWidgets_SensorWidgets_SensorWidgetId",
                       column: x => x.SensorWidgetId,
                       principalTable: "SensorWidgets",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateTable(
             name: "VideoSegmentBinaries",
             columns: table => new
             {
                Id = table.Column<long>(type: "bigint", nullable: false)
                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                VideoSegmentId = table.Column<long>(type: "bigint", nullable: false),
                Binary = table.Column<byte[]>(type: "bytea", nullable: true)
             },
             constraints: table =>
             {
                table.PrimaryKey("PK_VideoSegmentBinaries", x => x.Id);
                table.ForeignKey(
                       name: "FK_VideoSegmentBinaries_VideoSegments_VideoSegmentId",
                       column: x => x.VideoSegmentId,
                       principalTable: "VideoSegments",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
             });

         migrationBuilder.CreateIndex(
             name: "IX_AclRecords_UserRoleId",
             table: "AclRecords",
             column: "UserRoleId");

         migrationBuilder.CreateIndex(
             name: "IX_Addresses_StateProvinceId",
             table: "Addresses",
             column: "StateProvinceId");

         migrationBuilder.CreateIndex(
             name: "IX_BlogComments_BlogPostId",
             table: "BlogComments",
             column: "BlogPostId");

         migrationBuilder.CreateIndex(
             name: "IX_BlogComments_UserId",
             table: "BlogComments",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_BlogPosts_LanguageId",
             table: "BlogPosts",
             column: "LanguageId");

         migrationBuilder.CreateIndex(
             name: "IX_Campaigns_UserRoleId",
             table: "Campaigns",
             column: "UserRoleId");

         migrationBuilder.CreateIndex(
             name: "IX_CurrencyStateProvinces_StateProvinceId",
             table: "CurrencyStateProvinces",
             column: "StateProvinceId");

         migrationBuilder.CreateIndex(
             name: "IX_DeviceCommands_DeviceId",
             table: "DeviceCommands",
             column: "DeviceId");

         migrationBuilder.CreateIndex(
             name: "IX_DeviceCredentials_DeviceId",
             table: "DeviceCredentials",
             column: "DeviceId");

         migrationBuilder.CreateIndex(
             name: "IX_Devices_OwnerId",
             table: "Devices",
             column: "OwnerId");

         migrationBuilder.CreateIndex(
             name: "IX_Devices_SystemName",
             table: "Devices",
             column: "SystemName",
             unique: true,
             filter: "\"IsDeleted\" = false");

         migrationBuilder.CreateIndex(
             name: "IX_DownloadTracker_UserId",
             table: "DownloadTracker",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_ExternalAuthenticationRecord_UserId",
             table: "ExternalAuthenticationRecord",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_ForumPosts_ForumTopicId",
             table: "ForumPosts",
             column: "ForumTopicId");

         migrationBuilder.CreateIndex(
             name: "IX_ForumPosts_UserId",
             table: "ForumPosts",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_ForumPostVotes_UserId",
             table: "ForumPostVotes",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_Forums_ForumGroupId",
             table: "Forums",
             column: "ForumGroupId");

         migrationBuilder.CreateIndex(
             name: "IX_ForumSubscriptions_UserId",
             table: "ForumSubscriptions",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_ForumTopics_ForumId",
             table: "ForumTopics",
             column: "ForumId");

         migrationBuilder.CreateIndex(
             name: "IX_ForumTopics_UserId",
             table: "ForumTopics",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_GenericAttributes_EntityId_KeyGroup",
             table: "GenericAttributes",
             columns: new[] { "EntityId", "KeyGroup" });

         migrationBuilder.CreateIndex(
             name: "IX_LocaleStringResources_LanguageId",
             table: "LocaleStringResources",
             column: "LanguageId");

         migrationBuilder.CreateIndex(
             name: "IX_LocalizedProperties_EntityId_LocaleKey",
             table: "LocalizedProperties",
             columns: new[] { "EntityId", "LocaleKey" });

         migrationBuilder.CreateIndex(
             name: "IX_LocalizedProperties_LanguageId",
             table: "LocalizedProperties",
             column: "LanguageId");

         migrationBuilder.CreateIndex(
             name: "IX_LocalizedProperties_LocaleKeyGroup_LanguageId",
             table: "LocalizedProperties",
             columns: new[] { "LocaleKeyGroup", "LanguageId" });

         migrationBuilder.CreateIndex(
             name: "IX_Logs_LogLevelId",
             table: "Logs",
             column: "LogLevelId");

         migrationBuilder.CreateIndex(
             name: "IX_Monitors_OwnerId",
             table: "Monitors",
             column: "OwnerId");

         migrationBuilder.CreateIndex(
             name: "IX_MonitorSensorWidgets_MonitorId_SensorWidgetId",
             table: "MonitorSensorWidgets",
             columns: new[] { "MonitorId", "SensorWidgetId" },
             unique: true);

         migrationBuilder.CreateIndex(
             name: "IX_MonitorSensorWidgets_SensorWidgetId",
             table: "MonitorSensorWidgets",
             column: "SensorWidgetId");

         migrationBuilder.CreateIndex(
             name: "IX_PermissionRecordUserRoles_PermissionRecordId",
             table: "PermissionRecordUserRoles",
             column: "PermissionRecordId");

         migrationBuilder.CreateIndex(
             name: "IX_PictureBinaries_PictureId",
             table: "PictureBinaries",
             column: "PictureId",
             unique: true);

         migrationBuilder.CreateIndex(
             name: "IX_PollAnswers_PollId",
             table: "PollAnswers",
             column: "PollId");

         migrationBuilder.CreateIndex(
             name: "IX_Polls_LanguageId",
             table: "Polls",
             column: "LanguageId");

         migrationBuilder.CreateIndex(
             name: "IX_PollVotingRecords_PollAnswerId",
             table: "PollVotingRecords",
             column: "PollAnswerId");

         migrationBuilder.CreateIndex(
             name: "IX_PollVotingRecords_UserId",
             table: "PollVotingRecords",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_PrivateMessages_FromUserId",
             table: "PrivateMessages",
             column: "FromUserId");

         migrationBuilder.CreateIndex(
             name: "IX_PrivateMessages_ToUserId",
             table: "PrivateMessages",
             column: "ToUserId");

         migrationBuilder.CreateIndex(
             name: "IX_SensorRecords_SensorId",
             table: "SensorRecords",
             column: "SensorId");

         migrationBuilder.CreateIndex(
             name: "IX_Sensors_DeviceId_SystemName",
             table: "Sensors",
             columns: new[] { "DeviceId", "SystemName" },
             unique: true,
             filter: "\"IsDeleted\" = false");

         migrationBuilder.CreateIndex(
             name: "IX_SensorWidgets_SensorId",
             table: "SensorWidgets",
             column: "SensorId");

         migrationBuilder.CreateIndex(
             name: "IX_SensorWidgets_WidgetId_SensorId",
             table: "SensorWidgets",
             columns: new[] { "WidgetId", "SensorId" },
             unique: true);

         migrationBuilder.CreateIndex(
             name: "IX_Topics_TopicTemplateId",
             table: "Topics",
             column: "TopicTemplateId");

         migrationBuilder.CreateIndex(
             name: "IX_UserAddresses_AddressId",
             table: "UserAddresses",
             column: "AddressId");

         migrationBuilder.CreateIndex(
             name: "IX_UserAttributeValues_UserAttributeId",
             table: "UserAttributeValues",
             column: "UserAttributeId");

         migrationBuilder.CreateIndex(
             name: "IX_UserDevices_DeviceId",
             table: "UserDevices",
             column: "DeviceId");

         migrationBuilder.CreateIndex(
             name: "IX_UserMonitors_MonitorId",
             table: "UserMonitors",
             column: "MonitorId");

         migrationBuilder.CreateIndex(
             name: "IX_UserPasswords_UserId",
             table: "UserPasswords",
             column: "UserId");

         migrationBuilder.CreateIndex(
             name: "IX_Users_Email",
             table: "Users",
             column: "Email",
             filter: "\"IsDeleted\" = false");

         migrationBuilder.CreateIndex(
             name: "IX_UserUserRoles_UserRoleId",
             table: "UserUserRoles",
             column: "UserRoleId");

         migrationBuilder.CreateIndex(
             name: "IX_VideoSegmentBinaries_VideoSegmentId",
             table: "VideoSegmentBinaries",
             column: "VideoSegmentId",
             unique: true);

         migrationBuilder.CreateIndex(
             name: "IX_VideoSegments_IpcamId",
             table: "VideoSegments",
             column: "IpcamId");

         migrationBuilder.CreateIndex(
             name: "IX_Widgets_UserId",
             table: "Widgets",
             column: "UserId");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
         migrationBuilder.DropTable(
             name: "AclRecords");

         migrationBuilder.DropTable(
             name: "ActivityLogs");

         migrationBuilder.DropTable(
             name: "ActivityLogTypes");

         migrationBuilder.DropTable(
             name: "AddressAttributes");

         migrationBuilder.DropTable(
             name: "AddressAttributeValues");

         migrationBuilder.DropTable(
             name: "BlogComments");

         migrationBuilder.DropTable(
             name: "Campaigns");

         migrationBuilder.DropTable(
             name: "Countries");

         migrationBuilder.DropTable(
             name: "CurrencyStateProvinces");

         migrationBuilder.DropTable(
             name: "DeviceCommands");

         migrationBuilder.DropTable(
             name: "DeviceCredentials");

         migrationBuilder.DropTable(
             name: "Downloads");

         migrationBuilder.DropTable(
             name: "DownloadTracker");

         migrationBuilder.DropTable(
             name: "EmailAccounts");

         migrationBuilder.DropTable(
             name: "ExternalAuthenticationRecord");

         migrationBuilder.DropTable(
             name: "ForumPosts");

         migrationBuilder.DropTable(
             name: "ForumPostVotes");

         migrationBuilder.DropTable(
             name: "ForumSubscriptions");

         migrationBuilder.DropTable(
             name: "GdprConsents");

         migrationBuilder.DropTable(
             name: "GdprLogs");

         migrationBuilder.DropTable(
             name: "GenericAttributes");

         migrationBuilder.DropTable(
             name: "LocaleStringResources");

         migrationBuilder.DropTable(
             name: "LocalizedProperties");

         migrationBuilder.DropTable(
             name: "Logs");

         migrationBuilder.DropTable(
             name: "MeasureDimensions");

         migrationBuilder.DropTable(
             name: "MeasureWeights");

         migrationBuilder.DropTable(
             name: "MessageTemplates");

         migrationBuilder.DropTable(
             name: "MonitorSensorWidgets");

         migrationBuilder.DropTable(
             name: "NewsComments");

         migrationBuilder.DropTable(
             name: "NewsItems");

         migrationBuilder.DropTable(
             name: "NewsLetterSubscriptions");

         migrationBuilder.DropTable(
             name: "PermissionRecordUserRoles");

         migrationBuilder.DropTable(
             name: "PictureBinaries");

         migrationBuilder.DropTable(
             name: "PollVotingRecords");

         migrationBuilder.DropTable(
             name: "PrivateMessages");

         migrationBuilder.DropTable(
             name: "QueuedEmails");

         migrationBuilder.DropTable(
             name: "ScheduleTasks");

         migrationBuilder.DropTable(
             name: "SearchTerms");

         migrationBuilder.DropTable(
             name: "SensorRecords");

         migrationBuilder.DropTable(
             name: "Settings");

         migrationBuilder.DropTable(
             name: "Topics");

         migrationBuilder.DropTable(
             name: "UrlRecords");

         migrationBuilder.DropTable(
             name: "UserAddresses");

         migrationBuilder.DropTable(
             name: "UserAttributeValues");

         migrationBuilder.DropTable(
             name: "UserDevices");

         migrationBuilder.DropTable(
             name: "UserMonitors");

         migrationBuilder.DropTable(
             name: "UserPasswords");

         migrationBuilder.DropTable(
             name: "UserUserRoles");

         migrationBuilder.DropTable(
             name: "VideoSegmentBinaries");

         migrationBuilder.DropTable(
             name: "BlogPosts");

         migrationBuilder.DropTable(
             name: "Currencies");

         migrationBuilder.DropTable(
             name: "ForumTopics");

         migrationBuilder.DropTable(
             name: "SensorWidgets");

         migrationBuilder.DropTable(
             name: "PermissionRecords");

         migrationBuilder.DropTable(
             name: "Pictures");

         migrationBuilder.DropTable(
             name: "PollAnswers");

         migrationBuilder.DropTable(
             name: "TopicTemplates");

         migrationBuilder.DropTable(
             name: "Addresses");

         migrationBuilder.DropTable(
             name: "UserAttributes");

         migrationBuilder.DropTable(
             name: "Monitors");

         migrationBuilder.DropTable(
             name: "UserRoles");

         migrationBuilder.DropTable(
             name: "VideoSegments");

         migrationBuilder.DropTable(
             name: "Forums");

         migrationBuilder.DropTable(
             name: "Widgets");

         migrationBuilder.DropTable(
             name: "Polls");

         migrationBuilder.DropTable(
             name: "StateProvinces");

         migrationBuilder.DropTable(
             name: "Sensors");

         migrationBuilder.DropTable(
             name: "ForumGroups");

         migrationBuilder.DropTable(
             name: "Languages");

         migrationBuilder.DropTable(
             name: "Devices");

         migrationBuilder.DropTable(
             name: "Users");
      }
   }
}
