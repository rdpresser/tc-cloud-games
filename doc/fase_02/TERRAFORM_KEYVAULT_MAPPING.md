# 📊 Mapeamento Completo: Terraform → Key Vault → Workflow

## 🗺️ **Tabela de Mapeamento Completa**

| **Categoria** | **Terraform Variable** | **Key Vault Secret** | **Uso no Workflow** | **Tipo** |
|--------------|------------------------|---------------------|-------------------|----------|
| **🏗️ Azure Resource Configuration** |
| | `azure_resource_group_location` | `resource-group-location` | `${{ steps.fetch-infra-secrets.outputs['resource-group-location'] }}` | config |
| | `azure_resource_group_name` | `resource-group-name` | `${{ steps.fetch-infra-secrets.outputs['resource-group-name'] }}` | config |
| | `azure_tenant_id` | `tenant-id` | `${{ steps.fetch-infra-secrets.outputs['tenant-id'] }}` | config |
| **🗄️ Database Configuration** |
| | `postgres_admin_login` | `db-user` | `${{ steps.fetch-app-secrets.outputs['db-user'] }}` | config |
| | `postgres_admin_password` | `db-password` | `secretref:db-password-secret` | secret |
| | `postgres_db_host` | `db-host` | `${{ steps.fetch-app-secrets.outputs['db-host'] }}` | config |
| | `postgres_db_name` | `db-name` | `${{ steps.fetch-app-secrets.outputs['db-name'] }}` | config |
| | `postgres_db_port` | `db-port` | `${{ steps.fetch-app-secrets.outputs['db-port'] }}` | config |
| **👥 Identity and Access Management** |
| | `app_object_id` | `app-object-id` | `${{ steps.fetch-infra-secrets.outputs['app-object-id'] }}` | config |
| | `app_object_id_github_actions` | `app-object-id-github-actions` | `${{ steps.fetch-infra-secrets.outputs['app-object-id-github-actions'] }}` | config |
| | `user_object_id` | `user-object-id` | `${{ steps.fetch-infra-secrets.outputs['user-object-id'] }}` | config |
| **⚡ Azure Redis Cache Configuration** |
| | `redis_cache_password` | `cache-password` | `secretref:cache-password-secret` | secret |
| | `redis_cache_host` | `cache-host` | `${{ steps.fetch-app-secrets.outputs['cache-host'] }}` | config |
| | `redis_cache_port` | `cache-port` | `${{ steps.fetch-app-secrets.outputs['cache-port'] }}` | config |
| **📊 Grafana OpenTL Configuration** |
| | `grafana_logs_api_token` | `grafana-api-token` | `secretref:grafana-api-token-secret` | secret |
| | `grafana_open_tl_exporter_endpoint` | `otel-endpoint` | `${{ steps.fetch-app-secrets.outputs['otel-endpoint'] }}` | config |
| | `grafana_open_tl_exporter_protocol` | `otel-protocol` | `${{ steps.fetch-app-secrets.outputs['otel-protocol'] }}` | config |
| | `grafana_open_tl_resource_attributes` | `otel-resource-attributes` | `${{ steps.fetch-app-secrets.outputs['otel-resource-attributes'] }}` | config |
| | `grafana_open_tl_auth_header` | `otel-auth-header` | `secretref:otel-auth-header-secret` | secret |
| **🐳 Azure Container Registry Configuration** |
| | `acr_name` | `acr-name` | `${{ steps.fetch-infra-secrets.outputs['acr-name'] }}` | config |
| | `acr_admin_username` | `acr-username` | `${{ steps.fetch-infra-secrets.outputs['acr-username'] }}` | config |
| | `acr_admin_password` | `acr-password` | `${{ steps.fetch-infra-secrets.outputs['acr-password'] }}` | secret |

## 🔍 **Legenda dos Tipos:**

- **config**: Valores não sensíveis usados diretamente nas environment variables
- **secret**: Valores sensíveis que são referenciados como `secretref:nome-do-secret`

## 📋 **Como os valores são usados no Container App:**

### ✅ **Valores diretos (config):**
```yaml
environmentVariables: >
  DB_HOST=${{ steps.fetch-app-secrets.outputs['db-host'] }}
  DB_PORT=${{ steps.fetch-app-secrets.outputs['db-port'] }}
  CACHE_HOST=${{ steps.fetch-app-secrets.outputs['cache-host'] }}
```

### 🔐 **Valores como secrets:**
```yaml
environmentVariables: >
  DB_PASSWORD=secretref:db-password-secret
  CACHE_PASSWORD=secretref:cache-password-secret
  GRAFANA_API_TOKEN=secretref:grafana-api-token-secret
```

## 🎯 **Secrets necessários para o workflow atual:**

### 🏗️ **Infrastructure Secrets:**
```
acr-name, container-app-name, resource-group-name, acr-username, acr-password
```

### 📱 **Application Secrets:**
```
db-password, cache-password, otel-auth-header, grafana-api-token, 
db-host, db-port, db-name, db-user, cache-host, cache-port, 
otel-endpoint, otel-protocol, otel-resource-attributes
```

## 📝 **Observações importantes:**

1. **Container App Name**: Não está definido no `variables.tf`, mas é necessário para o workflow
2. **Secrets vs Configs**: Valores marcados como `sensitive = true` no Terraform devem usar `secretref:`
3. **Nomenclatura**: Todos os secrets no Key Vault usam `kebab-case` para compatibilidade
4. **Flexibilidade**: Valores podem ser alterados no Key Vault sem modificar o pipeline
