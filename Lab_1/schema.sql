DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS tours;
DROP TABLE IF EXISTS clients;

CREATE TABLE clients (
    id           INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    last_name    VARCHAR(50)  NOT NULL,
    first_name   VARCHAR(50)  NOT NULL,
    middle_name  VARCHAR(50),
    phone        VARCHAR(20)  NOT NULL UNIQUE,
    city         VARCHAR(50)  NOT NULL,
    street       VARCHAR(100) NOT NULL,
    building     VARCHAR(10)  NOT NULL,
    apartment    VARCHAR(10)
);

CREATE TABLE tours (
    id             INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name           VARCHAR(100)  NOT NULL UNIQUE,
    description    TEXT          NOT NULL,
    price          NUMERIC(10,2) NOT NULL CHECK (price > 0),
    duration_days  INT           NOT NULL CHECK (duration_days > 0)
);

CREATE TABLE orders (
    id                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    client_id         INT          NOT NULL REFERENCES clients(id),
    tour_id           INT          NOT NULL REFERENCES tours(id),
    order_date        DATE         NOT NULL,
    trip_date         DATE         NOT NULL,
    quantity          INT          NOT NULL CHECK (quantity > 0),
    discount_percent  NUMERIC(5,2) NOT NULL DEFAULT 0
                      CHECK (discount_percent >= 0 AND discount_percent <= 100),
    CHECK (trip_date >= order_date)
);

INSERT INTO clients (last_name, first_name, middle_name, phone, city, street, building, apartment) VALUES
('Шевченко',  'Андрій',    'Іванович',     '+380671234501', 'Київ',          'вул. Хрещатик',          '12',  '45'),
('Коваленко', 'Олена',     'Петрівна',     '+380501234502', 'Львів',         'вул. Городоцька',        '87',  '3'),
('Бондаренко','Максим',    'Сергійович',   '+380631234503', 'Одеса',         'вул. Дерибасівська',     '5',   NULL),
('Мельник',   'Ірина',     'Василівна',    '+380971234504', 'Харків',        'просп. Науки',           '40',  '112'),
('Ткаченко',  'Дмитро',    'Олександрович','+380661234505', 'Дніпро',        'просп. Яворницького',    '21',  '8'),
('Кравченко', 'Наталія',   'Миколаївна',   '+380681234506', 'Вінниця',       'вул. Соборна',           '64',  '17'),
('Олійник',   'Сергій',    'Анатолійович', '+380931234507', 'Полтава',       'вул. Європейська',       '9',   NULL),
('Шевчук',    'Юлія',      'Романівна',    '+380991234508', 'Івано-Франківськ','вул. Незалежності',    '33',  '2'),
('Поліщук',   'Віктор',    'Богданович',   '+380731234509', 'Житомир',       'вул. Київська',          '15',  '51'),
('Лисенко',   'Тетяна',    'Ігорівна',     '+380951234510', 'Черкаси',       'бульв. Шевченка',        '128', '9');

INSERT INTO tours (name, description, price, duration_days) VALUES
('Карпатські вершини', 'Піші маршрути Чорногорою, сходження на Говерлу, проживання в котеджі, сніданки та вечері.', 14500.00, 7),
('Львівські вихідні',  'Екскурсії старим містом, кав''ярні та музеї, нічні прогулянки, готель у центрі.',            6200.00, 3),
('Одеське узбережжя',  'Відпочинок на морі, екскурсія катакомбами, готель біля пляжу, сніданки включено.',           11800.00, 6),
('Єгипет: Хургада',    'Переліт, готель 5* «все включено», снорклінг на коралових рифах, екскурсія до Луксора.',    38900.00, 8),
('Туреччина: Анталія', 'Переліт, готель 4* «все включено», екскурсії до Памуккале та стародавнього Сіде.',          32400.00, 10);

INSERT INTO orders (client_id, tour_id, order_date, trip_date, quantity, discount_percent) VALUES
(1, 1, '2026-01-10', '2026-02-14', 2, 5),
(1, 4, '2026-03-02', '2026-05-01', 2, 10),
(2, 2, '2026-01-15', '2026-01-30', 1, 0),
(2, 3, '2026-04-20', '2026-07-05', 3, 7),
(2, 5, '2026-06-01', '2026-08-15', 2, 10),
(3, 4, '2026-02-11', '2026-03-20', 4, 12),
(3, 1, '2026-08-01', '2026-09-10', 1, 0),
(4, 5, '2026-03-18', '2026-06-10', 2, 5),
(4, 2, '2026-09-01', '2026-10-17', 2, 0),
(5, 3, '2026-05-05', '2026-06-20', 1, 0),
(5, 4, '2026-10-01', '2026-12-28', 2, 15),
(6, 1, '2026-01-20', '2026-02-21', 1, 0),
(7, 3, '2026-06-12', '2026-07-25', 4, 8),
(8, 2, '2026-02-03', '2026-03-07', 2, 0),
(9, 5, '2026-04-09', '2026-06-01', 3, 10),
(10, 4, '2026-07-07', '2026-09-15', 1, 3);
