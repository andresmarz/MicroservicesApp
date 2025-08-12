# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos del proyecto (puede variar si quieres copiar solo los necesarios)
COPY ["Catalog.API/Catalog.API.csproj", "Catalog.API/"]
COPY ["Catalog.Application/Catalog.Application.csproj", "Catalog.Application/"]
COPY ["Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]
COPY ["Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Catalog.Infrastructure/"]

# Restauramos dependencias
RUN dotnet restore "Catalog.API/Catalog.API.csproj"

# Copiamos todo el código
COPY . .

# Publicamos en modo Release
RUN dotnet publish "Catalog.API/Catalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exponemos el puerto donde corre la API
EXPOSE 5003

# Comando para arrancar la app
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
