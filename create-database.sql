-- Create database (run as postgres superuser)
CREATE DATABASE secure_code
    WITH
    OWNER = postgres
    ENCODING = 'UTF8';

-- Connect to the new database
\c secure_code

-- Create user/role
CREATE USER secure_code WITH PASSWORD '12345678';

-- Grant privileges
GRANT CONNECT ON DATABASE secure_code TO secure_code;
GRANT CREATE ON SCHEMA public TO secure_code;
GRANT ALL PRIVILEGES ON DATABASE secure_code TO secure_code;

-- Grant default privileges for future objects
ALTER DEFAULT PRIVILEGES IN SCHEMA public
    GRANT ALL PRIVILEGES ON TABLES TO secure_code;

ALTER DEFAULT PRIVILEGES IN SCHEMA public
    GRANT ALL PRIVILEGES ON SEQUENCES TO secure_code;
