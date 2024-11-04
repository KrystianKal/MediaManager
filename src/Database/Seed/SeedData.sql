-- Create tags
INSERT INTO tags (id, name, usage_count)
SELECT
    'c3b7e64f-3c3b-4c0d-af63-d9a9c5875d03'::uuid,
    'nature',
    0
WHERE NOT EXISTS (
    SELECT 1 FROM tags WHERE name = 'nature'
);

INSERT INTO tags (id, name, usage_count)
SELECT
    'f143c6a9-1a21-4c56-8f36-4c2cbdedb89c'::uuid,
    'landscape',
    0
WHERE NOT EXISTS (
    SELECT 1 FROM tags WHERE name = 'landscape'
);

-- Create Directories
INSERT INTO directories (
    id,
    path,
    file_count,
    size
)
SELECT
    'e5f6a7b8-c9d0-4e1f-af63-d9a9c5875d03'::uuid,
    '/videos',
    1,
    15728640  -- Sum of video files (15MB)
WHERE NOT EXISTS (
    SELECT 1 FROM directories WHERE path = '/videos'
);

INSERT INTO directories (
    id,
    path,
    file_count,
    size
)
SELECT
    'f6a7b8c9-d0e1-4f2c-8f36-4c2cbdedb89c'::uuid,
    '/images',
    2,
    15728640  -- Sum of image files (5MB + 10MB)
WHERE NOT EXISTS (
    SELECT 1 FROM directories WHERE path = '/images'
);

-- Create Media
INSERT INTO media (
    id,
    media_type,
    directory_id,
    name,
    width,
    height,
    file_size,
    format,
    duration_seconds,
    bit_rate
)
SELECT
    'a1b2c3d4-e5f6-4a5b-8c9d-1a2b3c4d5e6f'::uuid,
    'video',
    'e5f6a7b8-c9d0-4e1f-af63-d9a9c5875d03'::uuid,  -- Fixed directory UUID reference
    'nature_timelapse',
    1920,
    1080,
    15728640, -- 15MB in bytes
    'mp4',
    30,
    4000000  -- 4Mbps
WHERE NOT EXISTS (
    SELECT 1 FROM media WHERE name = 'nature_timelapse'
);

INSERT INTO media (
    id,
    media_type,
    directory_id,
    name,
    width,
    height,
    file_size,
    format
)
SELECT
    'b2c3d4e5-f6a7-5b6c-9d0e-2b3c4d5e6f7a'::uuid,
    'image',
    'f6a7b8c9-d0e1-4f2c-8f36-4c2cbdedb89c'::uuid,  -- Fixed directory UUID reference
    'mountain_landscape',
    1920,
    1080,
    5242880, -- 5MB in bytes
    'jpg'
WHERE NOT EXISTS (
    SELECT 1 FROM media WHERE name = 'mountain_landscape'
);

INSERT INTO media (
    id,
    media_type,
    directory_id,
    name,
    width,
    height,
    file_size,
    format
)
SELECT
    'd4e5f6a7-b8c9-4d0e-9f1a-3c4d5e6f7a8b'::uuid,
    'image',
    'f6a7b8c9-d0e1-4f2c-8f36-4c2cbdedb89c'::uuid,  -- Fixed directory UUID reference
    'forest_aerial',
    3840,
    2160,
    10485760, -- 10MB in bytes
    'png'
WHERE NOT EXISTS (
    SELECT 1 FROM media WHERE name = 'forest_aerial'
);
-- Create Tag Media relation

INSERT INTO media_tags (media_id, tag_id)
SELECT
    'a1b2c3d4-e5f6-4a5b-8c9d-1a2b3c4d5e6f'::uuid,
    'c3b7e64f-3c3b-4c0d-af63-d9a9c5875d03'::uuid
WHERE NOT EXISTS (
    SELECT 1 FROM media_tags
    WHERE media_id = 'a1b2c3d4-e5f6-4a5b-8c9d-1a2b3c4d5e6f'::uuid
      AND tag_id = 'c3b7e64f-3c3b-4c0d-af63-d9a9c5875d03'::uuid
);

INSERT INTO media_tags (media_id, tag_id)
SELECT
    'b2c3d4e5-f6a7-5b6c-9d0e-2b3c4d5e6f7a'::uuid,
    'c3b7e64f-3c3b-4c0d-af63-d9a9c5875d03'::uuid
WHERE NOT EXISTS (
    SELECT 1 FROM media_tags
    WHERE media_id = 'b2c3d4e5-f6a7-5b6c-9d0e-2b3c4d5e6f7a'::uuid
      AND tag_id = 'c3b7e64f-3c3b-4c0d-af63-d9a9c5875d03'::uuid
);

INSERT INTO media_tags (media_id, tag_id)
SELECT
    'b2c3d4e5-f6a7-5b6c-9d0e-2b3c4d5e6f7a'::uuid,
    'f143c6a9-1a21-4c56-8f36-4c2cbdedb89c'::uuid
WHERE NOT EXISTS (
    SELECT 1 FROM media_tags
    WHERE media_id = 'b2c3d4e5-f6a7-5b6c-9d0e-2b3c4d5e6f7a'::uuid
      AND tag_id = 'f143c6a9-1a21-4c56-8f36-4c2cbdedb89c'::uuid
);

INSERT INTO media_tags (media_id, tag_id)
SELECT
    'd4e5f6a7-b8c9-4d0e-9f1a-3c4d5e6f7a8b'::uuid,
    'f143c6a9-1a21-4c56-8f36-4c2cbdedb89c'::uuid
WHERE NOT EXISTS (
    SELECT 1 FROM media_tags
    WHERE media_id = 'd4e5f6a7-b8c9-4d0e-9f1a-3c4d5e6f7a8b'::uuid
      AND tag_id = 'f143c6a9-1a21-4c56-8f36-4c2cbdedb89c'::uuid
);
