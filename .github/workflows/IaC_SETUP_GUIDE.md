# IaC Management Workflow Setup Guide

## Required GitHub Secrets

To use the IaC Management workflow, you need to configure the following secrets in your GitHub repository:

### 1. Azure Credentials
- **Secret Name**: `MAIN_AZURE_CREDENTIALS`
- **Value**: JSON object with Azure service principal credentials
- **Format**:
```json
{
  "clientId": "your-client-id",
  "clientSecret": "your-client-secret", 
  "subscriptionId": "your-subscription-id",
  "tenantId": "your-tenant-id"
}
```

### 2. Terraform Cloud Credentials
- **Secret Name**: `TF_API_TOKEN`
- **Value**: Your Terraform Cloud API token
- **How to get**: Go to Terraform Cloud → User Settings → Tokens → Create API token

- **Secret Name**: `TF_CLOUD_ORGANIZATION`
- **Value**: Your Terraform Cloud organization name

- **Secret Name**: `TF_WORKSPACE`
- **Value**: Your Terraform Cloud workspace name (e.g., "tc-cloudgames-dev")

## How to Add Secrets

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add each secret with the name and value as specified above

## Workflow Features

### 🔧 Available Actions

#### Azure CLI Actions (Database & Container App)
- **stop-all**: Stop both database and container app
- **start-all**: Start both database and container app  
- **stop-database**: Stop only PostgreSQL database
- **start-database**: Start only PostgreSQL database
- **stop-container-app**: Stop only Container App (scale to 0)
- **start-container-app**: Start only Container App (scale up)

#### Terraform Cloud Actions (Redis Cache)
- **destroy-redis**: Permanently destroy Redis cache (⚠️ requires confirmation)
- **recreate-redis**: Destroy and recreate Redis cache (⚠️ requires confirmation)

### 🛡️ Safety Features

1. **Confirmation Required**: Destructive actions (destroy-redis, recreate-redis) require explicit confirmation checkbox
2. **Status Checking**: Checks current resource state before performing actions
3. **Error Handling**: Graceful handling of already stopped/started resources
4. **Detailed Logging**: Comprehensive status reports and progress tracking

### 💰 Cost Optimization

| Action | Monthly Savings | Notes |
|--------|----------------|-------|
| stop-all | ~$30-80 | Maximum savings |
| stop-database | ~$20-50 | Database only |
| stop-container-app | ~$10-30 | Container App only |
| destroy-redis | ~$15-25 | Permanent destruction |

### 📋 Usage Examples

#### End of Business Day
```
Action: stop-all
Confirmation: Not required
Result: All compute resources stopped, maximum cost savings
```

#### Start of Business Day
```
Action: start-all
Confirmation: Not required  
Result: All resources active, full functionality restored
```

#### Redis Cache Issues
```
Action: destroy-redis
Confirmation: ✅ Required
Result: Redis cache destroyed, cache functionality lost
```

#### Fresh Redis Instance
```
Action: recreate-redis
Confirmation: ✅ Required
Result: Clean Redis cache, all previous data lost
```

### ⏰ Recommended Automation Schedule

You can extend this workflow to run on schedule by adding:

```yaml
on:
  schedule:
    # Stop resources at 6 PM UTC weekdays
    - cron: '0 18 * * 1-5'  
  workflow_dispatch:
    # ... existing manual triggers
```

And create separate scheduled workflows for starting resources at business hours.

### 🔗 Integration with Terraform Cloud

The workflow uses Terraform Cloud API to:
- Create targeted destroy/apply runs
- Monitor run status
- Provide direct links to Terraform Cloud for progress tracking
- Handle both destroy and recreation scenarios

### 📊 Monitoring

After running the workflow:
1. Check the **Actions** tab for detailed execution logs
2. Monitor Terraform Cloud workspace for infrastructure changes
3. Verify resource status in Azure portal
4. Review cost impact in Azure Cost Management

## Troubleshooting

### Common Issues

1. **Workspace not found**: Verify `TF_CLOUD_ORGANIZATION` and `TF_WORKSPACE` secrets
2. **Azure login failed**: Check `MAIN_AZURE_CREDENTIALS` format and permissions
3. **Resource not found**: Ensure resource naming matches the patterns in the workflow
4. **Terraform API errors**: Verify `TF_API_TOKEN` has sufficient permissions

### Resource Naming Patterns

The workflow looks for resources with these patterns:
- Database: Contains "tc-cloudgames-dev-db"
- Container App: Contains "tc-cloudgames-dev-api-app"  
- Redis: Managed via Terraform resource "azurerm_redis_cache.redis_cache"

Adjust these patterns in the workflow if your resource names differ.

## Security Considerations

- All secrets are encrypted in GitHub
- Azure CLI commands use service principal authentication
- Terraform Cloud API uses token-based authentication
- Destructive actions require explicit confirmation
- All actions are logged and auditable
