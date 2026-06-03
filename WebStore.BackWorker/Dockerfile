# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebStore.BackWorker/WebStore.BackWorker.csproj", "WebStore.BackWorker/"]
COPY ["CartInfrastructure/WebStore.CartInfrastructure.csproj", "CartInfrastructure/"]
COPY ["WebStore.CartApp/WebStore.CartApp.csproj", "WebStore.CartApp/"]
COPY ["WebStore.CartDomain/WebStore.CartDomain.csproj", "WebStore.CartDomain/"]
COPY ["WebStore.CatalogApp/WebStore.CatalogApp.csproj", "WebStore.CatalogApp/"]
COPY ["WebStore.CatalogDomain/WebStore.CatalogDomain.csproj", "WebStore.CatalogDomain/"]
COPY ["WebStore.Contracts/WebStore.Contracts.csproj", "WebStore.Contracts/"]
COPY ["WebStore.Shared/WebStore.Shared.csproj", "WebStore.Shared/"]
COPY ["Webstore.CatalogInfrastructure/Webstore.CatalogInfrastructure.csproj", "Webstore.CatalogInfrastructure/"]
COPY ["WebStore.NotificationService/WebStore.NotificationService.csproj", "WebStore.NotificationService/"]
COPY ["WebStore.OrderApp/WebStore.OrderApp.csproj", "WebStore.OrderApp/"]
COPY ["WebStore.OrderDomain/WebStore.OrderDomain.csproj", "WebStore.OrderDomain/"]
COPY ["WebStore.OrderInfrastructure/WebStore.OrderInfrastructure.csproj", "WebStore.OrderInfrastructure/"]
COPY ["WebStore.UserInfrastructure/WebStore.UserInfrastructure.csproj", "WebStore.UserInfrastructure/"]
COPY ["WebStore.UserApp/WebStore.UserApp.csproj", "WebStore.UserApp/"]
COPY ["WebStore.UserDomain/WebStore.UserDomain.csproj", "WebStore.UserDomain/"]
RUN dotnet restore "./WebStore.BackWorker/WebStore.BackWorker.csproj"
COPY . .
WORKDIR "/src/WebStore.BackWorker"
RUN dotnet build "./WebStore.BackWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebStore.BackWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN mkdir -p Logs && chown -R 1654:1654 Logs
USER 1654

ENTRYPOINT ["dotnet", "WebStore.BackWorker.dll"]