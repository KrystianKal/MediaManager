CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS directories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    path VARCHAR(256) NOT NULL UNIQUE,
    file_count INT NOT NULL DEFAULT 0,
    size BIGINT NOT NULL DEFAULT 0,
    
    CONSTRAINT non_negative_size CHECK (size >= 0),
    CONSTRAINT not_negative_file_count CHECK (file_count >= 0)
);


