-- OrderService Database Schema

-- Create Orders table
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id" uuid PRIMARY KEY,
    "CustomerId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "OrderDate" timestamp with time zone NOT NULL,
    "ShippingAddress" varchar(500),
    "BillingAddress" varchar(500)
);

-- Create OrderItems table
CREATE TABLE IF NOT EXISTS "OrderItems" (
    "Id" uuid PRIMARY KEY,
    "OrderId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "ProductName" varchar(200) NOT NULL,
    "Quantity" integer NOT NULL,
    "UnitPrice" decimal(18,2) NOT NULL,
    FOREIGN KEY ("OrderId") REFERENCES "Orders"("Id") ON DELETE CASCADE
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "IX_Orders_CustomerId" ON "Orders" ("CustomerId");
CREATE INDEX IF NOT EXISTS "IX_Orders_Status" ON "Orders" ("Status");
CREATE INDEX IF NOT EXISTS "IX_Orders_OrderDate" ON "Orders" ("OrderDate");
CREATE INDEX IF NOT EXISTS "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");

-- Create read model tables (for CQRS queries)
CREATE TABLE IF NOT EXISTS "OrderSummaries" (
    "Id" uuid PRIMARY KEY,
    "CustomerId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "OrderDate" timestamp with time zone NOT NULL,
    "ItemCount" integer NOT NULL
);

CREATE TABLE IF NOT EXISTS "OrderDetails" (
    "Id" uuid PRIMARY KEY,
    "CustomerId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "OrderDate" timestamp with time zone NOT NULL,
    "ShippingAddress" varchar(500),
    "BillingAddress" varchar(500)
);

-- Create OrderDetail items (owned entity)
CREATE TABLE IF NOT EXISTS "OrderDetails_Items" (
    "OrderDetailsId" uuid NOT NULL,
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "ProductName" varchar(200) NOT NULL,
    "Quantity" integer NOT NULL,
    "UnitPrice" decimal(18,2) NOT NULL,
    "TotalPrice" decimal(18,2) NOT NULL,
    PRIMARY KEY ("OrderDetailsId", "Id"),
    FOREIGN KEY ("OrderDetailsId") REFERENCES "OrderDetails"("Id") ON DELETE CASCADE
);
