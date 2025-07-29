terraform {
  cloud {
    organization = "rdpresser_tccloudgames_fiap"

    workspaces {
      name = "tc-cloudgames-aws-dev"
    }
  }

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }

    random = {
      source  = "hashicorp/random"
      version = "~> 3.6"
    }
  }

  required_version = ">= 1.12"
}
