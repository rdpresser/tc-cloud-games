# Configure the Azure provider
terraform {
  required_version = ">= 1.1.0"
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~> 3.0.2"
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
  location            = var.resource_group_location
  name                = "tc-cloudgames-kv"
  resource_group_name = var.resource_group_name
  sku_name            = "standard"
  tenant_id           = var.tenant_id
  depends_on = [
    azurerm_resource_group.rg
  ]
}

resource "azurerm_postgresql_flexible_server" "res-14" {
  location            = var.resource_group_location
  name                = "tc-cloudgames-db"
  resource_group_name = var.resource_group_name
  zone                = "1"
  depends_on = [
    azurerm_resource_group.rg
  ]
}