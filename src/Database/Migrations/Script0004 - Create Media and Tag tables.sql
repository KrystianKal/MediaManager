
-- Create custom types       
DO $$       
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'media_type') THEN
        CREATE TYPE media_type AS ENUM ('image', 'video');
    END IF;
END $$;

CREATE TABLE IF NOT EXISTS media (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    directory_id UUID REFERENCES directories(id) ON DELETE CASCADE NOT NULL,
    media_type media_type NOT NULL,
    name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    view_count INTEGER NOT NULL DEFAULT 0,
    is_favorite BOOLEAN NOT NULL DEFAULT FALSE,
    width INTEGER NOT NULL,
    height INTEGER NOT NULL,
    file_size BIGINT NOT NULL,
    format VARCHAR(10) NULL,
    -- Video specific fields
    duration_seconds INTEGER NULL,
    bit_rate INTEGER NULL,
    
    CONSTRAINT positive_dimensions CHECK (width > 0 AND height > 0),
    CONSTRAINT positive_file_size CHECK (file_size > 0),
    CONSTRAINT positive_duration CHECK (duration_seconds IS NULL OR duration_seconds > 0),
    CONSTRAINT not_negative_bit_rate CHECK (bit_rate IS NULL OR bit_rate >= 0)
);

CREATE TABLE IF NOT EXISTS tags (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL UNIQUE,
    usage_count INT NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Junction table
CREATE TABLE IF NOT EXISTS media_tags (
    media_id UUID REFERENCES media(id) ON DELETE CASCADE,
    tag_id UUID REFERENCES tags(id) ON DELETE CASCADE,
    PRIMARY KEY (media_id, tag_id)
);
    
