provider "aws" {
  region = var.aws_region

  # Default tags for all resources (equivalent to Azure common_tags)
  default_tags {
    tags = local.common_tags
  }
}

# =============================================================================
# Data Sources (AWS account information)
# =============================================================================

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

# =============================================================================
# Local Values (equivalent to your Azure locals)
# =============================================================================

locals {
  # Naming configuration (same pattern as Azure)
  environment  = var.environment
  project_name = var.project_name
  name_prefix  = "${local.project_name}-${local.environment}"

  # Common tags (equivalent to your Azure common_tags)
  common_tags = {
    Environment = local.environment
    Project     = "TC Cloud Games"
    ManagedBy   = "Terraform"
    Owner       = "DevOps Team"
    CostCenter  = "Engineering"
    Workspace   = terraform.workspace
    Provider    = "AWS"
  }
}

# =============================================================================
# Global Unique Naming (identical to Azure approach)
# =============================================================================

resource "random_string" "unique_suffix" {
  length  = 4
  upper   = false
  special = false
}

# =============================================================================
# AWS Resource Groups (equivalent to Azure Resource Group)
# =============================================================================

resource "aws_resourcegroups_group" "main" {
  name = "${local.name_prefix}-rg"

  # Create logical grouping using tags (like Azure RG)
  resource_query {
    query = jsonencode({
      ResourceTypeFilters = ["AWS::AllSupported"]
      TagFilters = [
        {
          Key    = "Project"
          Values = ["TC Cloud Games"]
        }
      ]
    })
  }

  tags = merge(local.common_tags, {
    Name        = "${local.name_prefix}-resource-group"
    Description = "Logical grouping for TC CloudGames AWS resources"
  })
}