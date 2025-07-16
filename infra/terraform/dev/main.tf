# Configure the Azure provider
terraform {
  required_version = ">= 1.1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
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

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.resource_group_location
}

resource "azurerm_key_vault" "key_vault" {
  name                = "tc-cloudgames-dev-kv"
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  sku_name            = "standard"
  tenant_id           = var.tenant_id

  # 🔑 Application (Service Principal ou Managed Identity)
  access_policy {
    tenant_id = var.tenant_id
    object_id = var.app_object_id # Service principal objectId

    key_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete"
    ]

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Recover", "Backup"
    ]
  }

  # 👤 User (Azure AD user objectId)
  access_policy {
    tenant_id = var.tenant_id
    object_id = var.user_object_id # Usuário AAD

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Recover", "Backup"
    ]

    key_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete"
    ]
  }

  # 👤 User (Azure AD user objectId)
  access_policy {
    tenant_id = var.tenant_id
    object_id = var.app_object_id_github_actions

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Recover", "Backup"
    ]

    key_permissions = [
      "Get", "List", "Update", "Create", "Import", "Delete"
    ]
  }

  depends_on = [
    azurerm_resource_group.rg
  ]
}


resource "azurerm_postgresql_flexible_server" "postgres_server" {
  name                = "tc-cloudgames-dev-db"
  location            = "canadacentral"
  resource_group_name = var.resource_group_name
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
