# =============================================================================
# Azure Resource Configuration
# =============================================================================

variable "azure_resource_group_location" {
  description = "Location for the resource group"
  type        = string
}

variable "azure_resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

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

variable "postgres_db_host" {
  description = "Hostname for PostgreSQL database"
  type        = string
}

variable "postgres_db_name" {
  description = "Name of the PostgreSQL database"
  type        = string
}

variable "postgres_db_port" {
  description = "Port for PostgreSQL database"
  type        = number
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
# Azure Redis Cache Configuration
# =============================================================================

variable "redis_cache_password" {
  type        = string
  description = "Password for Redis cache authentication"
  sensitive   = true
}

variable "redis_cache_host" {
  type        = string
  description = "Hostname for Redis cache"
}

variable "redis_cache_port" {
  type        = number
  description = "Port for Redis cache"
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

# =============================================================================
# Azure Infrastructure Configuration (for CI/CD)
# =============================================================================

variable "acr_name" {
  description = "Name of the Azure Container Registry"
  type        = string
}

variable "acr_admin_username" {
  description = "Admin username for the Azure Container Registry"
  type        = string
}

variable "acr_admin_password" {
  description = "Admin password for the Azure Container Registry"
  type        = string
  sensitive   = true
}

variable "container_app_name" {
  description = "Name of the Azure Container App"
  type        = string
}
