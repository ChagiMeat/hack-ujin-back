#!/bin/bash
set -e

DB_HOST=${DB_HOST:-xakujin_mysql}
DB_PORT=${DB_PORT:-3306}

echo "Ожидание запуска MySQL на $DB_HOST:$DB_PORT..."

# Ждем, пока MySQL не откроет порт
until nc -z "$DB_HOST" "$DB_PORT"; do
  echo "🔄 MySQL ещё не доступен..."
  sleep 2
done

echo "MySQL доступен. Применяем миграции..."

dotnet restore /app/XakUjin2025.csproj
dotnet build /app/XakUjin2025.csproj -c Debug

dotnet ef database update --no-build


echo "Запускаем приложение..."
exec dotnet XakUjin2025.dll
