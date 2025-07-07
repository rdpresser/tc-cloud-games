# Stage 1: Install OpenTelemetry auto-instrumentation as root
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS otel-install
ARG OTEL_VERSION=1.4.0
WORKDIR /otel

ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v${OTEL_VERSION}/otel-dotnet-auto-install.sh otel-dotnet-auto-install.sh

RUN apt-get update && apt-get install -y curl unzip && \
    mkdir -p /otel-dotnet-auto && \
    OTEL_DOTNET_AUTO_HOME="/otel-dotnet-auto" sh ./otel-dotnet-auto-install.sh && \
    ls -la /otel-dotnet-auto && \
    chmod +x /otel-dotnet-auto/instrument.sh

# Stage 2: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Directory.Build.props ./
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

# Stage 3: Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TC.CloudGames.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final runtime image with telemetry
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
USER $APP_UID
WORKDIR /app

# Copy published app and OpenTelemetry instrumentation
COPY --from=publish /app/publish .
COPY --from=otel-install /otel/otel-dotnet-auto /otel-dotnet-auto

# Enable OpenTelemetry Auto-Instrumentation
ENV OTEL_DOTNET_AUTO_HOME="/otel-dotnet-auto"

# This entrypoint wraps your app with OpenTelemetry
ENTRYPOINT ["/otel-dotnet-auto/instrument.sh", "dotnet", "TC.CloudGames.Api.dll"]
