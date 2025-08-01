using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Migrations
{
    [Migration(20250828)]
    public class InitialProductTables: Migration
    {
        public override void Down()
        {
            Delete.Table("ProductPrice");
            Delete.Table("Product");
            Delete.Table("Category");
            Delete.Table("CategoryType");
        }

        public override void Up()
        {
            // 1. CategoryType
            Create.Table("CategoryType")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("CategoryTypeName").AsString(100).NotNullable()
                .WithColumn("IsDeleted").AsBoolean().WithDefaultValue(false)
                .WithColumn("IsActive").AsBoolean().WithDefaultValue(true)
                .WithColumn("CreatedBy").AsInt64().Nullable()
                .WithColumn("CreateDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("ModifiedBy").AsInt64().Nullable()
                .WithColumn("ModifiedDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);

            // 2. Category
            Create.Table("Category")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("CategoryName").AsString(100).NotNullable()
                .WithColumn("ImageUrl").AsString(500).Nullable()
                .WithColumn("CategoryTypeId").AsInt64().NotNullable()
                .WithColumn("IsDeleted").AsBoolean().WithDefaultValue(false)
                .WithColumn("IsActive").AsBoolean().WithDefaultValue(true)
                .WithColumn("CreatedBy").AsInt64().Nullable()
                .WithColumn("CreateDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("ModifiedBy").AsInt64().Nullable()
                .WithColumn("ModifiedDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_Category_CategoryType")
                .FromTable("Category").ForeignColumn("CategoryTypeId")
                .ToTable("CategoryType").PrimaryColumn("Id");

            // 3. Product
            Create.Table("Product")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ProductName").AsString(150).NotNullable()
                .WithColumn("Description").AsString(500).Nullable()
                .WithColumn("ImageUrl").AsString(500).Nullable()
                .WithColumn("CategoryId").AsInt64().NotNullable()
                .WithColumn("IsDeleted").AsBoolean().WithDefaultValue(false)
                .WithColumn("IsActive").AsBoolean().WithDefaultValue(true)
                .WithColumn("CreatedBy").AsInt64().Nullable()
                .WithColumn("CreateDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("ModifiedBy").AsInt64().Nullable()
                .WithColumn("ModifiedDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_Product_Category")
                .FromTable("Product").ForeignColumn("CategoryId")
                .ToTable("Category").PrimaryColumn("Id");

            // 4. ProductPrice
            Create.Table("ProductPrice")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ProductId").AsInt64().NotNullable()
                .WithColumn("Size").AsString(50).Nullable()
                .WithColumn("Price").AsDecimal(10, 2).NotNullable()
                .WithColumn("IsDeleted").AsBoolean().WithDefaultValue(false)
                .WithColumn("IsActive").AsBoolean().WithDefaultValue(true)
                .WithColumn("CreatedBy").AsInt64().Nullable()
                .WithColumn("CreateDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("ModifiedBy").AsInt64().Nullable()
                .WithColumn("ModifiedDate").AsDateTime().WithDefault(SystemMethods.CurrentUTCDateTime);

            Create.ForeignKey("FK_ProductPrice_Product")
                .FromTable("ProductPrice").ForeignColumn("ProductId")
                .ToTable("Product").PrimaryColumn("Id");

            // Insert into CategoryType
            Execute.Sql("INSERT INTO CategoryType (CategoryTypeName) VALUES ('Vegetarian');");
            Execute.Sql("INSERT INTO CategoryType (CategoryTypeName) VALUES ('Non-Vegetarian');");

            // Insert into Category assuming CategoryTypeId 1 (Veg), 2 (Non-Veg)
            Execute.Sql("INSERT INTO Category (CategoryName, ImageUrl, CategoryTypeId) VALUES ('Pizza', '/images/pizza.jpg', 1);");
            Execute.Sql("INSERT INTO Category (CategoryName, ImageUrl, CategoryTypeId) VALUES ('Biryani', '/images/biryani.jpg', 2);");
            Execute.Sql("INSERT INTO Category (CategoryName, ImageUrl, CategoryTypeId) VALUES ('Dessert', '/images/dessert.jpg', 1);");
            Execute.Sql("INSERT INTO Category (CategoryName, ImageUrl, CategoryTypeId) VALUES ('Salad', '/images/salad.jpg', 1);");

            // Insert Products
            for (int i = 1; i <= 20; i++)
            {
                var catId = (i % 4) + 1;
                var name = $"Product {i}";
                var desc = $"Description for Product {i}";
                var img = $"/images/product_{i}.jpg";
                Execute.Sql($"INSERT INTO Product (ProductName, Description, ImageUrl, CategoryId) VALUES ('{name}', '{desc}', '{img}', {catId});");
            }

            // Insert Prices
            for (int productId = 1; productId <= 20; productId++)
            {
                Execute.Sql($"INSERT INTO ProductPrice (ProductId, Size, Price) VALUES ({productId}, 'Regular', {99 + productId});");
                if (productId % 2 == 0)
                {
                    Execute.Sql($"INSERT INTO ProductPrice (ProductId, Size, Price) VALUES ({productId}, 'Large', {129 + productId});");
                }
            }
        }
    }
}
