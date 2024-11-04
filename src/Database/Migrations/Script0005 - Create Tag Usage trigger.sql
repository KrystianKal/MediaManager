CREATE OR REPLACE FUNCTION update_tag_usage_count()
    RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        UPDATE tags SET usage_count = usage_count + 1 WHERE id = NEW.tag_id;
    ELSIF TG_OP = 'DELETE' THEN
        UPDATE tags SET usage_count = usage_count - 1 WHERE id = OLD.tag_id;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DO $$
BEGIN 
    CREATE OR REPLACE TRIGGER trigger_update_tag_usage_count
        AFTER INSERT OR DELETE ON media_tags
        FOR EACH ROW
        EXECUTE FUNCTION update_tag_usage_count();
END $$;