-- 2. ตั้งค่า Timezone สำหรับ Session ปัจจุบัน
SET timezone = 'Asia/Bangkok';

-- 3. สร้าง Table (id เป็น SERIAL จะรัน 1, 2, 3... ให้เอง)
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    firstname varchar(255) NOT NULL,
    lastname varchar(255) NOT NULL,
    password varchar(255) NOT NULL,
    salary int NOT NULL,
    money int NOT NULL DEFAULT 1000,
    json_data jsonb NOT NULL
);

-- 4. การ Insert ข้อมูล (ไม่ระบุ Column 'id')
INSERT INTO users (firstname, lastname, password, salary, money, json_data) VALUES
('Anakin', 'Skywalker', 'pass111', 100000, 406500, '{"certification":[{"name":"Java EE","year":2009},{"name":"CCNA Security","year":2010},{"name":"CodeCamp","year":2018}]}'),
('Luke', 'Skywalker', 'pass112', 400000, 198900, '{"certification":[{"name":".NET Core","year":2016},{"name":"CCNA Networking","year":2015}]}'),
('Storm1', 'Trooper', 'pass113', 90000, 19900, '{"certification":[{"name":"Node.JS Enterprise","year":2016},{"name":"CCNA Cloud","year":2015}]}'),
('Storm2', 'Trooper', 'pass114', 90000, 29900, '{"certification":[{"name":".NET Core","year":2016},{"name":"CCNA Networking","year":2015}]}'),
('Storm3', 'Trooper', 'pass115', 50000, 39900, '{"certification":[{"name":"Node.JS Enterprise","year":2016},{"name":"CCNA Cloud","year":2015}]}'),
('Storm4', 'Trooper', 'pass116', 30000, 49900, '{"certification":[{"name":"Java EE","year":2009},{"name":"CCNA Security","year":2010}]}');