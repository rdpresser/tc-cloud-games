terraform {
  cloud {
    organization = "rdpresser_tccloudgames_fiap"

    workspaces {
      name = "tc-cloud-games-aws"
    }
  }

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  required_version = ">= 1.12"
}
