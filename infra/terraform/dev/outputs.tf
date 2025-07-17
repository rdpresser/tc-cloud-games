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