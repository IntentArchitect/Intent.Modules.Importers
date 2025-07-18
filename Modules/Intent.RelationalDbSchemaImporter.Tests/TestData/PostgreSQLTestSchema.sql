-- =============================================
-- LARGE E-COMMERCE DATABASE SCHEMA - PostgreSQL
-- =============================================

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
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    last_login_date TIMESTAMPTZ
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
    is_default BOOLEAN DEFAULT false,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Categories
CREATE TABLE categories (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(500),
    parent_category_id INTEGER,
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
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
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Products
CREATE TABLE products (
    product_id SERIAL PRIMARY KEY,
    product_name VARCHAR(255) NOT NULL,
    description VARCHAR(1000),
    sku VARCHAR(50) NOT NULL UNIQUE,
    category_id INTEGER NOT NULL,
    supplier_id INTEGER NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    units_in_stock INTEGER NOT NULL DEFAULT 0,
    units_on_order INTEGER NOT NULL DEFAULT 0,
    reorder_level INTEGER NOT NULL DEFAULT 0,
    discontinued BOOLEAN DEFAULT false,
    weight DECIMAL(8,2),
    dimensions VARCHAR(50),
    image_url VARCHAR(500),
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Product Reviews
CREATE TABLE product_reviews (
    review_id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    rating INTEGER NOT NULL CHECK (rating >= 1 AND rating <= 5),
    title VARCHAR(200),
    review_text VARCHAR(2000),
    is_approved BOOLEAN DEFAULT false,
    helpful_count INTEGER DEFAULT 0,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Shopping Cart
CREATE TABLE shopping_cart (
    cart_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    unit_price DECIMAL(10,2) NOT NULL,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Orders
CREATE TABLE orders (
    order_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    order_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    required_date TIMESTAMPTZ,
    shipped_date TIMESTAMPTZ,
    shipping_address_id INTEGER NOT NULL,
    billing_address_id INTEGER NOT NULL,
    sub_total DECIMAL(12,2) NOT NULL,
    tax_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
    shipping_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
    total_amount DECIMAL(12,2) NOT NULL,
    order_status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    payment_status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    tracking_number VARCHAR(50),
    notes VARCHAR(1000)
);

-- Order Details
CREATE TABLE order_details (
    order_detail_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    discount DECIMAL(5,2) DEFAULT 0,
    line_total DECIMAL(12,2) GENERATED ALWAYS AS (quantity * unit_price * (1 - discount)) STORED
);

-- Payment Methods
CREATE TABLE payment_methods (
    payment_method_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    payment_type VARCHAR(20) NOT NULL CHECK (payment_type IN ('Credit Card', 'PayPal', 'Bank Transfer')),
    card_number VARCHAR(20), -- Encrypted
    card_holder_name VARCHAR(100),
    expiry_month INTEGER,
    expiry_year INTEGER,
    is_default BOOLEAN DEFAULT false,
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Payments
CREATE TABLE payments (
    payment_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL,
    payment_method_id INTEGER NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    payment_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    transaction_id VARCHAR(100),
    processor_response VARCHAR(500),
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Inventory Transactions
CREATE TABLE inventory_transactions (
    transaction_id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL,
    transaction_type VARCHAR(20) NOT NULL CHECK (transaction_type IN ('Purchase', 'Sale', 'Return', 'Adjustment')),
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(10,2),
    reference_id INTEGER, -- Could be order_id, purchase_order_id, etc.
    notes VARCHAR(500),
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
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
    change_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
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
    ADD CONSTRAINT fk_products_categories FOREIGN KEY (category_id) REFERENCES categories(category_id),
    ADD CONSTRAINT fk_products_suppliers FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id);

-- Product Reviews
ALTER TABLE product_reviews
    ADD CONSTRAINT fk_product_reviews_products FOREIGN KEY (product_id) REFERENCES products(product_id),
    ADD CONSTRAINT fk_product_reviews_users FOREIGN KEY (user_id) REFERENCES users(user_id);

-- Shopping Cart
ALTER TABLE shopping_cart
    ADD CONSTRAINT fk_shopping_cart_users FOREIGN KEY (user_id) REFERENCES users(user_id),
    ADD CONSTRAINT fk_shopping_cart_products FOREIGN KEY (product_id) REFERENCES products(product_id);

-- Orders
ALTER TABLE orders
    ADD CONSTRAINT fk_orders_users FOREIGN KEY (user_id) REFERENCES users(user_id),
    ADD CONSTRAINT fk_orders_shipping_address FOREIGN KEY (shipping_address_id) REFERENCES user_addresses(address_id),
    ADD CONSTRAINT fk_orders_billing_address FOREIGN KEY (billing_address_id) REFERENCES user_addresses(address_id);

-- Order Details
ALTER TABLE order_details
    ADD CONSTRAINT fk_order_details_orders FOREIGN KEY (order_id) REFERENCES orders(order_id),
    ADD CONSTRAINT fk_order_details_products FOREIGN KEY (product_id) REFERENCES products(product_id);

-- Payment Methods
ALTER TABLE payment_methods
    ADD CONSTRAINT fk_payment_methods_users FOREIGN KEY (user_id) REFERENCES users(user_id);

-- Payments
ALTER TABLE payments
    ADD CONSTRAINT fk_payments_orders FOREIGN KEY (order_id) REFERENCES orders(order_id),
    ADD CONSTRAINT fk_payments_payment_methods FOREIGN KEY (payment_method_id) REFERENCES payment_methods(payment_method_id);

-- Inventory Transactions
ALTER TABLE inventory_transactions
    ADD CONSTRAINT fk_inventory_transactions_products FOREIGN KEY (product_id) REFERENCES products(product_id),
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
-- FUNCTIONS (PostgreSQL equivalent of triggers/procedures)
-- =============================================

-- Function to update modified_date
CREATE OR REPLACE FUNCTION update_modified_date()
RETURNS TRIGGER AS $$
BEGIN
    NEW.modified_date = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger to update modified_date on users table
CREATE TRIGGER tr_users_update_modified_date
    BEFORE UPDATE ON users
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_date();

-- Trigger to update modified_date on products table
CREATE TRIGGER tr_products_update_modified_date
    BEFORE UPDATE ON products
    FOR EACH ROW
    EXECUTE FUNCTION update_modified_date();

-- Function to update inventory when order is placed
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

-- Trigger to update inventory when order is placed
CREATE TRIGGER tr_order_details_update_inventory
    AFTER INSERT ON order_details
    FOR EACH ROW
    EXECUTE FUNCTION update_inventory_on_order();

-- Function to calculate order totals
CREATE OR REPLACE FUNCTION calculate_order_totals()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE orders
    SET sub_total = COALESCE((
        SELECT SUM(line_total)
        FROM order_details
        WHERE order_id = orders.order_id
    ), 0),
    total_amount = sub_total + tax_amount + shipping_amount
    WHERE order_id IN (
        CASE WHEN TG_OP = 'DELETE' THEN OLD.order_id ELSE NEW.order_id END
    );
    
    RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;

-- Trigger to automatically calculate order totals
CREATE TRIGGER tr_orders_calculate_totals
    AFTER INSERT OR UPDATE OR DELETE ON order_details
    FOR EACH ROW
    EXECUTE FUNCTION calculate_order_totals();

-- =============================================
-- STORED PROCEDURES/FUNCTIONS
-- =============================================

-- Get User Profile
CREATE OR REPLACE FUNCTION sp_get_user_profile(p_user_id INTEGER)
RETURNS TABLE (
    user_id INTEGER,
    username VARCHAR(50),
    email VARCHAR(100),
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    date_of_birth DATE,
    phone_number VARCHAR(15),
    is_active BOOLEAN,
    created_date TIMESTAMPTZ,
    last_login_date TIMESTAMPTZ,
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
        u.user_id,
        u.username,
        u.email,
        u.first_name,
        u.last_name,
        u.date_of_birth,
        u.phone_number,
        u.is_active,
        u.created_date,
        u.last_login_date,
        ua.address_id,
        ua.address_type,
        ua.address_line1,
        ua.address_line2,
        ua.city,
        ua.state,
        ua.zip_code,
        ua.country,
        ua.is_default
    FROM users u
    LEFT JOIN user_addresses ua ON u.user_id = ua.user_id
    WHERE u.user_id = p_user_id AND u.is_active = true;
END;
$$ LANGUAGE plpgsql;

-- Create New Order
CREATE OR REPLACE FUNCTION sp_create_order(
    p_user_id INTEGER,
    p_shipping_address_id INTEGER,
    p_billing_address_id INTEGER,
    p_tax_amount DECIMAL(12,2) DEFAULT 0,
    p_shipping_amount DECIMAL(12,2) DEFAULT 0
)
RETURNS INTEGER AS $$
DECLARE
    new_order_id INTEGER;
BEGIN
    -- Create the order
    INSERT INTO orders (user_id, shipping_address_id, billing_address_id, sub_total, tax_amount, shipping_amount, total_amount)
    VALUES (p_user_id, p_shipping_address_id, p_billing_address_id, 0, p_tax_amount, p_shipping_amount, p_tax_amount + p_shipping_amount)
    RETURNING order_id INTO new_order_id;
    
    -- Move items from shopping cart to order details
    INSERT INTO order_details (order_id, product_id, quantity, unit_price)
    SELECT
        new_order_id,
        sc.product_id,
        sc.quantity,
        sc.unit_price
    FROM shopping_cart sc
    WHERE sc.user_id = p_user_id;

    -- Clear shopping cart
    DELETE FROM shopping_cart WHERE user_id = p_user_id;

    RETURN new_order_id;
END;
$$ LANGUAGE plpgsql;

-- Add Product to Cart
CREATE OR REPLACE FUNCTION sp_add_to_cart(
    p_user_id INTEGER,
    p_product_id INTEGER,
    p_quantity INTEGER
)
RETURNS VOID AS $$
DECLARE
    v_unit_price DECIMAL(10,2);
BEGIN
    -- Get product price
    SELECT unit_price INTO v_unit_price
    FROM products
    WHERE product_id = p_product_id AND discontinued = false;

    IF v_unit_price IS NULL THEN
        RAISE EXCEPTION 'Product not found or discontinued';
    END IF;
    
    -- Check if item already in cart
    IF EXISTS (SELECT 1 FROM shopping_cart WHERE user_id = p_user_id AND product_id = p_product_id) THEN
        UPDATE shopping_cart
        SET quantity = quantity + p_quantity,
            modified_date = CURRENT_TIMESTAMP
        WHERE user_id = p_user_id AND product_id = p_product_id;
    ELSE
        INSERT INTO shopping_cart (user_id, product_id, quantity, unit_price)
        VALUES (p_user_id, p_product_id, p_quantity, v_unit_price);
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Get Product Search Results
CREATE OR REPLACE FUNCTION sp_search_products(
    p_search_term VARCHAR(255) DEFAULT NULL,
    p_category_id INTEGER DEFAULT NULL,
    p_min_price DECIMAL(10,2) DEFAULT NULL,
    p_max_price DECIMAL(10,2) DEFAULT NULL,
    p_sort_by VARCHAR(20) DEFAULT 'Name',
    p_sort_order VARCHAR(4) DEFAULT 'ASC',
    p_page_number INTEGER DEFAULT 1,
    p_page_size INTEGER DEFAULT 20
)
RETURNS TABLE (
    product_id INTEGER,
    product_name VARCHAR(255),
    description VARCHAR(1000),
    sku VARCHAR(50),
    unit_price DECIMAL(10,2),
    units_in_stock INTEGER,
    image_url VARCHAR(500),
    category_name VARCHAR(100),
    supplier_name VARCHAR(100),
    average_rating DECIMAL(3,2),
    review_count BIGINT
) AS $$
DECLARE
    v_offset INTEGER := (p_page_number - 1) * p_page_size;
BEGIN
    RETURN QUERY
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
        COALESCE(AVG(pr.rating::DECIMAL), 0)::DECIMAL(3,2) AS average_rating,
        COUNT(pr.review_id) AS review_count
    FROM products p
    INNER JOIN categories c ON p.category_id = c.category_id
    INNER JOIN suppliers s ON p.supplier_id = s.supplier_id
    LEFT JOIN product_reviews pr ON p.product_id = pr.product_id AND pr.is_approved = true
    WHERE p.discontinued = false
      AND (p_search_term IS NULL OR p.product_name ILIKE '%' || p_search_term || '%' OR p.description ILIKE '%' || p_search_term || '%')
      AND (p_category_id IS NULL OR p.category_id = p_category_id)
      AND (p_min_price IS NULL OR p.unit_price >= p_min_price)
      AND (p_max_price IS NULL OR p.unit_price <= p_max_price)
    GROUP BY p.product_id, p.product_name, p.description, p.sku, p.unit_price, p.units_in_stock, p.image_url, c.category_name, s.supplier_name
    ORDER BY
        CASE WHEN p_sort_by = 'Name' AND p_sort_order = 'ASC' THEN p.product_name END ASC,
        CASE WHEN p_sort_by = 'Name' AND p_sort_order = 'DESC' THEN p.product_name END DESC,
        CASE WHEN p_sort_by = 'Price' AND p_sort_order = 'ASC' THEN p.unit_price END ASC,
        CASE WHEN p_sort_by = 'Price' AND p_sort_order = 'DESC' THEN p.unit_price END DESC,
        CASE WHEN p_sort_by = 'Rating' AND p_sort_order = 'ASC' THEN AVG(pr.rating::DECIMAL) END ASC,
        CASE WHEN p_sort_by = 'Rating' AND p_sort_order = 'DESC' THEN AVG(pr.rating::DECIMAL) END DESC
    OFFSET v_offset
    LIMIT p_page_size;
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
    COALESCE(AVG(pr.rating::DECIMAL), 0)::DECIMAL(3,2) AS average_rating,
    COUNT(pr.review_id) AS review_count
FROM products p
INNER JOIN categories c ON p.category_id = c.category_id
INNER JOIN suppliers s ON p.supplier_id = s.supplier_id
LEFT JOIN product_reviews pr ON p.product_id = pr.product_id AND pr.is_approved = true
WHERE p.discontinued = false
GROUP BY p.product_id, p.product_name, p.description, p.sku, p.unit_price, p.units_in_stock, p.image_url, c.category_name, s.supplier_name;

-- Order Summary View
CREATE VIEW vw_order_summary AS
SELECT
    o.order_id,
    o.order_date,
    o.order_status,
    o.payment_status,
    u.first_name || ' ' || u.last_name AS customer_name,
    u.email AS customer_email,
    COUNT(od.order_detail_id) AS item_count,
    o.total_amount,
    o.tracking_number
FROM orders o
INNER JOIN users u ON o.user_id = u.user_id
INNER JOIN order_details od ON o.order_id = od.order_id
GROUP BY o.order_id, o.order_date, o.order_status, o.payment_status, u.first_name, u.last_name, u.email, o.total_amount, o.tracking_number; 