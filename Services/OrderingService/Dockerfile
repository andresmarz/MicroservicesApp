# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY ["Ordering.API/Ordering.API.csproj", "Ordering.API/"]
COPY ["Ordering.Application/Ordering.Application.csproj", "Ordering.Application/"]
COPY ["Ordering.Domain/Ordering.Domain.csproj", "Ordering.Domain/"]
COPY ["Ordering.Infrastructure/Ordering.Infrastructure.csproj", "Ordering.Infrastructure/"]


# Restaurar dependencias
RUN dotnet restore "Ordering.API/Ordering.API.csproj"

# Copiar todo el código
COPY . .

# Publicar aplicación
WORKDIR "/src/Ordering.API"
RUN dotnet publish "Ordering.API.csproj" -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
