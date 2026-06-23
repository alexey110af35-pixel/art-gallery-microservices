# TODO: Art Gallery Microservices

## Этап 0: Подготовка
- [x] Создать репозиторий на GitHub
- [x] Создать структуру папок
- [x] Добавить .gitignore
- [x] Добавить README.md
- [x] Добавить папку /docs со структурой

## Этап 1: Docker Compose (инфраструктура)
- [x] Написать docker-compose.yml (PostgreSQL, Kafka, Zookeeper, Kafka UI, Redis, MinIO)
- [x] Написать .env для переменных
- [x] Проверить работу всех контейнеров

## Этап 2: API Gateway + Gallery Service
- [x] Создать Gallery.API (Web API)
- [x] Подключить EF Core + Npgsql
- [x] Создать модель Painting и миграции
- [x] Реализовать CRUD-контроллеры
- [x] Создать ApiGateway (YARP) и настроить маршруты
- [x] Проверить работу через Swagger/Postman

## Этап 3: Identity Service (JWT)
- [x] Создать Identity.API
- [x] Реализовать регистрацию и вход
- [x] Настроить JWT-валидацию в Gateway
- [x] Защитить Gallery API

## Этап 4: Kafka + Upload Service
- [ ] Создать Upload.API
- [ ] Настроить Producer в Gallery (публикация ImageUploadRequested)
- [ ] Настроить Consumer в Upload (обработка и сохранение в MinIO)
- [ ] Настроить Producer в Upload (публикация ImageUploaded)
- [ ] Настроить Consumer в Gallery (обновление статуса)

## Этап 5: Frontend (React)
- [ ] Создать React-приложение
- [ ] Сделать страницу входа/регистрации
- [ ] Сделать список картин
- [ ] Сделать форму добавления картин
- [ ] Настроить CORS

## Этап 6: CI/CD (GitHub Actions)
- [ ] Написать workflow для CI (сборка, тесты)
- [ ] Написать workflow для CD (деплой на VPS)
- [ ] Настроить секреты в GitHub

## Бэклог (позже)
- [ ] Добавить Review Service (отзывы)
- [ ] Добавить Notification Service (уведомления)
- [ ] Добавить Payment Service (продажа)
- [ ] Перейти на Kubernetes
- [ ] Добавить мониторинг (Prometheus + Grafana)