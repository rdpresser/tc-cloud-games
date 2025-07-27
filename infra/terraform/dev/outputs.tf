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

output "key_vault_id" {
  description = "The ID of the Key Vault"
  value       = azurerm_key_vault.key_vault.id
}

output "key_vault_tenant_id" {
  description = "The tenant ID of the Key Vault"
  value       = azurerm_key_vault.key_vault.tenant_id
}

output "key_vault_sku" {
  description = "The SKU of the Key Vault"
  value       = azurerm_key_vault.key_vault.sku_name
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

output "postgres_server_id" {
  description = "The ID of the PostgreSQL server"
  value       = azurerm_postgresql_flexible_server.postgres_server.id
}

output "postgres_server_version" {
  description = "The version of the PostgreSQL server"
  value       = azurerm_postgresql_flexible_server.postgres_server.version
}

output "postgres_database_name" {
  description = "The name of the PostgreSQL database"
  value       = "tc_cloud_games"  # From your application configuration
}

output "postgres_port" {
  description = "The port of the PostgreSQL server"
  value       = 5432
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

output "container_registry_id" {
  description = "The ID of the Container Registry"
  value       = azurerm_container_registry.acr.id
}

output "container_registry_sku" {
  description = "The SKU of the Container Registry"
  value       = azurerm_container_registry.acr.sku
}

output "container_registry_admin_enabled" {
  description = "Whether admin user is enabled for the Container Registry"
  value       = azurerm_container_registry.acr.admin_enabled
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

output "log_analytics_workspace_location" {
  description = "The location of the Log Analytics workspace"
  value       = azurerm_log_analytics_workspace.log_analytics.location
}

output "log_analytics_workspace_sku" {
  description = "The SKU of the Log Analytics workspace"
  value       = azurerm_log_analytics_workspace.log_analytics.sku
}

output "log_analytics_workspace_retention_days" {
  description = "The retention period in days for the Log Analytics workspace"
  value       = azurerm_log_analytics_workspace.log_analytics.retention_in_days
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

output "container_app_environment_id" {
  description = "The ID of the Container App Environment"
  value       = azurerm_container_app_environment.container_app_environment.id
}

output "container_app_revision_mode" {
  description = "The revision mode of the Container App"
  value       = azurerm_container_app.container_app.revision_mode
}

output "container_app_ingress_external" {
  description = "Whether the Container App ingress is external"
  value       = azurerm_container_app.container_app.ingress[0].external_enabled
}

output "container_app_ingress_target_port" {
  description = "The target port of the Container App ingress"
  value       = azurerm_container_app.container_app.ingress[0].target_port
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

output "redis_cache_port" {
  description = "Non-SSL port of the Redis cache"
  value       = azurerm_redis_cache.redis_cache.port
}

output "redis_cache_sku_name" {
  description = "SKU name of the Redis cache"
  value       = azurerm_redis_cache.redis_cache.sku_name
}

output "redis_cache_family" {
  description = "Family of the Redis cache"
  value       = azurerm_redis_cache.redis_cache.family
}

output "redis_cache_capacity" {
  description = "Capacity of the Redis cache"
  value       = azurerm_redis_cache.redis_cache.capacity
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

output "azure_location" {
  description = "The Azure region where resources are deployed"
  value       = azurerm_resource_group.rg.location
}

output "deployment_timestamp" {
  description = "Timestamp of the deployment"
  value       = timestamp()
}

# =============================================================================
# Resource URLs & Endpoints
# =============================================================================

output "azure_portal_resource_group_url" {
  description = "Direct link to the resource group in Azure Portal"
  value       = "https://portal.azure.com/#@/resource/subscriptions/SUBSCRIPTION_ID/resourceGroups/${azurerm_resource_group.rg.name}/overview"
}

output "container_app_swagger_url" {
  description = "Direct link to the Container App Swagger documentation"
  value       = "https://${azurerm_container_app.container_app.ingress[0].fqdn}/swagger"
}

output "container_app_health_url" {
  description = "Direct link to the Container App health endpoint"
  value       = "https://${azurerm_container_app.container_app.ingress[0].fqdn}/health"
}

# =============================================================================
# Monitoring & Debugging Information
# =============================================================================

output "all_resource_names" {
  description = "Map of all created resource names for easy reference"
  value = {
    resource_group      = azurerm_resource_group.rg.name
    key_vault          = azurerm_key_vault.key_vault.name
    container_registry = azurerm_container_registry.acr.name
    container_app      = azurerm_container_app.container_app.name
    container_app_env  = azurerm_container_app_environment.container_app_environment.name
    postgres_server    = azurerm_postgresql_flexible_server.postgres_server.name
    redis_cache        = azurerm_redis_cache.redis_cache.name
    log_analytics      = azurerm_log_analytics_workspace.log_analytics.name
  }
}

output "connection_info" {
  description = "Non-sensitive connection information for debugging"
  value = {
    postgres_host = azurerm_postgresql_flexible_server.postgres_server.fqdn
    postgres_port = 5432
    redis_host    = azurerm_redis_cache.redis_cache.hostname
    redis_port    = azurerm_redis_cache.redis_cache.ssl_port
    acr_server    = azurerm_container_registry.acr.login_server
    app_url       = "https://${azurerm_container_app.container_app.ingress[0].fqdn}"
  }
}

# =============================================================================
# Deployment Summary
# =============================================================================

output "deployment_summary" {
  description = "Summary of the deployed infrastructure"
  value = {
    environment           = local.environment
    location             = azurerm_resource_group.rg.location
    resource_group       = azurerm_resource_group.rg.name
    unique_suffix        = random_string.unique_suffix.result
    total_resources      = "9"  # Approximate count of main resources
    application_url      = "https://${azurerm_container_app.container_app.ingress[0].fqdn}"
    deployment_timestamp = timestamp()
  }
}

output "infrastructure_health_checks" {
  description = "URLs for infrastructure health checks and monitoring"
  value = {
    application_health = "https://${azurerm_container_app.container_app.ingress[0].fqdn}/health"
    application_swagger = "https://${azurerm_container_app.container_app.ingress[0].fqdn}/swagger"
    azure_portal_rg = "https://portal.azure.com/#@/resource/subscriptions/SUBSCRIPTION_ID/resourceGroups/${azurerm_resource_group.rg.name}/overview"
    key_vault_portal = "https://portal.azure.com/#@/resource${azurerm_key_vault.key_vault.id}/overview"
    container_app_portal = "https://portal.azure.com/#@/resource${azurerm_container_app.container_app.id}/overview"
  }
}

# =============================================================================