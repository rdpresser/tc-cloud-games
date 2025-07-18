# Configure the Azure provider
terraform {
  required_version = ">= 1.1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.1"
    }
  }
  cloud {
    organization = "rdpresser_tccloudgames_fiap"
    workspaces {
      name = "tc-cloud-games"
    }
  }
}

provider "azurerm" {
  features {}
}

# Local values for standard configurations
locals {
  postgres_port = 5432 # Standard PostgreSQL port
}

# =============================================================================
# Random String for Globally Unique Resources
# =============================================================================
resource "random_string" "unique_suffix" {
  length  = 4
  upper   = false
  special = false
}

resource "azurerm_resource_group" "rg" {
  name     = var.azure_resource_group_name
  location = var.azure_resource_group_location
}

resource "azurerm_key_vault" "key_vault" {
  name                = "tccloudgames-dev-kv-${random_string.unique_suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku_name            = "standard"
  tenant_id           = var.azure_tenant_id

  # Enable soft delete with purge protection disabled to allow purging
  soft_delete_retention_days = 7
  purge_protection_enabled   = false

  # 🔑 Application (Service Principal ou Managed Identity)
  access_policy {
    tenant_id = var.azure_tenant_id
    object_id = var.app_object_id # Service principal objectId

    key_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]

    certificate_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]
  }

  # 👤 User (Azure AD user objectId)
  access_policy {
    tenant_id = var.azure_tenant_id
    object_id = var.user_object_id # Usuário AAD

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]

    key_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]

    certificate_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]
  }

  # 🤖 GitHub Actions Service Principal
  access_policy {
    tenant_id = var.azure_tenant_id
    object_id = var.app_object_id_github_actions

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]

    key_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]

    certificate_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete", "Purge", "Recover", "Backup", "Restore"
    ]
  }

  depends_on = [
    azurerm_resource_group.rg
  ]
}

# =============================================================================
# Infrastructure Configuration Secrets (for CI/CD)
# =============================================================================

resource "azurerm_key_vault_secret" "key_vault_secret_acr_name" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "acr-name"
  value        = azurerm_container_registry.acr.name

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_container_registry.acr
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_acr_username" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "acr-username"
  value        = azurerm_container_registry.acr.admin_username

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_container_registry.acr
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_acr_password" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "acr-password"
  value        = azurerm_container_registry.acr.admin_password

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_container_registry.acr
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_container_app_name" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "container-app-name"
  value        = azurerm_container_app.tc_cloudgames_api.name

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_container_app.tc_cloudgames_api
  ]
}

# =============================================================================
# Application Secrets
# =============================================================================

resource "azurerm_key_vault_secret" "key_vault_secret_cache_password" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "cache-password"
  value        = azurerm_redis_cache.redis_cache.primary_access_key

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_redis_cache.redis_cache
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_db_password" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "db-password"
  value        = var.postgres_admin_password

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_grafana_api_token" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "grafana-api-token"
  value        = var.grafana_logs_api_token

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_otel_auth_header" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "otel-auth-header"
  value        = var.grafana_open_tl_auth_header

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

# =============================================================================
# Database Configuration Secrets
# =============================================================================

resource "azurerm_key_vault_secret" "key_vault_secret_db_admin_login" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "db-admin-login"
  value        = var.postgres_admin_login

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_db_host" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "db-host"
  value        = azurerm_postgresql_flexible_server.postgres_server.fqdn

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_postgresql_flexible_server.postgres_server
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_db_name" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "db-name"
  value        = var.postgres_db_name

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_db_port" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "db-port"
  value        = tostring(local.postgres_port)

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

# =============================================================================
# Redis Cache Configuration Secrets
# =============================================================================

resource "azurerm_key_vault_secret" "key_vault_secret_cache_host" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "cache-host"
  value        = azurerm_redis_cache.redis_cache.hostname

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_redis_cache.redis_cache
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_cache_port" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "cache-port"
  value        = tostring(azurerm_redis_cache.redis_cache.ssl_port)

  depends_on = [
    azurerm_key_vault.key_vault,
    azurerm_redis_cache.redis_cache
  ]
}

# =============================================================================
# Grafana OpenTL Configuration Secrets
# =============================================================================

resource "azurerm_key_vault_secret" "key_vault_secret_grafana_endpoint" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "grafana-endpoint"
  value        = var.grafana_open_tl_exporter_endpoint

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_grafana_protocol" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "grafana-protocol"
  value        = var.grafana_open_tl_exporter_protocol

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_grafana_resource_attributes" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "grafana-resource-attributes"
  value        = var.grafana_open_tl_resource_attributes

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_postgresql_flexible_server" "postgres_server" {
  name                = "tccloudgames-dev-db-${random_string.unique_suffix.result}"
  location            = "canadacentral"
  resource_group_name = azurerm_resource_group.rg.name
  zone                = "1"

  administrator_login    = var.postgres_admin_login
  administrator_password = var.postgres_admin_password

  sku_name = "B_Standard_B1ms"

  storage_mb = 32768 # 32 GB
  version    = "16"

  backup_retention_days        = 7
  geo_redundant_backup_enabled = false

  depends_on = [
    azurerm_resource_group.rg
  ]
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "postgres_server_firewall_rule_01" {
  end_ip_address   = "0.0.0.0"
  name             = "AllowAllAzureServicesAndResourcesWithinAzureIps_2025-7-15_10-48-25"
  server_id        = azurerm_postgresql_flexible_server.postgres_server.id
  start_ip_address = "0.0.0.0"
}
resource "azurerm_postgresql_flexible_server_firewall_rule" "postgres_server_firewall_rule_02" {
  end_ip_address   = "179.216.21.147"
  name             = "ClientIPAddress_2025-7-15_10-36-11"
  server_id        = azurerm_postgresql_flexible_server.postgres_server.id
  start_ip_address = "179.216.21.147"
}

