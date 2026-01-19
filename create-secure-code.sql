CREATE SEQUENCE users_id_seq
  START WITH 1
  INCREMENT BY 1
  NO MAXVALUE
  CACHE 1;

CREATE TABLE users (
  id          INTEGER        PRIMARY KEY,
  firstname   VARCHAR(100)   NOT NULL,
  lastname    VARCHAR(100)   NOT NULL,
  password    VARCHAR(100)   NOT NULL,
  salary      INTEGER        NOT NULL,
  money       INTEGER        NOT NULL DEFAULT 1000,
  json_data   TEXT           NOT NULL
);

INSERT INTO users (id, firstname, lastname, password, salary, money, json_data)
VALUES (1, 'Anakin', 'Skywalker', 'pass111', 100000, 406500, '{"certification":"..."}');

INSERT INTO users (id, firstname, lastname, password, salary, money, json_data)
VALUES (2, 'Luke', 'Skywalker', 'pass112', 400000, 198900, '{"certification":"..."}');

INSERT INTO users (id, firstname, lastname, password, salary, money, json_data)
VALUES (3, 'Storm1', 'Trooper', 'pass113', 90000, 19900, '{"certification":"..."}');

INSERT INTO users (id, firstname, lastname, password, salary, money, json_data)
VALUES (4, 'Storm2', 'Trooper', 'pass114', 90000, 29900, '{"certification":"..."}');

INSERT INTO users (id, firstname, lastname, password, salary, money, json_data)
VALUES (5, 'Storm3', 'Trooper', 'pass115', 50000, 39900, '{"certification":"..."}');

INSERT INTO users (id, firstname, lastname, password, salary, money, json_data)
VALUES (6, 'Storm4', 'Trooper', 'pass116', 30000, 49900, '{"certification":"..."}');

COMMIT;
