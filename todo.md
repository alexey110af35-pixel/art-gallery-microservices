# TODO: Art Gallery Microservices

## Этап 0: Подготовка
- [x] Создать репозиторий на GitHub
- [ ] Создать структуру папок
- [ ] Добавить .gitignore
- [ ] Добавить README.md
- [ ] Добавить папку /docs со структурой

## Этап 1: Docker Compose (инфраструктура)
- [ ] Написать docker-compose.yml (PostgreSQL, Kafka, Zookeeper, Kafka UI, Redis, MinIO)
- [ ] Написать .env для переменных
- [ ] Проверить работу всех контейнеров

## Этап 2: API Gateway + Gallery Service
- [ ] Создать Gallery.API (Web API)
- [ ] Подключить EF Core + Npgsql
- [ ] Создать модель Painting и миграции
- [ ] Реализовать CRUD-контроллеры
- [ ] Создать ApiGateway (YARP) и настроить маршруты
- [ ] Проверить работу через Swagger/Postman

## Этап 3: Identity Service (JWT)
- [ ] Создать Identity.API
- [ ] Реализовать регистрацию и вход
- [ ] Настроить JWT-валидацию в Gateway
- [ ] Защитить Gallery API

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