# =============================================================================
# Azure Container Registry
# =============================================================================

resource "azurerm_container_registry" "acr" {
  name                = "tccloudgamesacr${random_string.unique_suffix.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true

  tags = {
    Environment = "Development"
    Project     = "TC Cloud Games"
    ManagedBy   = "Terraform"
  }
}

# =============================================================================
# Log Analytics Workspace
# =============================================================================

resource "azurerm_log_analytics_workspace" "log_analytics" {
  name                = "tc-cloudgames-logs-${random_string.unique_suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = {
    Environment = "dev"
    ManagedBy   = "terraform"
    Project     = "tc-cloudgames"
  }
}

# =============================================================================
# Container App Environment
# =============================================================================

resource "azurerm_container_app_environment" "container_app_environment" {
  name                       = "managedEnvironment-tccloudgamesrg-${random_string.unique_suffix.result}"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.log_analytics.id
}

# =============================================================================
# Container App
# =============================================================================

resource "azurerm_container_app" "tc_cloudgames_api" {
  name                         = "tc-cloudgames-api-app"
  container_app_environment_id = azurerm_container_app_environment.container_app_environment.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  # Registry configuration using the new ACR
  registry {
    server               = azurerm_container_registry.acr.login_server
    username             = azurerm_container_registry.acr.admin_username
    password_secret_name = "acr-password-secret"
  }

  # Secrets management - Using placeholder values that will be updated by CI/CD
  # The CI/CD pipeline uses: az containerapp secret set --secrets <values from Key Vault>
  secret {
    name  = "acr-password-secret"
    value = azurerm_container_registry.acr.admin_password
  }
  secret {
    name  = "db-password-secret"
    value = "placeholder-updated-by-cicd"
  }
  secret {
    name  = "cache-password-secret"
    value = "placeholder-updated-by-cicd"
  }
  secret {
    name  = "otel-auth-header-secret"
    value = "placeholder-updated-by-cicd"
  }
  secret {
    name  = "grafana-api-token-secret"
    value = "placeholder-updated-by-cicd"
  }

  # Ingress configuration
  ingress {
    external_enabled = true
    target_port      = 8080

    # Optional: Improve security
    allow_insecure_connections = false # Force HTTPS

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  # Container template
  template {
    container {
      name   = "tc-cloudgames-api-container"
      image  = "${azurerm_container_registry.acr.login_server}/tc-cloudgames-api-app:latest"
      cpu    = 0.5
      memory = "1Gi"

      # Environment variables aligned with CI/CD workflow
      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Development"
      }

      # Database configuration
      env {
        name  = "DB_HOST"
        value = azurerm_postgresql_flexible_server.postgres_server.fqdn
      }
      env {
        name  = "DB_PORT"
        value = tostring(local.postgres_port)
      }
      env {
        name  = "DB_NAME"
        value = var.postgres_db_name
      }
      env {
        name  = "DB_USER"
        value = var.postgres_admin_login
      }
      env {
        name        = "DB_PASSWORD"
        secret_name = "db-password-secret"
      }

      # Cache configuration - Using actual Redis resource values
      env {
        name  = "CACHE_HOST"
        value = azurerm_redis_cache.redis_cache.hostname
      }
      env {
        name  = "CACHE_PORT"
        value = tostring(azurerm_redis_cache.redis_cache.ssl_port)
      }
      env {
        name        = "CACHE_PASSWORD"
        secret_name = "cache-password-secret"
      }

      # Observability configuration - Using env vars from CI/CD
      env {
        name  = "OTEL_EXPORTER_OTLP_ENDPOINT"
        value = var.grafana_open_tl_exporter_endpoint
      }
      env {
        name  = "OTEL_EXPORTER_OTLP_PROTOCOL"
        value = var.grafana_open_tl_exporter_protocol
      }
      env {
        name        = "OTEL_EXPORTER_OTLP_HEADERS"
        secret_name = "otel-auth-header-secret"
      }
      env {
        name        = "GRAFANA_API_TOKEN"
        secret_name = "grafana-api-token-secret"
      }
      env {
        name  = "OTEL_RESOURCE_ATTRIBUTES"
        value = var.grafana_open_tl_resource_attributes
      }
    }

    # Scaling configuration
    min_replicas = 0
    max_replicas = 10

    http_scale_rule {
      name                = "http-scaler"
      concurrent_requests = "10"
    }
  }
}

# =============================================================================
# Azure Cache for Redis
# =============================================================================

resource "azurerm_redis_cache" "redis_cache" {
  name                = "tc-cloudgames-redis-${random_string.unique_suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  capacity            = 0
  family              = "C"
  sku_name            = "Basic"

  # Security settings
  non_ssl_port_enabled = false
  minimum_tls_version  = "1.2"

  # Access settings
  public_network_access_enabled = true

  # Redis configuration - Fixed for azurerm provider v4.x
  redis_configuration {
    # Memory management settings (these are the correct properties)
    maxmemory_reserved = 2
    maxmemory_delta    = 2
    maxmemory_policy   = "volatile-lru"
  }

  tags = {
    Environment = "Development"
    Project     = "TC Cloud Games"
    ManagedBy   = "Terraform"
  }

  depends_on = [
    azurerm_resource_group.rg
  ]
}