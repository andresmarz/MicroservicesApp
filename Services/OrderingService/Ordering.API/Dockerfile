FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Ordering.API/Ordering.API.csproj", "Ordering.API/"]
COPY ["../Ordering.Application/Ordering.Application.csproj", "Ordering.Application/"]
COPY ["../Ordering.Domain/Ordering.Domain.csproj", "Ordering.Domain/"]
COPY ["../Ordering.Infrastructure/Ordering.Infrastructure.csproj", "Ordering.Infrastructure/"]

RUN dotnet restore "Ordering.API/Ordering.API.csproj"

COPY . .

RUN dotnet publish "Ordering.API/Ordering.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Ordering.API.dll"]
