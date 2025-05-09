FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TC.CloudGames.Api/TC.CloudGames.Api.csproj", "src/TC.CloudGames.Api/"]
COPY ["src/TC.CloudGames.Application/TC.CloudGames.Application.csproj", "src/TC.CloudGames.Application/"]
COPY ["src/TC.CloudGames.Domain/TC.CloudGames.Domain.csproj", "src/TC.CloudGames.Domain/"]
COPY ["src/TC.CloudGames.Infra.CrossCutting.Commons/TC.CloudGames.Infra.CrossCutting.Commons.csproj", "src/TC.CloudGames.Infra.CrossCutting.Commons/"]
COPY ["src/TC.CloudGames.Infra.CrossCutting.IoC/TC.CloudGames.Infra.CrossCutting.IoC.csproj", "src/TC.CloudGames.Infra.CrossCutting.IoC/"]
COPY ["src/TC.CloudGames.Infra.Data/TC.CloudGames.Infra.Data.csproj", "src/TC.CloudGames.Infra.Data/"]
RUN dotnet restore "./src/TC.CloudGames.Api/TC.CloudGames.Api.csproj"

COPY . .
WORKDIR "/src/src/TC.CloudGames.Api"
RUN dotnet build "./TC.CloudGames.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TC.CloudGames.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TC.CloudGames.Api.dll"]
