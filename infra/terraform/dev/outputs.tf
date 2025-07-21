# =============================================================================
# OUTPUTS - Unique Resource Names
# =============================================================================

output "unique_suffix" {
  description = "The random suffix used for globally unique resources"
  value       = random_string.unique_suffix.result
}

# =============================================================================
# Key Vault Outputs
# =============================================================================

output "key_vault_name" {
  description = "The name of the Key Vault"
  value       = azurerm_key_vault.key_vault.name
}

output "key_vault_uri" {
  description = "The URI of the Key Vault"
  value       = azurerm_key_vault.key_vault.vault_uri
}

# =============================================================================
# PostgreSQL Outputs
# =============================================================================

output "postgres_server_name" {
  description = "The name of the PostgreSQL server"
  value       = azurerm_postgresql_flexible_server.postgres_server.name
}

output "postgres_server_fqdn" {
  description = "The fully qualified domain name of the PostgreSQL server"
  value       = azurerm_postgresql_flexible_server.postgres_server.fqdn
}

# =============================================================================
# Resource Group Output
# =============================================================================

output "resource_group_name" {
  description = "The name of the Resource Group"
  value       = azurerm_resource_group.rg.name
}

output "resource_group_id" {
  description = "The ID of the Resource Group"
  value       = azurerm_resource_group.rg.id
}

# =============================================================================
# Container Registry Outputs
# =============================================================================

output "container_registry_name" {
  description = "The name of the Container Registry"
  value       = azurerm_container_registry.acr.name
}

output "container_registry_login_server" {
  description = "The login server of the Container Registry"
  value       = azurerm_container_registry.acr.login_server
}

# =============================================================================
# Log Analytics Outputs
# =============================================================================

output "log_analytics_workspace_name" {
  description = "The name of the Log Analytics workspace"
  value       = azurerm_log_analytics_workspace.log_analytics.name
}

output "log_analytics_workspace_id" {
  description = "The ID of the Log Analytics workspace"
  value       = azurerm_log_analytics_workspace.log_analytics.id
}

# =============================================================================
# Container App Outputs
# =============================================================================

output "container_app_name" {
  description = "The name of the Container App"
  value       = azurerm_container_app.container_app.name
}

output "container_app_fqdn" {
  description = "The FQDN of the Container App"
  value       = azurerm_container_app.container_app.ingress[0].fqdn
}

output "container_app_url" {
  description = "The full URL of the Container App"
  value       = "https://${azurerm_container_app.container_app.ingress[0].fqdn}"
}

output "container_app_environment_name" {
  description = "The name of the Container App Environment"
  value       = azurerm_container_app_environment.container_app_environment.name
}

# =============================================================================
# Redis Cache Outputs
# =============================================================================

output "redis_cache_name" {
  description = "Name of the Redis cache"
  value       = azurerm_redis_cache.redis_cache.name
}

output "redis_cache_hostname" {
  description = "Hostname of the Redis cache"
  value       = azurerm_redis_cache.redis_cache.hostname
}

output "redis_cache_ssl_port" {
  description = "SSL port of the Redis cache"
  value       = azurerm_redis_cache.redis_cache.ssl_port
}

# =============================================================================
# Environment Information
# =============================================================================

output "environment" {
  description = "The deployment environment"
  value       = local.environment
}

output "resource_name_prefix" {
  description = "The naming prefix used for all resources"
  value       = local.name_prefix
}

# =============================================================================