-- =============================================
-- LARGE E-COMMERCE DATABASE SCHEMA - PostgreSQL
-- =============================================

-- Create Database (run this separately as postgres superuser)
-- CREATE DATABASE ecommerce_db;
-- \c ecommerce_db;

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =============================================
-- TABLES
-- =============================================

-- Users Table
CREATE TABLE users (
                       user_id SERIAL PRIMARY KEY,
                       username VARCHAR(50) NOT NULL UNIQUE,
                       email VARCHAR(100) NOT NULL UNIQUE,
                       password_hash VARCHAR(255) NOT NULL,
                       first_name VARCHAR(50) NOT NULL,
                       last_name VARCHAR(50) NOT NULL,
                       date_of_birth DATE,
                       phone_number VARCHAR(15),
                       is_active BOOLEAN DEFAULT TRUE,
                       created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                       modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                       last_login_date TIMESTAMP
);

-- User Addresses
CREATE TABLE user_addresses (
                                address_id SERIAL PRIMARY KEY,
                                user_id INTEGER NOT NULL,
                                address_type VARCHAR(20) NOT NULL CHECK (address_type IN ('Billing', 'Shipping')),
                                address_line1 VARCHAR(255) NOT NULL,
                                address_line2 VARCHAR(255),
                                city VARCHAR(100) NOT NULL,
                                state VARCHAR(50) NOT NULL,
                                zip_code VARCHAR(10) NOT NULL,
                                country VARCHAR(50) NOT NULL DEFAULT 'USA',
                                is_default BOOLEAN DEFAULT FALSE,
                                created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Categories
CREATE TABLE categories (
                            category_id SERIAL PRIMARY KEY,
                            category_name VARCHAR(100) NOT NULL UNIQUE,
                            description TEXT,
                            parent_category_id INTEGER,
                            is_active BOOLEAN DEFAULT TRUE,
                            created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                            modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Suppliers
CREATE TABLE suppliers (
                           supplier_id SERIAL PRIMARY KEY,
                           supplier_name VARCHAR(100) NOT NULL,
                           contact_name VARCHAR(100),
                           email VARCHAR(100),
                           phone VARCHAR(15),
                           address VARCHAR(255),
                           city VARCHAR(100),
                           state VARCHAR(50),
                           zip_code VARCHAR(10),
                           country VARCHAR(50) DEFAULT 'USA',
                           is_active BOOLEAN DEFAULT TRUE,
                           created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Products
CREATE TABLE products (
                          product_id SERIAL PRIMARY KEY,
                          product_name VARCHAR(255) NOT NULL,
                          description TEXT,
                          sku VARCHAR(50) NOT NULL UNIQUE,
                          category_id INTEGER NOT NULL,
                          supplier_id INTEGER NOT NULL,
                          unit_price DECIMAL(10,2) NOT NULL,
                          units_in_stock INTEGER NOT NULL DEFAULT 0,
                          units_on_order INTEGER NOT NULL DEFAULT 0,
                          reorder_level INTEGER NOT NULL DEFAULT 0,
                          discontinued BOOLEAN DEFAULT FALSE,
                          weight DECIMAL(8,2),
                          dimensions VARCHAR(50),
                          image_url VARCHAR(500),
                          created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                          modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Product Reviews
CREATE TABLE product_reviews (
                                 review_id SERIAL PRIMARY KEY,
                                 product_id INTEGER NOT NULL,
                                 user_id INTEGER NOT NULL,
                                 rating INTEGER NOT NULL CHECK (rating >= 1 AND rating <= 5),
                                 title VARCHAR(200),
                                 review_text TEXT,
                                 is_approved BOOLEAN DEFAULT FALSE,
                                 helpful_count INTEGER DEFAULT 0,
                                 created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Shopping Cart
CREATE TABLE shopping_cart (
                               cart_id SERIAL PRIMARY KEY,
                               user_id INTEGER NOT NULL,
                               product_id INTEGER NOT NULL,
                               quantity INTEGER NOT NULL DEFAULT 1,
                               unit_price DECIMAL(10,2) NOT NULL,
                               created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                               modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Orders
CREATE TABLE orders (
                        order_id SERIAL PRIMARY KEY,
                        user_id INTEGER NOT NULL,
                        order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        required_date TIMESTAMP,
                        shipped_date TIMESTAMP,
                        shipping_address_id INTEGER NOT NULL,
                        billing_address_id INTEGER NOT NULL,
                        sub_total DECIMAL(12,2) NOT NULL,
                        tax_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
                        shipping_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
                        total_amount DECIMAL(12,2) NOT NULL,
                        order_status VARCHAR(20) NOT NULL DEFAULT 'Pending',
                        payment_status VARCHAR(20) NOT NULL DEFAULT 'Pending',
                        tracking_number VARCHAR(50),
                        notes TEXT
);

-- Order Details
CREATE TABLE order_details (
                               order_detail_id SERIAL PRIMARY KEY,
                               order_id INTEGER NOT NULL,
                               product_id INTEGER NOT NULL,
                               quantity INTEGER NOT NULL,
                               unit_price DECIMAL(10,2) NOT NULL,
                               discount DECIMAL(5,2) DEFAULT 0,
                               line_total DECIMAL(12,2)
);

-- Payment Methods
CREATE TABLE payment_methods (
                                 payment_method_id SERIAL PRIMARY KEY,
                                 user_id INTEGER NOT NULL,
                                 payment_type VARCHAR(20) NOT NULL CHECK (payment_type IN ('Credit Card', 'PayPal', 'Bank Transfer')),
                                 card_number VARCHAR(20),
                                 card_holder_name VARCHAR(100),
                                 expiry_month INTEGER CHECK (expiry_month BETWEEN 1 AND 12),
                                 expiry_year INTEGER,
                                 is_default BOOLEAN DEFAULT FALSE,
                                 is_active BOOLEAN DEFAULT TRUE,
                                 created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Payments
CREATE TABLE payments (
                          payment_id SERIAL PRIMARY KEY,
                          order_id INTEGER NOT NULL,
                          payment_method_id INTEGER NOT NULL,
                          amount DECIMAL(12,2) NOT NULL,
                          payment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                          payment_status VARCHAR(20) NOT NULL DEFAULT 'Pending',
                          transaction_id VARCHAR(100),
                          processor_response TEXT,
                          created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Inventory Transactions
CREATE TABLE inventory_transactions (
                                        transaction_id SERIAL PRIMARY KEY,
                                        product_id INTEGER NOT NULL,
                                        transaction_type VARCHAR(20) NOT NULL CHECK (transaction_type IN ('Purchase', 'Sale', 'Return', 'Adjustment')),
                                        quantity INTEGER NOT NULL,
                                        unit_price DECIMAL(10,2),
                                        reference_id INTEGER,
                                        notes TEXT,
                                        created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                        created_by INTEGER NOT NULL
);

-- Audit Log
CREATE TABLE audit_log (
                           audit_id SERIAL PRIMARY KEY,
                           table_name VARCHAR(100) NOT NULL,
                           record_id INTEGER NOT NULL,
                           action VARCHAR(10) NOT NULL CHECK (action IN ('INSERT', 'UPDATE', 'DELETE')),
    old_values JSONB,
    new_values JSONB,
    changed_by INTEGER,
    change_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- FOREIGN KEY CONSTRAINTS
-- =============================================

-- User Addresses
ALTER TABLE user_addresses
    ADD CONSTRAINT fk_user_addresses_users FOREIGN KEY (user_id) REFERENCES users(user_id);

-- Categories (Self-referencing)
ALTER TABLE categories
    ADD CONSTRAINT fk_categories_parent_category FOREIGN KEY (parent_category_id) REFERENCES categories(category_id);

-- Products
ALTER TABLE products
    ADD CONSTRAINT fk_products_categories FOREIGN KEY (category_id) REFERENCES categories(category_id);

ALTER TABLE products
    ADD CONSTRAINT fk_products_suppliers FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id);

-- Product Reviews
ALTER TABLE product_reviews
    ADD CONSTRAINT fk_product_reviews_products FOREIGN KEY (product_id) REFERENCES products(product_id);

ALTER TABLE product_reviews
    ADD CONSTRAINT fk_product_reviews_users FOREIGN KEY (user_id) REFERENCES users(user_id);

-- Shopping Cart
ALTER TABLE shopping_cart
    ADD CONSTRAINT fk_shopping_cart_users FOREIGN KEY (user_id) REFERENCES users(user_id);

ALTER TABLE shopping_cart
    ADD CONSTRAINT fk_shopping_cart_products FOREIGN KEY (product_id) REFERENCES products(product_id);

-- Orders
ALTER TABLE orders
    ADD CONSTRAINT fk_orders_users FOREIGN KEY (user_id) REFERENCES users(user_id);

ALTER TABLE orders
    ADD CONSTRAINT fk_orders_shipping_address FOREIGN KEY (shipping_address_id) REFERENCES user_addresses(address_id);

ALTER TABLE orders
    ADD CONSTRAINT fk_orders_billing_address FOREIGN KEY (billing_address_id) REFERENCES user_addresses(address_id);

-- Order Details
ALTER TABLE order_details
    ADD CONSTRAINT fk_order_details_orders FOREIGN KEY (order_id) REFERENCES orders(order_id);

ALTER TABLE order_details
    ADD CONSTRAINT fk_order_details_products FOREIGN KEY (product_id) REFERENCES products(product_id);

-- Payment Methods
ALTER TABLE payment_methods
    ADD CONSTRAINT fk_payment_methods_users FOREIGN KEY (user_id) REFERENCES users(user_id);

-- Payments
ALTER TABLE payments
    ADD CONSTRAINT fk_payments_orders FOREIGN KEY (order_id) REFERENCES orders(order_id);

ALTER TABLE payments
    ADD CONSTRAINT fk_payments_payment_methods FOREIGN KEY (payment_method_id) REFERENCES payment_methods(payment_method_id);

-- Inventory Transactions
ALTER TABLE inventory_transactions
    ADD CONSTRAINT fk_inventory_transactions_products FOREIGN KEY (product_id) REFERENCES products(product_id);

ALTER TABLE inventory_transactions
    ADD CONSTRAINT fk_inventory_transactions_users FOREIGN KEY (created_by) REFERENCES users(user_id);

-- =============================================
-- INDEXES
-- =============================================

-- Users
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_last_login_date ON users(last_login_date);

-- Products
CREATE INDEX idx_products_category_id ON products(category_id);
CREATE INDEX idx_products_supplier_id ON products(supplier_id);
CREATE INDEX idx_products_price ON products(unit_price);
CREATE INDEX idx_products_stock ON products(units_in_stock);
CREATE INDEX idx_products_name ON products USING gin(to_tsvector('english', product_name));

-- Orders
CREATE INDEX idx_orders_user_id ON orders(user_id);
CREATE INDEX idx_orders_order_date ON orders(order_date);
CREATE INDEX idx_orders_status ON orders(order_status);
CREATE INDEX idx_orders_tracking_number ON orders(tracking_number);

-- Order Details
CREATE INDEX idx_order_details_order_id ON order_details(order_id);
CREATE INDEX idx_order_details_product_id ON order_details(product_id);

-- Shopping Cart
CREATE INDEX idx_shopping_cart_user_id ON shopping_cart(user_id);
CREATE INDEX idx_shopping_cart_product_id ON shopping_cart(product_id);

-- Product Reviews
CREATE INDEX idx_product_reviews_product_id ON product_reviews(product_id);
CREATE INDEX idx_product_reviews_user_id ON product_reviews(user_id);
CREATE INDEX idx_product_reviews_rating ON product_reviews(rating);

-- Inventory Transactions
CREATE INDEX idx_inventory_transactions_product_id ON inventory_transactions(product_id);
CREATE INDEX idx_inventory_transactions_date ON inventory_transactions(created_date);

-- Audit Log
CREATE INDEX idx_audit_log_table_name ON audit_log(table_name);
CREATE INDEX idx_audit_log_record_id ON audit_log(record_id);
CREATE INDEX idx_audit_log_date ON audit_log(change_date);

-- =============================================
-- TRIGGER FUNCTIONS
-- =============================================

-- Function to update modified_date
CREATE OR REPLACE FUNCTION update_modified_date()
RETURNS TRIGGER AS $$
BEGIN
    NEW.modified_date = CURRENT_TIMESTAMP;
RETURN NEW;
END;

$$
LANGUAGE plpgsql;

-- Function to calculate line totals
CREATE OR REPLACE FUNCTION calculate_line_total()
RETURNS TRIGGER AS 
$$

BEGIN
    NEW.line_total = NEW.quantity * NEW.unit_price * (1 - COALESCE(NEW.discount, 0));
RETURN NEW;
END;

$$
LANGUAGE plpgsql;

-- Function for audit logging
CREATE OR REPLACE FUNCTION audit_trigger_function()
RETURNS TRIGGER AS 
$$

DECLARE
record_id_val INTEGER;
BEGIN
    -- Get the appropriate record ID based on the table
    IF TG_TABLE_NAME = 'users' THEN
        record_id_val = COALESCE(NEW.user_id, OLD.user_id);
    ELSIF TG_TABLE_NAME = 'products' THEN
        record_id_val = COALESCE(NEW.product_id, OLD.product_id);
    ELSIF TG_TABLE_NAME = 'orders' THEN
        record_id_val = COALESCE(NEW.order_id, OLD.order_id);
ELSE
        record_id_val = 0; -- Default fallback
END IF;

    IF TG_OP = 'DELETE' THEN
        INSERT INTO audit_log (table_name, record_id, action, old_values, changed_by)
        VALUES (TG_TABLE_NAME, record_id_val, 'DELETE', to_jsonb(OLD), 1);
RETURN OLD;
ELSIF TG_OP = 'UPDATE' THEN
        INSERT INTO audit_log (table_name, record_id, action, old_values, new_values, changed_by)
        VALUES (TG_TABLE_NAME, record_id_val, 'UPDATE', to_jsonb(OLD), to_jsonb(NEW), 1);
RETURN NEW;
ELSIF TG_OP = 'INSERT' THEN
        INSERT INTO audit_log (table_name, record_id, action, new_values, changed_by)
        VALUES (TG_TABLE_NAME, record_id_val, 'INSERT', to_jsonb(NEW), 1);
RETURN NEW;
END IF;
RETURN NULL;
END;

$$
LANGUAGE plpgsql;

-- Function for order total calculation
CREATE OR REPLACE FUNCTION calculate_order_totals()
RETURNS TRIGGER AS 
$$

DECLARE
v_order_id INTEGER;
BEGIN
    v_order_id = COALESCE(NEW.order_id, OLD.order_id);

UPDATE orders
SET sub_total = (
    SELECT COALESCE(SUM(line_total), 0)
    FROM order_details
    WHERE order_id = v_order_id
)
WHERE order_id = v_order_id;

UPDATE orders
SET total_amount = sub_total + tax_amount + shipping_amount
WHERE order_id = v_order_id;

RETURN COALESCE(NEW, OLD);
END;

$$
LANGUAGE plpgsql;

-- =============================================
-- TRIGGERS
-- =============================================

-- Modified date triggers
CREATE TRIGGER tr_users_update_modified_date
    BEFORE UPDATE ON users
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_date();

CREATE TRIGGER tr_products_update_modified_date
    BEFORE UPDATE ON products
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_date();

CREATE TRIGGER tr_categories_update_modified_date
    BEFORE UPDATE ON categories
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_date();

CREATE TRIGGER tr_shopping_cart_update_modified_date
    BEFORE UPDATE ON shopping_cart
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_date();

-- Line total calculation trigger
CREATE TRIGGER tr_order_details_calculate_line_total
    BEFORE INSERT OR UPDATE ON order_details
                         FOR EACH ROW
                         EXECUTE FUNCTION calculate_line_total();

-- Order totals trigger
CREATE TRIGGER tr_order_details_calculate_totals
    AFTER INSERT OR UPDATE OR DELETE ON order_details
    FOR EACH ROW
    EXECUTE FUNCTION calculate_order_totals();

-- Audit triggers
CREATE TRIGGER tr_users_audit
    AFTER INSERT OR UPDATE OR DELETE ON users
    FOR EACH ROW
    EXECUTE FUNCTION audit_trigger_function();

CREATE TRIGGER tr_products_audit
    AFTER INSERT OR UPDATE OR DELETE ON products
    FOR EACH ROW
    EXECUTE FUNCTION audit_trigger_function();

CREATE TRIGGER tr_orders_audit
    AFTER INSERT OR UPDATE OR DELETE ON orders
    FOR EACH ROW
    EXECUTE FUNCTION audit_trigger_function();

-- Inventory update trigger (equivalent to SQL Server)
CREATE OR REPLACE FUNCTION update_inventory_on_order()
RETURNS TRIGGER AS $$
BEGIN
    -- Update product stock
    UPDATE products
    SET units_in_stock = units_in_stock - NEW.quantity
    WHERE product_id = NEW.product_id;

    -- Log inventory transaction
    INSERT INTO inventory_transactions (product_id, transaction_type, quantity, unit_price, reference_id, created_by)
    SELECT
        NEW.product_id,
        'Sale',
        -NEW.quantity,
        NEW.unit_price,
        NEW.order_id,
        o.user_id
    FROM orders o
    WHERE o.order_id = NEW.order_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_order_details_update_inventory
    AFTER INSERT ON order_details
    FOR EACH ROW
    EXECUTE FUNCTION update_inventory_on_order();

-- =============================================
-- CORRECTED STORED PROCEDURES
-- =============================================

-- Create New Order Function
CREATE OR REPLACE FUNCTION create_order(
    p_UserID INTEGER,
    p_ShippingAddressID INTEGER,
    p_BillingAddressID INTEGER,
    p_TaxAmount DECIMAL(12,2) DEFAULT 0,
    p_ShippingAmount DECIMAL(12,2) DEFAULT 0,
    OUT p_OrderID INTEGER
)
RETURNS INTEGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- Start transaction (implicit in function)
    
    -- Create the order
    INSERT INTO Orders (UserID, ShippingAddressID, BillingAddressID, SubTotal, TaxAmount, ShippingAmount, TotalAmount)
    VALUES (p_UserID, p_ShippingAddressID, p_BillingAddressID, 0, p_TaxAmount, p_ShippingAmount, p_TaxAmount + p_ShippingAmount)
        RETURNING OrderID INTO p_OrderID;
    
    -- Move items from shopping cart to order details
    INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
    SELECT
        p_OrderID,
        sc.ProductID,
        sc.Quantity,
        sc.UnitPrice
    FROM ShoppingCart sc
    WHERE sc.UserID = p_UserID;
    
    -- Clear shopping cart
    DELETE FROM ShoppingCart WHERE UserID = p_UserID;
    
    -- Transaction commits automatically on successful completion
    -- Rolls back automatically on exception
    
    EXCEPTION
        WHEN OTHERS THEN
            -- Re-raise the exception (transaction will be rolled back automatically)
            RAISE;
END;
$$;
    
-- Fix: Add Product to Cart (corrected parameter defaults)
CREATE OR REPLACE FUNCTION add_to_cart(
    p_user_id INTEGER,
    p_product_id INTEGER,
    p_quantity INTEGER DEFAULT 1
) RETURNS VOID AS $$
BEGIN
    -- Check if item already exists in cart
    IF EXISTS (SELECT 1 FROM shopping_cart WHERE user_id = p_user_id AND product_id = p_product_id) THEN
        -- Update quantity
        UPDATE shopping_cart
        SET quantity = quantity + p_quantity,
            modified_date = CURRENT_TIMESTAMP
        WHERE user_id = p_user_id AND product_id = p_product_id;
    ELSE
        -- Add new item
        INSERT INTO shopping_cart (user_id, product_id, quantity)
        VALUES (p_user_id, p_product_id, p_quantity);
    END IF;
END;

$$
LANGUAGE plpgsql;

-- Fix: Update User Profile (corrected parameter defaults)
CREATE OR REPLACE FUNCTION update_user_profile(
    p_user_id INTEGER,
    p_first_name VARCHAR(50) DEFAULT NULL,
    p_last_name VARCHAR(50) DEFAULT NULL,
    p_email VARCHAR(100) DEFAULT NULL,
    p_phone VARCHAR(15) DEFAULT NULL
) RETURNS BOOLEAN AS 
$$

BEGIN
UPDATE users
SET
    first_name = COALESCE(p_first_name, first_name),
    last_name = COALESCE(p_last_name, last_name),
    email = COALESCE(p_email, email),
    phone_number = COALESCE(p_phone, phone_number),
    modified_date = CURRENT_TIMESTAMP
WHERE user_id = p_user_id;

RETURN FOUND;
END;

$$
LANGUAGE plpgsql;

-- Fix: Search Products (corrected parameter defaults)
CREATE OR REPLACE FUNCTION search_products(
    p_search_term VARCHAR(255) DEFAULT NULL,
    p_category_id INTEGER DEFAULT NULL,
    p_min_price DECIMAL(10,2) DEFAULT NULL,
    p_max_price DECIMAL(10,2) DEFAULT NULL,
    p_limit INTEGER DEFAULT 50,
    p_offset INTEGER DEFAULT 0
) RETURNS TABLE (
    product_id INTEGER,
    product_name VARCHAR(255),
    description TEXT,
    unit_price DECIMAL(10,2),
    category_name VARCHAR(100),
    supplier_name VARCHAR(100),
    units_in_stock INTEGER
) AS 
$$

BEGIN
RETURN QUERY
    SELECT
        p.product_id,
        p.product_name,
        p.description,
        p.unit_price,
        c.category_name,
        s.supplier_name,
        p.units_in_stock
    FROM products p
             INNER JOIN categories c ON p.category_id = c.category_id
             INNER JOIN suppliers s ON p.supplier_id = s.supplier_id
    WHERE
        p.discontinued = FALSE
      AND (p_search_term IS NULL OR
           p.product_name ILIKE '%' || p_search_term || '%' OR 
                 p.description ILIKE '%' || p_search_term || '%')
      AND (p_category_id IS NULL OR p.category_id = p_category_id)
      AND (p_min_price IS NULL OR p.unit_price >= p_min_price)
      AND (p_max_price IS NULL OR p.unit_price <= p_max_price)
    ORDER BY p.product_name
        LIMIT p_limit OFFSET p_offset;
END;

$$
LANGUAGE plpgsql;

-- Fix: Get Sales Report (corrected parameter defaults)
CREATE OR REPLACE FUNCTION get_sales_report(
    p_start_date DATE DEFAULT NULL,
    p_end_date DATE DEFAULT NULL,
    p_category_id INTEGER DEFAULT NULL
) RETURNS TABLE (
    product_id INTEGER,
    product_name VARCHAR(255),
    category_name VARCHAR(100),
    total_quantity BIGINT,
    total_revenue DECIMAL(15,2),
    order_count BIGINT
) AS 
$$

BEGIN
RETURN QUERY
    SELECT
        p.product_id,
        p.product_name,
        c.category_name,
        SUM(od.quantity) as total_quantity,
        SUM(od.unit_price * od.quantity) as total_revenue,
        COUNT(DISTINCT o.order_id) as order_count
    FROM products p
             INNER JOIN categories c ON p.category_id = c.category_id
             INNER JOIN order_details od ON p.product_id = od.product_id
             INNER JOIN orders o ON od.order_id = o.order_id
    WHERE
        o.order_status = 'Completed'
      AND (p_start_date IS NULL OR o.order_date >= p_start_date)
      AND (p_end_date IS NULL OR o.order_date <= p_end_date)
      AND (p_category_id IS NULL OR p.category_id = p_category_id)
    GROUP BY p.product_id, p.product_name, c.category_name
    ORDER BY total_revenue DESC;
END;

$$
LANGUAGE plpgsql;

-- Fix: Process Order (corrected parameter defaults)
CREATE OR REPLACE FUNCTION process_order(
    p_user_id INTEGER,
    p_shipping_address_id INTEGER,
    p_payment_method VARCHAR(20) DEFAULT 'Credit Card'
) RETURNS INTEGER AS 
$$

DECLARE
v_order_id INTEGER;
    v_total_amount DECIMAL(10,2) := 0;
    cart_item RECORD;
BEGIN
    -- Create order
    INSERT INTO orders (user_id, order_date, order_status, payment_method, payment_status)
    VALUES (p_user_id, CURRENT_TIMESTAMP, 'Pending', p_payment_method, 'Pending')
        RETURNING order_id INTO v_order_id;
    
    -- Add order details from cart
    FOR cart_item IN
    SELECT sc.product_id, sc.quantity, p.unit_price
    FROM shopping_cart sc
             INNER JOIN products p ON sc.product_id = p.product_id
    WHERE sc.user_id = p_user_id
        LOOP
    INSERT INTO order_details (order_id, product_id, quantity, unit_price)
    VALUES (v_order_id, cart_item.product_id, cart_item.quantity, cart_item.unit_price);
    
    v_total_amount := v_total_amount + (cart_item.quantity * cart_item.unit_price);
    END LOOP;
        
        -- Update order total
    UPDATE orders SET total_amount = v_total_amount WHERE order_id = v_order_id;
    
    -- Clear cart
    DELETE FROM shopping_cart WHERE user_id = p_user_id;
    
    RETURN v_order_id;
END;

$$
LANGUAGE plpgsql;

-- Fix: Update Inventory (corrected parameter defaults)
CREATE OR REPLACE FUNCTION update_inventory(
    p_product_id INTEGER,
    p_quantity_change INTEGER,
    p_operation VARCHAR(10) DEFAULT 'ADD'
) RETURNS BOOLEAN AS 
$$

BEGIN
    IF p_operation = 'ADD' THEN
        UPDATE products
        SET units_in_stock = units_in_stock + p_quantity_change,
            modified_date = CURRENT_TIMESTAMP
        WHERE product_id = p_product_id;
    ELSIF p_operation = 'SUBTRACT' THEN
        UPDATE products
        SET units_in_stock = units_in_stock - p_quantity_change,
            modified_date = CURRENT_TIMESTAMP
        WHERE product_id = p_product_id AND units_in_stock >= p_quantity_change;
    END IF;
        
    RETURN FOUND;
END;

$$
LANGUAGE plpgsql;

-- Fix: Get User Cart (no parameters issue here, but included for completeness)
CREATE OR REPLACE FUNCTION get_user_cart(p_user_id INTEGER)
RETURNS TABLE (
    product_id INTEGER,
    product_name VARCHAR(255),
    unit_price DECIMAL(10,2),
    quantity INTEGER,
    subtotal DECIMAL(10,2)
) AS 
$$

BEGIN
RETURN QUERY
    SELECT
        p.product_id,
        p.product_name,
        p.unit_price,
        sc.quantity,
        (p.unit_price * sc.quantity) as subtotal
    FROM shopping_cart sc
             INNER JOIN products p ON sc.product_id = p.product_id
    WHERE sc.user_id = p_user_id;
END;
$$ LANGUAGE plpgsql;


-- =============================================
-- VIEWS
-- =============================================

-- Product Catalog View
CREATE VIEW vw_product_catalog AS
SELECT
    p.product_id,
    p.product_name,
    p.description,
    p.sku,
    p.unit_price,
    p.units_in_stock,
    p.image_url,
    c.category_name,
    s.supplier_name,
    COALESCE(AVG(pr.rating), 0)::DECIMAL(3,2) AS average_rating,
    COUNT(pr.review_id) AS review_count
FROM products p
         INNER JOIN categories c ON p.category_id = c.category_id
         INNER JOIN suppliers s ON p.supplier_id = s.supplier_id
         LEFT JOIN product_reviews pr ON p.product_id = pr.product_id AND pr.is_approved = TRUE
WHERE p.discontinued = FALSE
GROUP BY p.product_id, p.product_name, p.description, p.sku, p.unit_price, p.units_in_stock, p.image_url, c.category_name, s.supplier_name;

-- Order Summary View
CREATE VIEW vw_order_summary AS
SELECT
    o.order_id,
    o.order_date,
    o.order_status,
    o.payment_status,
    (u.first_name || ' ' || u.last_name) AS customer_name,
    u.email AS customer_email,
    COUNT(od.order_detail_id) AS item_count,
    o.total_amount,
    o.tracking_number
FROM orders o
         INNER JOIN users u ON o.user_id = u.user_id
         LEFT JOIN order_details od ON o.order_id = od.order_id
GROUP BY o.order_id, o.order_date, o.order_status, o.payment_status, u.first_name, u.last_name, u.email, o.total_amount, o.tracking_number;

-- =============================================
-- SPECIAL CASES
-- =============================================

CREATE TABLE Brands (
        Id UUID NOT NULL,
        Name TEXT NOT NULL,
        IsActive BOOLEAN NOT NULL,
        CONSTRAINT PK_Brands PRIMARY KEY (Id)
);

CREATE TYPE BrandType AS (
    Name TEXT,
    IsActive BOOLEAN
);

CREATE OR REPLACE FUNCTION InsertBrand(brand_data BrandType[])
RETURNS VOID AS $$
BEGIN
INSERT INTO Brands (Id, Name, IsActive)
SELECT
    gen_random_uuid() AS Id,
    (brand_item).Name,
        (brand_item).IsActive
FROM unnest(brand_data) AS brand_item;
END;
$$ 
LANGUAGE plpgsql;

CREATE TABLE FKTable (
         FKTableId SERIAL PRIMARY KEY,
         Name VARCHAR(100) NOT NULL
);

CREATE TABLE PrimaryTable (
          PrimaryTableId SERIAL PRIMARY KEY,
          Name VARCHAR(100) NOT NULL,
          FKTableId1 INT NOT NULL,
          FKWithTableId2 INT NOT NULL,
          FKAsTableId3 INT NOT NULL,
          FKTryTableId4 INT NOT NULL,
          FKThisTableId5 INT NOT NULL,
          CONSTRAINT FK_PrimaryTable_FKTable1 FOREIGN KEY (FKTableId1) REFERENCES FKTable(FKTableId),
          CONSTRAINT FK_PrimaryTable_FKWithTableId2 FOREIGN KEY (FKWithTableId2) REFERENCES FKTable(FKTableId),
          CONSTRAINT FK_PrimaryTable_FKAsTableId3 FOREIGN KEY (FKAsTableId3) REFERENCES FKTable(FKTableId),
          CONSTRAINT FK_PrimaryTable_FKTryTableId4 FOREIGN KEY (FKTryTableId4) REFERENCES FKTable(FKTableId),
          CONSTRAINT FK_PrimaryTable_FKThisTableId5 FOREIGN KEY (FKThisTableId5) REFERENCES FKTable(FKTableId)
);

CREATE TABLE SelfReferenceTable (
        ID UUID NOT NULL,
        Name VARCHAR(50) NOT NULL,
        Email VARCHAR(50),
        ManagerId UUID,
        CONSTRAINT PK_SelfReferenceTable PRIMARY KEY (ID),
        CONSTRAINT FK_SelfReferenceTable_SelfReferenceTable FOREIGN KEY (ManagerId) REFERENCES SelfReferenceTable(ID)
);

-- Add expiry month constraint to payment_methods
ALTER TABLE payment_methods 
ADD CONSTRAINT chk_payment_methods_expiry_month 
CHECK (expiry_month BETWEEN 1 AND 12);

-- Get User Profile (equivalent to SQL Server procedure)
CREATE OR REPLACE FUNCTION get_user_profile(p_user_id INTEGER)
RETURNS TABLE (
    user_id INTEGER,
    username VARCHAR(50),
    email VARCHAR(100),
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    date_of_birth DATE,
    phone_number VARCHAR(15),
    is_active BOOLEAN,
    created_date TIMESTAMP,
    last_login_date TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        u.user_id,
        u.username,
        u.email,
        u.first_name,
        u.last_name,
        u.date_of_birth,
        u.phone_number,
        u.is_active,
        u.created_date,
        u.last_login_date
    FROM users u
    WHERE u.user_id = p_user_id AND u.is_active = TRUE;
END;
$$ LANGUAGE plpgsql;

-- Get User Addresses (companion function)
CREATE OR REPLACE FUNCTION get_user_addresses(p_user_id INTEGER)
RETURNS TABLE (
    address_id INTEGER,
    address_type VARCHAR(20),
    address_line1 VARCHAR(255),
    address_line2 VARCHAR(255),
    city VARCHAR(100),
    state VARCHAR(50),
    zip_code VARCHAR(10),
    country VARCHAR(50),
    is_default BOOLEAN
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        ua.address_id,
        ua.address_type,
        ua.address_line1,
        ua.address_line2,
        ua.city,
        ua.state,
        ua.zip_code,
        ua.country,
        ua.is_default
    FROM user_addresses ua
    WHERE ua.user_id = p_user_id;
END;
$$ LANGUAGE plpgsql;

-- Get Order Details (equivalent to SQL Server procedure)
CREATE OR REPLACE FUNCTION get_order_details(p_order_id INTEGER)
RETURNS TABLE (
    order_id INTEGER,
    order_date TIMESTAMP,
    order_status VARCHAR(20),
    payment_status VARCHAR(20),
    sub_total DECIMAL(12,2),
    tax_amount DECIMAL(12,2),
    shipping_amount DECIMAL(12,2),
    total_amount DECIMAL(12,2),
    tracking_number VARCHAR(50),
    customer_name TEXT,
    customer_email VARCHAR(100),
    shipping_address1 VARCHAR(255),
    shipping_address2 VARCHAR(255),
    shipping_city VARCHAR(100),
    shipping_state VARCHAR(50),
    shipping_zip VARCHAR(10)
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        o.order_id,
        o.order_date,
        o.order_status,
        o.payment_status,
        o.sub_total,
        o.tax_amount,
        o.shipping_amount,
        o.total_amount,
        o.tracking_number,
        (u.first_name || ' ' || u.last_name) AS customer_name,
        u.email AS customer_email,
        sa.address_line1 AS shipping_address1,
        sa.address_line2 AS shipping_address2,
        sa.city AS shipping_city,
        sa.state AS shipping_state,
        sa.zip_code AS shipping_zip
    FROM orders o
    INNER JOIN users u ON o.user_id = u.user_id
    INNER JOIN user_addresses sa ON o.shipping_address_id = sa.address_id
    WHERE o.order_id = p_order_id;
END;
$$ LANGUAGE plpgsql;

-- Get Order Line Items (companion function)
CREATE OR REPLACE FUNCTION get_order_line_items(p_order_id INTEGER)
RETURNS TABLE (
    order_detail_id INTEGER,
    product_id INTEGER,
    product_name VARCHAR(255),
    sku VARCHAR(50),
    quantity INTEGER,
    unit_price DECIMAL(10,2),
    discount DECIMAL(5,2),
    line_total DECIMAL(12,2)
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        od.order_detail_id,
        od.product_id,
        p.product_name,
        p.sku,
        od.quantity,
        od.unit_price,
        od.discount,
        od.line_total
    FROM order_details od
    INNER JOIN products p ON od.product_id = p.product_id
    WHERE od.order_id = p_order_id;
END;
$$ LANGUAGE plpgsql;

-- Update Product Stock (equivalent to SQL Server procedure)
CREATE OR REPLACE FUNCTION update_product_stock(
    p_product_id INTEGER,
    p_quantity INTEGER,
    p_transaction_type VARCHAR(20),
    p_user_id INTEGER,
    p_unit_price DECIMAL(10,2) DEFAULT NULL,
    p_notes TEXT DEFAULT NULL
) RETURNS VOID AS $$
BEGIN
    -- Update product stock
    UPDATE products
    SET units_in_stock = units_in_stock + p_quantity,
        modified_date = CURRENT_TIMESTAMP
    WHERE product_id = p_product_id;

    -- Log inventory transaction
    INSERT INTO inventory_transactions (product_id, transaction_type, quantity, unit_price, notes, created_by)
    VALUES (p_product_id, p_transaction_type, p_quantity, p_unit_price, p_notes, p_user_id);
END;
$$ LANGUAGE plpgsql;

-- =============================================
-- COMPREHENSIVE DATA TYPES TEST TABLE
-- =============================================

CREATE TABLE data_types_test_table (
    -- Primary Key
    id SERIAL PRIMARY KEY,
    
    -- Numeric Types
    boolean_column BOOLEAN,
    smallint_column SMALLINT,
    integer_column INTEGER,
    bigint_column BIGINT,
    decimal_column DECIMAL(18,2),
    numeric_column NUMERIC(10,4),
    real_column REAL,
    double_precision_column DOUBLE PRECISION,
    
    -- Serial Types (Auto-incrementing)
    smallserial_column SMALLSERIAL,
    serial_column SERIAL,
    bigserial_column BIGSERIAL,
    
    -- Monetary Type
    money_column MONEY,
    
    -- Character Types
    char_column CHAR(10),
    varchar_column VARCHAR(255),
    text_column TEXT,
    
    -- Binary Data Types
    bytea_column BYTEA,
    
    -- Date/Time Types
    date_column DATE,
    time_column TIME,
    time_with_tz_column TIME WITH TIME ZONE,
    timestamp_column TIMESTAMP,
    timestamp_with_tz_column TIMESTAMP WITH TIME ZONE,
    interval_column INTERVAL,
    
    -- Network Address Types
    inet_column INET,
    cidr_column CIDR,
    macaddr_column MACADDR,
    macaddr8_column MACADDR8,
    
    -- Bit String Types
    bit_column BIT(8),
    bit_varying_column BIT VARYING(64),
    
    -- UUID Type
    uuid_column UUID DEFAULT gen_random_uuid(),
    
    -- JSON Types
    json_column JSON,
    jsonb_column JSONB,
    
    -- XML Type
    xml_column XML,
    
    -- Geometric Types
    point_column POINT,
    line_column LINE,
    lseg_column LSEG,
    box_column BOX,
    path_column PATH,
    polygon_column POLYGON,
    circle_column CIRCLE,
    
    -- Range Types
    int4range_column INT4RANGE,
    int8range_column INT8RANGE,
    numrange_column NUMRANGE,
    tsrange_column TSRANGE,
    tstzrange_column TSTZRANGE,
    daterange_column DATERANGE,
    
    -- Array Types
    integer_array_column INTEGER[],
    text_array_column TEXT[],
    varchar_array_column VARCHAR(50)[],
    
    -- Composite Types
    -- We'll create a custom type first
    
    -- Special PostgreSQL Types
    tsvector_column TSVECTOR,
    tsquery_column TSQUERY,
    
    -- Constraints and Defaults
    email_column VARCHAR(100) CHECK (email_column ~ '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    age_column INTEGER CHECK (age_column >= 0 AND age_column <= 150),
    status_column CHAR(1) DEFAULT 'A' CHECK (status_column IN ('A', 'I', 'P')),
    
    -- Nullable and Non-nullable variants
    required_text VARCHAR(50) NOT NULL,
    optional_text VARCHAR(50),
    
    -- Columns with Default Values
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    
    -- Large precision types
    big_decimal_column DECIMAL(38,10),
    precise_numeric_column NUMERIC(28,8),
    
    -- Computed/Generated columns (PostgreSQL 12+)
    full_name TEXT GENERATED ALWAYS AS (varchar_column || ' - ' || text_column) STORED,
    
    -- Unique constraints
    CONSTRAINT uq_data_types_test_table_uuid UNIQUE (uuid_column)
);

-- Create a custom composite type for demonstration
CREATE TYPE address_type AS (
    street VARCHAR(100),
    city VARCHAR(50),
    state VARCHAR(50),
    zip_code VARCHAR(10),
    country VARCHAR(50)
);

-- Add a column using the composite type
ALTER TABLE data_types_test_table ADD COLUMN address_composite address_type;

-- Create indexes for the comprehensive table
CREATE INDEX idx_data_types_test_table_timestamp ON data_types_test_table(timestamp_column);
CREATE INDEX idx_data_types_test_table_varchar ON data_types_test_table(varchar_column);
CREATE INDEX idx_data_types_test_table_status ON data_types_test_table(status_column);
CREATE INDEX idx_data_types_test_table_is_active ON data_types_test_table(is_active);

-- GIN indexes for arrays and JSON
CREATE INDEX idx_data_types_test_table_integer_array ON data_types_test_table USING GIN(integer_array_column);
CREATE INDEX idx_data_types_test_table_jsonb ON data_types_test_table USING GIN(jsonb_column);
CREATE INDEX idx_data_types_test_table_tsvector ON data_types_test_table USING GIN(tsvector_column);

-- GiST indexes for geometric types
CREATE INDEX idx_data_types_test_table_point ON data_types_test_table USING GIST(point_column);
CREATE INDEX idx_data_types_test_table_box ON data_types_test_table USING GIST(box_column);

-- =============================================
-- COMPREHENSIVE PARAMETER TYPES FUNCTIONS
-- =============================================

-- Function with various input parameter types and OUT parameters
CREATE OR REPLACE FUNCTION comprehensive_parameter_types(
    -- Required input parameters (no defaults)
    input_int INTEGER,
    input_decimal DECIMAL(18,2),
    input_varchar VARCHAR(100),
    
    -- Optional input parameters (all have defaults)
    input_bigint BIGINT DEFAULT NULL,
    input_real REAL DEFAULT 0.0,
    input_boolean BOOLEAN DEFAULT TRUE,
    input_text TEXT DEFAULT 'Default Value',
    input_char CHAR(5) DEFAULT 'DEFLT',
    input_date DATE DEFAULT NULL,
    input_timestamp TIMESTAMP DEFAULT NULL,
    input_time TIME DEFAULT NULL,
    input_interval INTERVAL DEFAULT NULL,
    input_uuid UUID DEFAULT NULL,
    input_json JSON DEFAULT NULL,
    input_jsonb JSONB DEFAULT NULL,
    input_xml XML DEFAULT NULL,
    input_integer_array INTEGER[] DEFAULT NULL,
    input_text_array TEXT[] DEFAULT NULL,
    input_address address_type DEFAULT NULL,
    input_inet INET DEFAULT NULL,
    input_cidr CIDR DEFAULT NULL,
    input_point POINT DEFAULT NULL,
    input_box BOX DEFAULT NULL,
    
    -- OUT parameters of various types
    OUT output_int INTEGER,
    OUT output_text TEXT,
    OUT output_decimal DECIMAL(18,4),
    OUT output_timestamp TIMESTAMP,
    OUT output_uuid UUID,
    OUT output_json JSONB,
    OUT output_array INTEGER[],
    OUT output_row_count INTEGER
)
RETURNS RECORD
LANGUAGE plpgsql
AS $$
DECLARE
    temp_record RECORD;
BEGIN
    -- Set output values based on inputs
    output_int := input_int * 2;
    output_text := 'Processed: ' || input_varchar || ' - ' || input_text;
    output_decimal := input_decimal * input_real;
    output_timestamp := CURRENT_TIMESTAMP;
    output_uuid := gen_random_uuid();
    
    -- Process JSON input
    IF input_jsonb IS NOT NULL THEN
        output_json := jsonb_build_object(
            'original', input_jsonb,
            'processed_at', CURRENT_TIMESTAMP,
            'input_int', input_int
        );
    ELSE
        output_json := jsonb_build_object('status', 'no_json_input');
    END IF;
    
    -- Process array input
    IF input_integer_array IS NOT NULL THEN
        output_array := array(SELECT unnest(input_integer_array) * 2);
    ELSE
        output_array := ARRAY[input_int, input_int * 2, input_int * 3];
    END IF;
    
    -- Count something for demonstration
    SELECT COUNT(*) INTO output_row_count FROM users WHERE is_active = input_boolean;
    
    -- Insert test data if address provided
    IF input_address IS NOT NULL THEN
        INSERT INTO data_types_test_table (
            varchar_column, 
            address_composite, 
            integer_column
        ) VALUES (
            'Test from function',
            input_address,
            input_int
        );
        GET DIAGNOSTICS output_row_count = ROW_COUNT;
    END IF;
END;
$$;

-- Function demonstrating multiple result sets (using SETOF)
CREATE OR REPLACE FUNCTION multiple_result_sets(
    category_filter VARCHAR(100) DEFAULT NULL,
    include_inactive BOOLEAN DEFAULT FALSE,
    top_count INTEGER DEFAULT 10
)
RETURNS TABLE (
    product_id INTEGER,
    product_name VARCHAR(255),
    unit_price DECIMAL(10,2),
    category_name VARCHAR(100),
    units_in_stock INTEGER,
    discontinued BOOLEAN,
    -- Summary fields (will be the same for all rows)
    total_product_count BIGINT,
    total_categories BIGINT,
    average_price DECIMAL(10,2)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_total_product_count BIGINT;
    v_total_categories BIGINT;
    v_average_price DECIMAL(10,2);
BEGIN
    -- Calculate summary statistics first
    SELECT COUNT(*) INTO v_total_product_count
    FROM products p
    INNER JOIN categories c ON p.category_id = c.category_id
    WHERE (category_filter IS NULL OR c.category_name ILIKE '%' || category_filter || '%')
      AND (include_inactive = TRUE OR p.discontinued = FALSE);
      
    SELECT COUNT(DISTINCT c.category_id) INTO v_total_categories
    FROM categories c
    WHERE (category_filter IS NULL OR c.category_name ILIKE '%' || category_filter || '%');
    
    SELECT AVG(p.unit_price) INTO v_average_price
    FROM products p
    INNER JOIN categories c ON p.category_id = c.category_id
    WHERE (category_filter IS NULL OR c.category_name ILIKE '%' || category_filter || '%')
      AND (include_inactive = TRUE OR p.discontinued = FALSE);
    
    -- Return product records with summary data
    RETURN QUERY
    SELECT 
        p.product_id,
        p.product_name,
        p.unit_price,
        c.category_name,
        p.units_in_stock,
        p.discontinued,
        v_total_product_count,
        v_total_categories,
        v_average_price
    FROM products p
    INNER JOIN categories c ON p.category_id = c.category_id
    WHERE (category_filter IS NULL OR c.category_name ILIKE '%' || category_filter || '%')
      AND (include_inactive = TRUE OR p.discontinued = FALSE)
    ORDER BY p.unit_price DESC
    LIMIT top_count;
END;
$$;

-- Function with complex data types and error handling
CREATE OR REPLACE FUNCTION complex_data_processing(
    -- Geometric parameters
    location_point POINT DEFAULT NULL,
    search_area BOX DEFAULT NULL,
    
    -- JSON parameter for complex input
    config_json JSONB DEFAULT NULL,
    metadata_json JSON DEFAULT NULL,
    
    -- Large text parameters
    description_text TEXT DEFAULT NULL,
    notes_text TEXT DEFAULT NULL,
    
    -- Binary parameter
    image_data BYTEA DEFAULT NULL,
    
    -- Array parameters
    tag_array TEXT[] DEFAULT NULL,
    id_array INTEGER[] DEFAULT NULL,
    
    -- Network parameters
    client_ip INET DEFAULT NULL,
    
    -- Range parameters
    date_range DATERANGE DEFAULT NULL,
    price_range NUMRANGE DEFAULT NULL,
    
    -- OUT parameters
    OUT processed_records INTEGER,
    OUT error_message TEXT,
    OUT processing_time_ms INTEGER,
    OUT result_data JSONB
)
RETURNS RECORD
LANGUAGE plpgsql
AS $$
DECLARE
    start_time TIMESTAMP;
    config_value TEXT;
    temp_count INTEGER;
BEGIN
    start_time := clock_timestamp();
    processed_records := 0;
    error_message := NULL;
    
    BEGIN
        -- Validate JSON if provided
        IF metadata_json IS NOT NULL THEN
            -- Try to extract a value to validate JSON structure
            SELECT metadata_json->>'status' INTO config_value;
        END IF;
        
        -- Process JSONB configuration if provided
        IF config_json IS NOT NULL THEN
            SELECT config_json->>'processing_mode' INTO config_value;
            
            -- Simulate processing based on config
            CASE config_value
                WHEN 'batch' THEN
                    processed_records := processed_records + 100;
                WHEN 'single' THEN
                    processed_records := processed_records + 1;
                ELSE
                    processed_records := processed_records + 10;
            END CASE;
        END IF;
        
        -- Process geometric data if provided
        IF location_point IS NOT NULL THEN
            -- Count products within a certain distance (simplified example)
            SELECT COUNT(*) INTO temp_count
            FROM products p
            WHERE TRUE; -- Placeholder for actual geometric query
            processed_records := processed_records + temp_count;
        END IF;
        
        -- Process array data if provided
        IF tag_array IS NOT NULL THEN
            processed_records := processed_records + array_length(tag_array, 1);
        END IF;
        
        -- Process binary data if provided
        IF image_data IS NOT NULL THEN
            CASE 
                WHEN octet_length(image_data) > 1000000 THEN
                    processed_records := processed_records + 10; -- Large image
                WHEN octet_length(image_data) > 100000 THEN
                    processed_records := processed_records + 5;  -- Medium image
                ELSE
                    processed_records := processed_records + 1;  -- Small image
            END CASE;
        END IF;
        
        -- Process date range if provided
        IF date_range IS NOT NULL THEN
            SELECT COUNT(*) INTO temp_count
            FROM orders o
            WHERE o.order_date::DATE <@ date_range;
            processed_records := processed_records + temp_count;
        END IF;
        
        -- Build result data
        result_data := jsonb_build_object(
            'config_mode', config_value,
            'location_provided', (location_point IS NOT NULL),
            'arrays_processed', (tag_array IS NOT NULL OR id_array IS NOT NULL),
            'image_size_bytes', CASE WHEN image_data IS NOT NULL THEN octet_length(image_data) ELSE NULL END,
            'client_ip', client_ip::TEXT,
            'processing_timestamp', start_time
        );
        
        -- Calculate processing time
        processing_time_ms := EXTRACT(EPOCH FROM (clock_timestamp() - start_time)) * 1000;
        
    EXCEPTION
        WHEN OTHERS THEN
            error_message := SQLERRM;
            processed_records := -1;
            processing_time_ms := EXTRACT(EPOCH FROM (clock_timestamp() - start_time)) * 1000;
            result_data := jsonb_build_object('error', error_message);
    END;
END;
$$;

-- Function demonstrating INOUT pattern using composite types
CREATE OR REPLACE FUNCTION inout_parameter_example(
    INOUT value_data RECORD,
    multiplier DECIMAL(5,2) DEFAULT 2.0,
    operation CHAR(1) DEFAULT 'M' -- 'M'ultiply, 'A'dd, 'S'ubtract
)
RETURNS RECORD
LANGUAGE plpgsql
AS $$
DECLARE
    original_value INTEGER;
    new_value INTEGER;
    operation_count INTEGER;
BEGIN
    -- Extract values from the input record
    original_value := (value_data).original_value;
    operation_count := COALESCE((value_data).operation_count, 0) + 1;
    
    -- Perform operation based on operation parameter
    CASE operation
        WHEN 'M' THEN
            new_value := original_value * multiplier;
        WHEN 'A' THEN
            new_value := original_value + multiplier;
        WHEN 'S' THEN
            new_value := original_value - multiplier;
        ELSE
            RAISE EXCEPTION 'Invalid operation. Use M, A, or S.';
    END CASE;
    
    -- Build the return record
    value_data := ROW(
        original_value,
        new_value,
        operation,
        multiplier,
        operation_count
    );
END;
$$;

-- Function with table-valued parameters (using arrays and custom types)
CREATE OR REPLACE FUNCTION bulk_insert_with_arrays(
    names TEXT[],
    emails TEXT[],
    active_flags BOOLEAN[],
    OUT inserted_count INTEGER,
    OUT failed_count INTEGER,
    OUT error_details TEXT[]
)
RETURNS RECORD
LANGUAGE plpgsql
AS $$
DECLARE
    i INTEGER;
    temp_errors TEXT[] := ARRAY[]::TEXT[];
BEGIN
    inserted_count := 0;
    failed_count := 0;
    
    -- Validate array lengths match
    IF array_length(names, 1) != array_length(emails, 1) OR 
       array_length(names, 1) != array_length(active_flags, 1) THEN
        RAISE EXCEPTION 'Array lengths must match';
    END IF;
    
    -- Process each element
    FOR i IN 1..array_length(names, 1) LOOP
        BEGIN
            INSERT INTO data_types_test_table (
                varchar_column,
                email_column,
                is_active,
                text_column
            ) VALUES (
                names[i],
                emails[i],
                active_flags[i],
                'Bulk inserted record'
            );
            inserted_count := inserted_count + 1;
            
        EXCEPTION
            WHEN OTHERS THEN
                failed_count := failed_count + 1;
                temp_errors := array_append(temp_errors, 
                    'Row ' || i || ': ' || SQLERRM);
        END;
    END LOOP;
    
    error_details := temp_errors;
END;
$$;

-- Function returning a table with various column types
CREATE OR REPLACE FUNCTION get_comprehensive_report(
    start_date DATE DEFAULT NULL,
    end_date DATE DEFAULT NULL,
    include_arrays BOOLEAN DEFAULT FALSE
)
RETURNS TABLE (
    record_id INTEGER,
    summary_text TEXT,
    calculated_decimal DECIMAL(12,2),
    timestamp_value TIMESTAMP,
    json_data JSONB,
    array_data INTEGER[],
    geometric_data POINT,
    network_info INET,
    uuid_value UUID,
    range_data DATERANGE
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        dt.id,
        dt.varchar_column || ' - Report Generated',
        dt.decimal_column * 1.15,
        dt.created_date,
        jsonb_build_object(
            'id', dt.id,
            'status', dt.status_column,
            'active', dt.is_active,
            'generated_at', CURRENT_TIMESTAMP
        ),
        CASE WHEN include_arrays THEN 
            ARRAY[dt.id, dt.integer_column, dt.age_column]
        ELSE NULL END,
        dt.point_column,
        dt.inet_column,
        dt.uuid_column,
        CASE WHEN start_date IS NOT NULL AND end_date IS NOT NULL THEN
            daterange(start_date, end_date)
        ELSE NULL END
    FROM data_types_test_table dt
    WHERE (start_date IS NULL OR dt.created_date::DATE >= start_date)
      AND (end_date IS NULL OR dt.created_date::DATE <= end_date)
      AND dt.is_active = TRUE
    ORDER BY dt.created_date DESC;
END;
$$;
