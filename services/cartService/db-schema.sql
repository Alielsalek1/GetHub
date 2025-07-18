-- CartService Database Schema

-- Create Carts table
CREATE TABLE IF NOT EXISTS "Carts" (
    "Id" uuid PRIMARY KEY,
    "CustomerId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT true
);

-- Create CartItems table
CREATE TABLE IF NOT EXISTS "CartItems" (
    "Id" uuid PRIMARY KEY,
    "CartId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "ProductName" varchar(200) NOT NULL,
    "UnitPrice" decimal(18,2) NOT NULL,
    "Quantity" integer NOT NULL,
    "AddedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    FOREIGN KEY ("CartId") REFERENCES "Carts"("Id") ON DELETE CASCADE
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "IX_Carts_CustomerId" ON "Carts" ("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_Carts_IsActive" ON "Carts" ("IsActive");
CREATE INDEX IF NOT EXISTS "IX_CartItems_CartId" ON "CartItems" ("CartId");
CREATE INDEX IF NOT EXISTS "IX_CartItems_ProductId" ON "CartItems" ("ProductId");

-- Create read model tables (for CQRS queries)
CREATE TABLE IF NOT EXISTS "CartSummaries" (
    "Id" uuid PRIMARY KEY,
    "CustomerId" uuid NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "TotalItems" integer NOT NULL,
    "IsActive" boolean NOT NULL
);

CREATE TABLE IF NOT EXISTS "CartDetails" (
    "Id" uuid PRIMARY KEY,
    "CustomerId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "IsActive" boolean NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "TotalItems" integer NOT NULL
);

-- Create CartDetail items (owned entity)
CREATE TABLE IF NOT EXISTS "CartDetails_Items" (
    "CartDetailsId" uuid NOT NULL,
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "ProductName" varchar(200) NOT NULL,
    "UnitPrice" decimal(18,2) NOT NULL,
    "Quantity" integer NOT NULL,
    "TotalPrice" decimal(18,2) NOT NULL,
    "AddedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    PRIMARY KEY ("CartDetailsId", "Id"),
    FOREIGN KEY ("CartDetailsId") REFERENCES "CartDetails"("Id") ON DELETE CASCADE
);

-- Create views for optimized read operations (optional)
CREATE OR REPLACE VIEW "ActiveCartSummaries" AS
SELECT 
    c."Id",
    c."CustomerId",
    c."UpdatedAt",
    COALESCE(SUM(ci."UnitPrice" * ci."Quantity"), 0) AS "TotalAmount",
    COALESCE(SUM(ci."Quantity"), 0) AS "TotalItems",
    c."IsActive"
FROM "Carts" c
LEFT JOIN "CartItems" ci ON c."Id" = ci."CartId"
WHERE c."IsActive" = true
GROUP BY c."Id", c."CustomerId", c."UpdatedAt", c."IsActive";

CREATE OR REPLACE VIEW "CartDetailsView" AS
SELECT 
    c."Id",
    c."CustomerId",
    c."CreatedAt",
    c."UpdatedAt",
    c."IsActive",
    COALESCE(SUM(ci."UnitPrice" * ci."Quantity"), 0) AS "TotalAmount",
    COALESCE(SUM(ci."Quantity"), 0) AS "TotalItems"
FROM "Carts" c
LEFT JOIN "CartItems" ci ON c."Id" = ci."CartId"
WHERE c."IsActive" = true
GROUP BY c."Id", c."CustomerId", c."CreatedAt", c."UpdatedAt", c."IsActive";
