FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["customerportalapi/customerportalapi.csproj", "customerportalapi/"]
RUN dotnet restore "customerportalapi/customerportalapi.csproj"
COPY . .
WORKDIR "/src/customerportalapi"
RUN dotnet build "customerportalapi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "customerportalapi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "customerportalapi.dll"]