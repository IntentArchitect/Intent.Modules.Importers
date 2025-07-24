-- PostgreSQL equivalent of SqlServerImporterTest database
-- Converted from SQL Server CreateDB.sql

-- Create database (run this separately as a superuser if needed)
-- CREATE DATABASE "PostgresImporterTest" WITH ENCODING 'UTF8';

-- Connect to the database
-- \c PostgresImporterTest;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create schemas
CREATE SCHEMA IF NOT EXISTS schema2;
CREATE SCHEMA IF NOT EXISTS views;

-- Create tables

-- Table: __EFMigrationsHistory
CREATE TABLE "__EFMigrationsHistory" (
    "MigrationId" VARCHAR(150) NOT NULL,
    "ProductVersion" VARCHAR(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Table: AspNetRoles
CREATE TABLE "AspNetRoles" (
    "Id" VARCHAR(450) NOT NULL,
    "Name" VARCHAR(256) NULL,
    "NormalizedName" VARCHAR(256) NULL,
    "ConcurrencyStamp" TEXT NULL,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

-- Table: AspNetUsers
CREATE TABLE "AspNetUsers" (
    "Id" VARCHAR(450) NOT NULL,
    "RefreshToken" TEXT NULL,
    "RefreshTokenExpired" TIMESTAMP NULL,
    "UserName" VARCHAR(256) NULL,
    "NormalizedUserName" VARCHAR(256) NULL,
    "Email" VARCHAR(256) NULL,
    "NormalizedEmail" VARCHAR(256) NULL,
    "EmailConfirmed" BOOLEAN NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL,
    "TwoFactorEnabled" BOOLEAN NOT NULL,
    "LockoutEnd" TIMESTAMPTZ NULL,
    "LockoutEnabled" BOOLEAN NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);

-- Table: AspNetRoleClaims
CREATE TABLE "AspNetRoleClaims" (
    "Id" SERIAL NOT NULL,
    "RoleId" VARCHAR(450) NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id")
);

-- Table: AspNetUserClaims
CREATE TABLE "AspNetUserClaims" (
    "Id" SERIAL NOT NULL,
    "UserId" VARCHAR(450) NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id")
);

-- Table: AspNetUserLogins
CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" VARCHAR(450) NOT NULL,
    "ProviderKey" VARCHAR(450) NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" VARCHAR(450) NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey")
);

-- Table: AspNetUserRoles
CREATE TABLE "AspNetUserRoles" (
    "UserId" VARCHAR(450) NOT NULL,
    "RoleId" VARCHAR(450) NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId")
);

-- Table: AspNetUserTokens
CREATE TABLE "AspNetUserTokens" (
    "UserId" VARCHAR(450) NOT NULL,
    "LoginProvider" VARCHAR(450) NOT NULL,
    "Name" VARCHAR(450) NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name")
);

-- Table: Brands
CREATE TABLE "Brands" (
    "Id" UUID NOT NULL,
    "Name" TEXT NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    CONSTRAINT "PK_Brands" PRIMARY KEY ("Id")
);

-- Table: Customers
CREATE TABLE "Customers" (
    "Id" UUID NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Surname" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    "Preferences_Specials" BOOLEAN NULL,
    "Preferences_Newsletter" BOOLEAN NULL,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

-- Table: Orders
CREATE TABLE "Orders" (
    "Id" UUID NOT NULL,
    "CustomerId" UUID NOT NULL,
    "OrderDate" TIMESTAMP NOT NULL,
    "RefNo" VARCHAR(450) NOT NULL,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id")
);

-- Table: Products
CREATE TABLE "Products" (
    "Id" UUID NOT NULL,
    "BrandId" UUID NOT NULL,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id")
);

-- Table: Addresses
CREATE TABLE "Addresses" (
    "Id" UUID NOT NULL,
    "CustomerId" UUID NOT NULL,
    "Line1" TEXT NOT NULL,
    "Line2" TEXT NOT NULL,
    "City" TEXT NOT NULL,
    "PostalCode" TEXT NOT NULL,
    "AddressType" INTEGER NOT NULL,
    CONSTRAINT "PK_Addresses" PRIMARY KEY ("Id")
);

-- Table: OrderItems
CREATE TABLE "OrderItems" (
    "Id" UUID NOT NULL,
    "OrderId" UUID NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "Amount" NUMERIC(16, 4) NOT NULL,
    "ProductId" UUID NOT NULL,
    CONSTRAINT "PK_OrderItems" PRIMARY KEY ("Id")
);

-- Table: Prices
CREATE TABLE "Prices" (
    "Id" UUID NOT NULL,
    "ProductId" UUID NOT NULL,
    "ActiveFrom" TIMESTAMP NOT NULL,
    "Amount" NUMERIC(16, 4) NOT NULL,
    CONSTRAINT "PK_Prices" PRIMARY KEY ("Id")
);

-- Table: Parents (composite primary key)
CREATE TABLE "Parents" (
    "Id" UUID NOT NULL,
    "Id2" UUID NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "PK_Parents" PRIMARY KEY ("Id", "Id2")
);

-- Table: Children
CREATE TABLE "Children" (
    "Id" UUID NOT NULL,
    "ParentId" UUID NOT NULL,
    "ParentId2" UUID NOT NULL,
    CONSTRAINT "PK_Children" PRIMARY KEY ("Id")
);

-- Table: Legacy_Table
CREATE TABLE "Legacy_Table" (
    "LegacyID" INTEGER NOT NULL,
    "LegacyColumn" VARCHAR(100) NOT NULL,
    "BadDate" TIMESTAMP NOT NULL
);

-- Tables in schema2
CREATE TABLE schema2."Bank" (
    "Id" UUID NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "PK_Bank" PRIMARY KEY ("Id")
);

CREATE TABLE schema2."Banks" (
    "Id" UUID NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "PK_Banks" PRIMARY KEY ("Id")
);

CREATE TABLE schema2."Customers" (
    "Id" UUID NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Surname" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

-- Create indexes

-- AspNetRoles indexes
CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName") WHERE "NormalizedName" IS NOT NULL;

-- AspNetUsers indexes
CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName") WHERE "NormalizedUserName" IS NOT NULL;

-- AspNetRoleClaims indexes
CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

-- AspNetUserClaims indexes
CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

-- AspNetUserLogins indexes
CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

-- AspNetUserRoles indexes
CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

-- Business table indexes
CREATE INDEX "IX_Addresses_CustomerId" ON "Addresses" ("CustomerId");
CREATE INDEX "IX_Children_ParentId_ParentId2" ON "Children" ("ParentId", "ParentId2");
CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");
CREATE INDEX "IX_OrderItems_ProductId" ON "OrderItems" ("ProductId");
CREATE INDEX "IX_Orders_CustomerId" ON "Orders" ("CustomerId") INCLUDE ("OrderDate");
CREATE UNIQUE INDEX "IX_Orders_RefNo" ON "Orders" ("RefNo");
CREATE INDEX "IX_Prices_ProductId" ON "Prices" ("ProductId");
CREATE INDEX "IX_Products_BrandId" ON "Products" ("BrandId");

-- Add foreign key constraints

-- AspNet Identity foreign keys
ALTER TABLE "AspNetRoleClaims" ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" 
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE;

ALTER TABLE "AspNetUserClaims" ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;

ALTER TABLE "AspNetUserLogins" ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;

ALTER TABLE "AspNetUserRoles" ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" 
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE;

ALTER TABLE "AspNetUserRoles" ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;

ALTER TABLE "AspNetUserTokens" ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE;

-- Business table foreign keys
ALTER TABLE "Addresses" ADD CONSTRAINT "FK_Addresses_Customers_CustomerId" 
    FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE;

ALTER TABLE "Children" ADD CONSTRAINT "FK_Children_Parents_ParentId_ParentId2" 
    FOREIGN KEY ("ParentId", "ParentId2") REFERENCES "Parents" ("Id", "Id2");

ALTER TABLE "OrderItems" ADD CONSTRAINT "FK_OrderItems_Orders_OrderId" 
    FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE;

ALTER TABLE "OrderItems" ADD CONSTRAINT "FK_OrderItems_Products_ProductId" 
    FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id");

ALTER TABLE "Orders" ADD CONSTRAINT "FK_Orders_Customers_CustomerId" 
    FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id");

ALTER TABLE "Prices" ADD CONSTRAINT "FK_Prices_Products_ProductId" 
    FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE;

ALTER TABLE "Products" ADD CONSTRAINT "FK_Products_Brands_BrandId" 
    FOREIGN KEY ("BrandId") REFERENCES "Brands" ("Id");

-- Create view
CREATE VIEW views."vwOrders" AS
SELECT "Orders".*
FROM "Orders";

-- Create functions (PostgreSQL equivalent of stored procedures)

-- Function: GetCustomerOrders
CREATE OR REPLACE FUNCTION "GetCustomerOrders"(customer_id UUID)
RETURNS TABLE(
    "OrderDate" TIMESTAMP,
    "RefNo" VARCHAR(450)
) AS $$
BEGIN
    RETURN QUERY
    SELECT o."OrderDate", o."RefNo" 
    FROM "Orders" o
    WHERE o."CustomerId" = customer_id;
END;
$$ LANGUAGE plpgsql;

-- Function: GetOrderItemDetails
CREATE OR REPLACE FUNCTION "GetOrderItemDetails"(order_id UUID)
RETURNS TABLE(
    "Id" UUID,
    "OrderId" UUID,
    "Quantity" INTEGER,
    "Amount" NUMERIC(16,4),
    "ProductId" UUID,
    "RefNo" VARCHAR(450),
    "Name" VARCHAR(100),
    "Surname" TEXT,
    "Email" TEXT,
    "ProductName" TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        oi."Id",
        oi."OrderId",
        oi."Quantity",
        oi."Amount",
        oi."ProductId",
        o."RefNo",
        c."Name",
        c."Surname",
        c."Email",
        p."Name" as "ProductName"
    FROM "OrderItems" oi
    INNER JOIN "Orders" o ON o."Id" = oi."OrderId"
    INNER JOIN "Customers" c ON c."Id" = o."CustomerId"
    INNER JOIN "Products" p ON p."Id" = oi."ProductId"
    WHERE o."Id" = order_id;
END;
$$ LANGUAGE plpgsql;

-- Create composite type for brand data (PostgreSQL equivalent of table type)
CREATE TYPE "BrandType" AS (
    "Name" TEXT,
    "IsActive" BOOLEAN
);

-- Function: InsertBrand (using array of composite type)
CREATE OR REPLACE FUNCTION "InsertBrand"(brands "BrandType"[])
RETURNS VOID AS $$
DECLARE
    brand "BrandType";
BEGIN
    FOREACH brand IN ARRAY brands
    LOOP
        INSERT INTO "Brands" ("Id", "Name", "IsActive")
        VALUES (uuid_generate_v4(), brand."Name", brand."IsActive");
    END LOOP;
END;
$$ LANGUAGE plpgsql;

-- Alternative InsertBrand function using temporary table approach
CREATE OR REPLACE FUNCTION "InsertBrandFromTemp"()
RETURNS VOID AS $$
BEGIN
    INSERT INTO "Brands" ("Id", "Name", "IsActive")
    SELECT uuid_generate_v4(), "Name", "IsActive" 
    FROM temp_brands;
END;
$$ LANGUAGE plpgsql;

-- Comments for usage
COMMENT ON FUNCTION "GetCustomerOrders"(UUID) IS 'Returns order date and reference number for a specific customer';
COMMENT ON FUNCTION "GetOrderItemDetails"(UUID) IS 'Returns detailed order item information including customer and product details';
COMMENT ON FUNCTION "InsertBrand"("BrandType"[]) IS 'Inserts multiple brands using array of BrandType composite type';
COMMENT ON FUNCTION "InsertBrandFromTemp"() IS 'Inserts brands from a temporary table named temp_brands';

-- Usage examples (commented out):
/*
-- Example usage of GetCustomerOrders:
-- SELECT * FROM "GetCustomerOrders"('some-uuid-here');

-- Example usage of GetOrderItemDetails:
-- SELECT * FROM "GetOrderItemDetails"('some-uuid-here');

-- Example usage of InsertBrand:
-- SELECT "InsertBrand"(ARRAY[('Brand Name', true)::BrandType, ('Another Brand', false)::BrandType]);

-- Example usage of InsertBrandFromTemp:
-- CREATE TEMP TABLE temp_brands ("Name" TEXT, "IsActive" BOOLEAN);
-- INSERT INTO temp_brands VALUES ('Temp Brand', true);
-- SELECT "InsertBrandFromTemp"();
-- DROP TABLE temp_brands;
*/
