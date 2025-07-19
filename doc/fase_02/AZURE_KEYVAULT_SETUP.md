# 📋 Azure Key Vault Configuration Guide

## 🎯 Objetivo
Centralizar configurações de infraestrutura e aplicação no Azure Key Vault, permitindo que o CI/CD seja totalmente dinâmico baseado na subscription ativa.

## 🔑 Secrets necessários no Azure Key Vault

### 🏗️ **Infrastructure Configuration**
```bash
# Informações básicas da infraestrutura
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "acr-name" --value "tccloudgamesregistry"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "container-app-name" --value "tc-cloudgames-api-container-app"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "resource-group-name" --value "tc-cloudgames-rg"

# Credenciais do Azure Container Registry
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "acr-username" --value "seu-acr-username"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "acr-password" --value "sua-acr-password"
```

### 🗄️ **Database Configuration**
```bash
# Configurações do PostgreSQL
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "db-host" --value "seu-postgres-host.postgres.database.azure.com"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "db-port" --value "5432"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "db-name" --value "tc-cloudgames-db"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "db-user" --value "seu-db-usuario"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "db-password" --value "sua-senha-super-secreta"
```

### ⚡ **Cache Configuration**
```bash
# Configurações do Redis
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "cache-host" --value "seu-redis-host.redis.cache.windows.net"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "cache-port" --value "6380"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "cache-password" --value "sua-redis-key"
```

### 📊 **Observability Configuration**
```bash
# Configurações do Grafana e OpenTelemetry
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "grafana-api-token" --value "seu-grafana-token"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "otel-auth-header" --value "Authorization=Basic base64(instance:token)"

# Configurações do OpenTelemetry Endpoint e Protocol
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "otel-endpoint" --value "https://otlp-gateway-prod-sa-east-1.grafana.net/otlp"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "otel-protocol" --value "http/protobuf"
az keyvault secret set --vault-name "tc-cloudgames-kv" --name "otel-resource-attributes" --value "service.name=tccloudgames-app,service.namespace=tccloudgames-app-group,deployment.environment=development"
```

## � **Lista completa de secrets necessários**

### 🔑 **Todos os secrets que devem estar no Key Vault:**

```bash
# Infrastructure
acr-name, container-app-name, resource-group-name, acr-username, acr-password

# Database  
db-host, db-port, db-name, db-user, db-password

# Cache
cache-host, cache-port, cache-password

# Observability
grafana-api-token, otel-auth-header, otel-endpoint, otel-protocol, otel-resource-attributes
```

## �🔧 Como funciona a descoberta automática

### 1. **Descoberta do Key Vault**
```bash
# O workflow automaticamente busca o Key Vault baseado na subscription ativa
KEYVAULT_NAME=$(az keyvault list --query "[?contains(name, 'tc-cloudgames')].name" -o tsv | head -1)
```

### 2. **Busca de secrets por categoria**
- **Infrastructure secrets**: Configurações de ACR, Container App, Resource Group
- **Application secrets**: Database, Cache, Observability

### 3. **Uso nos steps subsequentes**
Todos os valores são referenciados dinamicamente:
```yaml
ACR_NAME: ${{ steps.fetch-infra-secrets.outputs['acr-name'] }}
DB_PASSWORD: ${{ steps.fetch-app-secrets.outputs['db-password'] }}
```

## 🌍 Vantagens por ambiente

### ✅ **Benefícios da abordagem**

1. **Multi-ambiente**: Cada subscription tem seu próprio Key Vault com valores específicos
2. **Segurança**: Nenhuma configuração sensível fica hardcoded no código
3. **Flexibilidade**: Mudança de valores sem necessidade de redeploy do código
4. **Auditoria**: Todas as mudanças de configuração ficam auditadas no Key Vault
5. **Descoberta automática**: O workflow se adapta automaticamente ao ambiente ativo

### 🎭 **Cenários suportados**
- **Development**: `tc-cloudgames-dev-kv`
- **Staging**: `tc-cloudgames-stg-kv`  
- **Production**: `tc-cloudgames-prd-kv`

## 🚨 Importantes

### ⚠️ **Nomenclatura do Key Vault**
- Use sempre o padrão: `tc-cloudgames-{env}-kv`
- O workflow busca por Key Vaults que contenham `tc-cloudgames`

### 🔐 **Permissions necessárias**
```bash
# Dar permissão para o service principal do GitHub Actions
az keyvault set-policy --name "tc-cloudgames-kv" \
  --spn "seu-service-principal-id" \
  --secret-permissions get list
```

### 🏷️ **Nomenclatura dos secrets**
- Use sempre **kebab-case**: `db-password`, `acr-name`
- Evite underscores: ~~`db_password`~~
- Use a sintaxe de colchetes no GitHub Actions: `outputs['db-password']`

## 🔄 Migração das configurações atuais

### Remover do GitHub Secrets (se existirem):
- `AZURE_DB_HOST` → Move para Key Vault como `db-host`
- `AZURE_REGISTRY_USERNAME` → Move para Key Vault como `acr-username`
- `AZURE_CACHE_HOST` → Move para Key Vault como `cache-host`

### Manter no GitHub Secrets:
- `AZURE_CREDENTIALS` (credenciais para login no Azure)

## 🚀 **Script completo para configurar todos os secrets**

```bash
#!/bin/bash
# Script para configurar todos os secrets no Azure Key Vault
KEYVAULT_NAME="tc-cloudgames-kv"

echo "🔧 Configurando secrets de infraestrutura..."
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "acr-name" --value "tccloudgamesregistry"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "container-app-name" --value "tc-cloudgames-api-container-app"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "resource-group-name" --value "tc-cloudgames-rg"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "acr-username" --value "seu-acr-username"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "acr-password" --value "sua-acr-password"

echo "🗄️ Configurando secrets de database..."
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "db-host" --value "seu-postgres-host.postgres.database.azure.com"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "db-port" --value "5432"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "db-name" --value "tc-cloudgames-db"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "db-user" --value "seu-db-usuario"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "db-password" --value "sua-senha-super-secreta"

echo "⚡ Configurando secrets de cache..."
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "cache-host" --value "seu-redis-host.redis.cache.windows.net"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "cache-port" --value "6380"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "cache-password" --value "sua-redis-key"

echo "📊 Configurando secrets de observabilidade..."
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "grafana-api-token" --value "seu-grafana-token"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "otel-auth-header" --value "Authorization=Basic base64(instance:token)"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "otel-endpoint" --value "https://otlp-gateway-prod-sa-east-1.grafana.net/otlp"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "otel-protocol" --value "http/protobuf"
az keyvault secret set --vault-name "$KEYVAULT_NAME" --name "otel-resource-attributes" --value "service.name=tccloudgames-app,service.namespace=tccloudgames-app-group,deployment.environment=development"

echo "✅ Todos os secrets configurados no Key Vault: $KEYVAULT_NAME"
```

Esta abordagem garante que seu pipeline seja totalmente dinâmico e seguro! 🚀
