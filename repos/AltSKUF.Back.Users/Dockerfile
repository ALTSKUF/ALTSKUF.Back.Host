# Базовый образ для .NET SDK 9.0 (нужен для сборки)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Копируем и восстанавливаем зависимости
COPY . ./
RUN dotnet restore "src/AltSKUF.Server.Users/AltSKUF.Back.Users.csproj"

# Сборка
RUN dotnet publish "src/AltSKUF.Server.Users/AltSKUF.Back.Users.csproj" -c Release -o /app/out

# Образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "AltSKUF.Back.Users.dll"]
