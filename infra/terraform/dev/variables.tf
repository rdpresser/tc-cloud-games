variable "resource_group_name" {
  default = "tc-cloudgames-rg"
}
variable "resource_group_location" {
  default = "brazilsouth"
}
variable "tenant_id" {
  default = "084169c0-a779-43c3-970c-487a71a93f88"
}
variable "postgres_admin_login" {
  type    = string
  default = "tccloudgamesadm"
}
variable "postgres_admin_password" {
  type      = string
  sensitive = true
}
