CREATE TABLE IF NOT EXISTS service_categories (
    id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    duration INTERVAL NOT NULL
);

CREATE TABLE IF NOT EXISTS services (
    id UUID PRIMARY KEY,
    specialization_id UUID NOT NULL,
    service_category_id UUID NOT NULL REFERENCES service_categories(id) ON DELETE RESTRICT,
    name VARCHAR(100) NOT NULL UNIQUE,
    price NUMERIC(18, 2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE INDEX IF NOT EXISTS idx_services_specialization_id
ON services(specialization_id);

CREATE INDEX IF NOT EXISTS idx_services_service_category_id
ON services(service_category_id);