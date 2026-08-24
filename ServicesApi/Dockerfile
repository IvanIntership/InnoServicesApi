FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["ServicesApi.API/ServicesApi.API.csproj", "ServicesApi.API/"]
COPY ["ServicesApi.Application/ServicesApi.Application.csproj", "ServicesApi.Application/"]
COPY ["ServicesApi.Domain/ServicesApi.Domain.csproj", "ServicesApi.Domain/"]
COPY ["ServicesApi.Infrastructure/ServicesApi.Infrastructure.csproj", "ServicesApi.Infrastructure/"]

RUN dotnet restore "ServicesApi.API/ServicesApi.API.csproj"

COPY . .

WORKDIR "/src/ServicesApi.API"
RUN dotnet publish "ServicesApi.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServicesApi.API.dll"]