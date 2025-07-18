-- =============================================
-- LARGE E-COMMERCE DATABASE SCHEMA - MySQL
-- =============================================

-- =============================================
-- TABLES
-- =============================================

-- Users Table
CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    date_of_birth DATE,
    phone_number VARCHAR(15),
    is_active BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    last_login_date TIMESTAMP NULL
);

-- User Addresses
CREATE TABLE user_addresses (
    address_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    address_type ENUM('Billing', 'Shipping') NOT NULL,
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
    category_id INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(500),
    parent_category_id INT,
    is_active BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Suppliers
CREATE TABLE suppliers (
    supplier_id INT AUTO_INCREMENT PRIMARY KEY,
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
    product_id INT AUTO_INCREMENT PRIMARY KEY,
    product_name VARCHAR(255) NOT NULL,
    description TEXT,
    sku VARCHAR(50) NOT NULL UNIQUE,
    category_id INT NOT NULL,
    supplier_id INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    units_in_stock INT NOT NULL DEFAULT 0,
    units_on_order INT NOT NULL DEFAULT 0,
    reorder_level INT NOT NULL DEFAULT 0,
    discontinued BOOLEAN DEFAULT FALSE,
    weight DECIMAL(8,2),
    dimensions VARCHAR(50),
    image_url VARCHAR(500),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Product Reviews
CREATE TABLE product_reviews (
    review_id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT NOT NULL,
    user_id INT NOT NULL,
    rating INT NOT NULL CHECK (rating >= 1 AND rating <= 5),
    title VARCHAR(200),
    review_text TEXT,
    is_approved BOOLEAN DEFAULT FALSE,
    helpful_count INT DEFAULT 0,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Shopping Cart
CREATE TABLE shopping_cart (
    cart_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    unit_price DECIMAL(10,2) NOT NULL,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Orders
CREATE TABLE orders (
    order_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    required_date TIMESTAMP NULL,
    shipped_date TIMESTAMP NULL,
    shipping_address_id INT NOT NULL,
    billing_address_id INT NOT NULL,
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
    order_detail_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    discount DECIMAL(5,2) DEFAULT 0,
    line_total DECIMAL(12,2) GENERATED ALWAYS AS (quantity * unit_price * (1 - discount)) STORED
);

-- Payment Methods
CREATE TABLE payment_methods (
    payment_method_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    payment_type ENUM('Credit Card', 'PayPal', 'Bank Transfer') NOT NULL,
    card_number VARCHAR(20), -- Encrypted
    card_holder_name VARCHAR(100),
    expiry_month INT,
    expiry_year INT,
    is_default BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Payments
CREATE TABLE payments (
    payment_id INT AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    payment_method_id INT NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    payment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    transaction_id VARCHAR(100),
    processor_response VARCHAR(500),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Inventory Transactions
CREATE TABLE inventory_transactions (
    transaction_id INT AUTO_INCREMENT PRIMARY KEY,
    product_id INT NOT NULL,
    transaction_type ENUM('Purchase', 'Sale', 'Return', 'Adjustment') NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2),
    reference_id INT, -- Could be order_id, purchase_order_id, etc.
    notes VARCHAR(500),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INT NOT NULL
);

-- Audit Log
CREATE TABLE audit_log (
    audit_id INT AUTO_INCREMENT PRIMARY KEY,
    table_name VARCHAR(100) NOT NULL,
    record_id INT NOT NULL,
    action ENUM('INSERT', 'UPDATE', 'DELETE') NOT NULL,
    old_values JSON,
    new_values JSON,
    changed_by INT,
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
-- TRIGGERS (MySQL style)
-- =============================================

-- Update inventory when order details are inserted
DELIMITER //
CREATE TRIGGER tr_order_details_update_inventory
    AFTER INSERT ON order_details
    FOR EACH ROW
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
END//
DELIMITER ;

-- Calculate order totals when order details change
DELIMITER //
CREATE TRIGGER tr_order_details_calculate_totals_insert
    AFTER INSERT ON order_details
    FOR EACH ROW
BEGIN
    UPDATE orders
    SET sub_total = (
        SELECT COALESCE(SUM(line_total), 0)
        FROM order_details
        WHERE order_id = NEW.order_id
    ),
    total_amount = sub_total + tax_amount + shipping_amount
    WHERE order_id = NEW.order_id;
END//
DELIMITER ;

DELIMITER //
CREATE TRIGGER tr_order_details_calculate_totals_update
    AFTER UPDATE ON order_details
    FOR EACH ROW
BEGIN
    UPDATE orders
    SET sub_total = (
        SELECT COALESCE(SUM(line_total), 0)
        FROM order_details
        WHERE order_id = NEW.order_id
    ),
    total_amount = sub_total + tax_amount + shipping_amount
    WHERE order_id = NEW.order_id;
END//
DELIMITER ;

DELIMITER //
CREATE TRIGGER tr_order_details_calculate_totals_delete
    AFTER DELETE ON order_details
    FOR EACH ROW
BEGIN
    UPDATE orders
    SET sub_total = (
        SELECT COALESCE(SUM(line_total), 0)
        FROM order_details
        WHERE order_id = OLD.order_id
    ),
    total_amount = sub_total + tax_amount + shipping_amount
    WHERE order_id = OLD.order_id;
END//
DELIMITER ;

-- =============================================
-- STORED PROCEDURES
-- =============================================

-- Get User Profile
DELIMITER //
CREATE PROCEDURE sp_get_user_profile(IN p_user_id INT)
BEGIN
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

    -- Get user addresses
    SELECT
        address_id,
        address_type,
        address_line1,
        address_line2,
        city,
        state,
        zip_code,
        country,
        is_default
    FROM user_addresses
    WHERE user_id = p_user_id;
END//
DELIMITER ;

-- Create New Order
DELIMITER //
CREATE PROCEDURE sp_create_order(
    IN p_user_id INT,
    IN p_shipping_address_id INT,
    IN p_billing_address_id INT,
    IN p_tax_amount DECIMAL(12,2),
    IN p_shipping_amount DECIMAL(12,2),
    OUT p_order_id INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Create the order
    INSERT INTO orders (user_id, shipping_address_id, billing_address_id, sub_total, tax_amount, shipping_amount, total_amount)
    VALUES (p_user_id, p_shipping_address_id, p_billing_address_id, 0, p_tax_amount, p_shipping_amount, p_tax_amount + p_shipping_amount);
    
    SET p_order_id = LAST_INSERT_ID();
    
    -- Move items from shopping cart to order details
    INSERT INTO order_details (order_id, product_id, quantity, unit_price)
    SELECT
        p_order_id,
        sc.product_id,
        sc.quantity,
        sc.unit_price
    FROM shopping_cart sc
    WHERE sc.user_id = p_user_id;

    -- Clear shopping cart
    DELETE FROM shopping_cart WHERE user_id = p_user_id;

    COMMIT;
END//
DELIMITER ;

-- Add Product to Cart
DELIMITER //
CREATE PROCEDURE sp_add_to_cart(
    IN p_user_id INT,
    IN p_product_id INT,
    IN p_quantity INT
)
BEGIN
    DECLARE v_unit_price DECIMAL(10,2);
    DECLARE v_cart_exists INT DEFAULT 0;
    
    -- Get product price
    SELECT unit_price INTO v_unit_price
    FROM products
    WHERE product_id = p_product_id AND discontinued = FALSE;

    IF v_unit_price IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Product not found or discontinued';
    END IF;
    
    -- Check if item already in cart
    SELECT COUNT(*) INTO v_cart_exists
    FROM shopping_cart 
    WHERE user_id = p_user_id AND product_id = p_product_id;
    
    IF v_cart_exists > 0 THEN
        UPDATE shopping_cart
        SET quantity = quantity + p_quantity,
            modified_date = CURRENT_TIMESTAMP
        WHERE user_id = p_user_id AND product_id = p_product_id;
    ELSE
        INSERT INTO shopping_cart (user_id, product_id, quantity, unit_price)
        VALUES (p_user_id, p_product_id, p_quantity, v_unit_price);
    END IF;
END//
DELIMITER ;

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
    COALESCE(AVG(pr.rating), 0) AS average_rating,
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
    CONCAT(u.first_name, ' ', u.last_name) AS customer_name,
    u.email AS customer_email,
    COUNT(od.order_detail_id) AS item_count,
    o.total_amount,
    o.tracking_number
FROM orders o
INNER JOIN users u ON o.user_id = u.user_id
INNER JOIN order_details od ON o.order_id = od.order_id
GROUP BY o.order_id, o.order_date, o.order_status, o.payment_status, u.first_name, u.last_name, u.email, o.total_amount, o.tracking_number; 