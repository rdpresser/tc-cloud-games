# =============================================================================
# Basic AWS Outputs (equivalent to Azure outputs)
# =============================================================================

output "aws_account_id" {
  description = "AWS Account ID"
  value       = data.aws_caller_identity.current.account_id
}

output "aws_region" {
  description = "AWS region where resources are deployed"
  value       = data.aws_region.current.name
}

output "resource_group_name" {
  description = "AWS Resource Group name (logical grouping)"
  value       = aws_resourcegroups_group.main.name
}

output "resource_group_arn" {
  description = "AWS Resource Group ARN"
  value       = aws_resourcegroups_group.main.arn
}

output "unique_suffix" {
  description = "Unique suffix used in resource names"
  value       = random_string.unique_suffix.result
}

output "name_prefix" {
  description = "Common naming prefix for all resources"
  value       = local.name_prefix
}

# Summary output (like your Azure deployment_summary)
output "deployment_summary" {
  description = "Summary of deployed AWS resources"
  value = {
    project             = var.project_name
    environment         = var.environment
    region              = data.aws_region.current.name
    account_id          = data.aws_caller_identity.current.account_id
    resource_group      = aws_resourcegroups_group.main.name
    unique_suffix       = random_string.unique_suffix.result
    terraform_workspace = terraform.workspace
  }
}