FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
RUN apt update -qq \    
    && sed -i '/^mozilla\/DST_Root_CA_X3.crt$/ s/^/!/' /etc/ca-certificates.conf \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*
# Required for Time Zone database lookups
ENV TZ="Europe/Madrid"
ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y tzdata
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && dpkg-reconfigure -f noninteractive tzdata
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["customerportalapi/customerportalapi.csproj", "customerportalapi/"]
RUN dotnet restore "customerportalapi/customerportalapi.csproj"
COPY . .
WORKDIR /src/customerportalapi
RUN dotnet build "customerportalapi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "customerportalapi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "customerportalapi.dll"]