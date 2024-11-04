CREATE OR REPLACE FUNCTION update_directory_file_count()
    RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE directories SET file_count = file_count + 1 WHERE id = NEW.directory_id;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE directories SET file_count = file_count - 1 WHERE id = OLD.directory_id;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_directory_size()
    RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE directories SET size = size + NEW.file_size WHERE id = NEW.directory_id;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE directories SET size = size - OLD.file_size WHERE id = OLD.directory_id;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DO $$
    BEGIN
        CREATE OR REPLACE TRIGGER trigger_update_directory_file_count
            AFTER INSERT OR DELETE ON media
            FOR EACH ROW
        EXECUTE FUNCTION update_directory_file_count();
        
        CREATE OR REPLACE TRIGGER trigger_update_directory_size
            AFTER INSERT OR DELETE ON media
            FOR EACH ROW
        EXECUTE FUNCTION update_directory_size();
    END $$;
