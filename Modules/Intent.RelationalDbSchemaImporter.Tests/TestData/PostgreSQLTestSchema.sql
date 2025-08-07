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
CREATE INDEX idx_products_sku ON products(sku);
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
