CREATE SEQUENCE users_id_seq
  START WITH 1
  INCREMENT BY 1
  NOMAXVALUE;

CREATE TABLE users (
  id          NUMBER(10)      PRIMARY KEY,
  firstname   VARCHAR2(100)   NOT NULL,
  lastname    VARCHAR2(100)   NOT NULL,
  password    VARCHAR2(100)   NOT NULL,
  salary      NUMBER(10)      NOT NULL,
  money       NUMBER(10)      DEFAULT 1000 NOT NULL,
  json_data   VARCHAR2(4000)  NOT NULL
);

INSERT ALL
  INTO users (id, firstname, lastname, password, salary, money, json_data) VALUES (1, 'Anakin', 'Skywalker', 'pass111', 100000, 406500, '{"certification":"..."}')
  INTO users (id, firstname, lastname, password, salary, money, json_data) VALUES (2, 'Luke', 'Skywalker', 'pass112', 400000, 198900, '{"certification":"..."}')
  INTO users (id, firstname, lastname, password, salary, money, json_data) VALUES (3, 'Storm1', 'Trooper', 'pass113', 90000, 19900, '{"certification":"..."}')
  INTO users (id, firstname, lastname, password, salary, money, json_data) VALUES (4, 'Storm2', 'Trooper', 'pass114', 90000, 29900, '{"certification":"..."}')
  INTO users (id, firstname, lastname, password, salary, money, json_data) VALUES (5, 'Storm3', 'Trooper', 'pass115', 50000, 39900, '{"certification":"..."}')
  INTO users (id, firstname, lastname, password, salary, money, json_data) VALUES (6, 'Storm4', 'Trooper', 'pass116', 30000, 49900, '{"certification":"..."}')
SELECT 1 FROM DUAL; 

COMMIT;