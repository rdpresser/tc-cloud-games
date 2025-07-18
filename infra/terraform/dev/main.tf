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

# =============================================================================
# Random String for Globally Unique Resources
# =============================================================================

resource "random_string" "unique_suffix" {
  length  = 8
  upper   = false
  special = false
}

resource "azurerm_resource_group" "rg" {
  name     = var.azure_resource_group_name
  location = var.azure_resource_group_location
}

resource "azurerm_key_vault" "key_vault" {
  name                = "tc-cloudgames-kv-${random_string.unique_suffix.result}"
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
  value        = var.acr_name

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_acr_username" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "acr-username"
  value        = var.acr_admin_username

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_acr_password" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "acr-password"
  value        = var.acr_admin_password

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_container_app_name" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "container-app-name"
  value        = var.container_app_name

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

# =============================================================================
# Application Secrets
# =============================================================================

resource "azurerm_key_vault_secret" "key_vault_secret_cache_password" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "cache-password"
  value        = var.redis_cache_password

  depends_on = [
    azurerm_key_vault.key_vault
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
  value        = var.postgres_db_host

  depends_on = [
    azurerm_key_vault.key_vault
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
  value        = tostring(var.postgres_db_port)

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
  value        = var.redis_cache_host

  depends_on = [
    azurerm_key_vault.key_vault
  ]
}

resource "azurerm_key_vault_secret" "key_vault_secret_cache_port" {
  key_vault_id = azurerm_key_vault.key_vault.id
  name         = "cache-port"
  value        = tostring(var.redis_cache_port)

  depends_on = [
    azurerm_key_vault.key_vault
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
  name                = "tc-cloudgames-dev-db-${random_string.unique_suffix.result}"
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
