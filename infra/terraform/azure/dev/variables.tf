# =============================================================================
# Common Configuration Variables
# =============================================================================

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"

  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be dev, staging, or prod."
  }
}

variable "project_name" {
  description = "Project name used for resource naming"
  type        = string
  default     = "tc-cloudgames"

  validation {
    condition     = can(regex("^[a-z0-9-]+$", var.project_name))
    error_message = "Project name must contain only lowercase letters, numbers, and hyphens."
  }
}

# =============================================================================
# Azure Resource Configuration
# =============================================================================

variable "azure_resource_group_location" {
  description = "Location for the resource group"
  type        = string

  validation {
    condition = contains([
      "eastus", "eastus2", "westus", "westus2", "westus3",
      "centralus", "southcentralus", "northcentralus", "westcentralus",
      "canadacentral", "canadaeast", "brazilsouth",
      "northeurope", "westeurope", "francecentral", "germanywestcentral", "norwayeast",
      "uksouth", "ukwest", "switzerlandnorth", "swedencentral",
      "eastasia", "southeastasia", "japaneast", "japanwest", "koreacentral", "australiaeast", "australiasoutheast"
    ], var.azure_resource_group_location)
    error_message = "The azure_resource_group_location must be a valid Azure region."
  }
}

# Resource group name is now dynamically generated using name_prefix approach
# Pattern: "tc-cloudgames-dev-rg"

variable "azure_tenant_id" {
  description = "Azure Tenant ID"
  type        = string
}

# =============================================================================
# Database Configuration
# =============================================================================

variable "postgres_admin_login" {
  description = "Admin login for PostgreSQL database"
  type        = string
}

variable "postgres_admin_password" {
  description = "Admin password for PostgreSQL database"
  type        = string
  sensitive   = true
}

variable "postgres_db_name" {
  description = "Name of the PostgreSQL database"
  type        = string
}

# =============================================================================
# Identity and Access Management
# =============================================================================

variable "app_object_id" {
  description = "Object ID of the application in Azure AD"
  type        = string
}

variable "app_object_id_github_actions" {
  description = "Object ID of the GitHub Actions application in Azure AD"
  type        = string
}

variable "user_object_id" {
  description = "Object ID of the user in Azure AD"
  type        = string
}

# =============================================================================
# Grafana OpenTL Configuration
# =============================================================================
variable "grafana_logs_api_token" {
  type        = string
  description = "API token for Grafana Logs"
  sensitive   = true
}

variable "grafana_open_tl_exporter_endpoint" {
  type        = string
  description = "Endpoint for Grafana OpenTL Exporter"
}

variable "grafana_open_tl_exporter_protocol" {
  type        = string
  description = "Protocol for Grafana OpenTL Exporter"
}

variable "grafana_open_tl_resource_attributes" {
  type        = string
  description = "Resource attributes for Grafana OpenTL Exporter"
}

variable "grafana_open_tl_auth_header" {
  type        = string
  description = "Grafana OpenTL authentication header API key"
  sensitive   = true
}

variable "grafana_open_tl_prometheus" {
  type        = string
  description = "Grafana Cloud Prometheus API token for metrics ingestion"
  sensitive   = true
}