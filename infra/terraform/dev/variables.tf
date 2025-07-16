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
variable "app_object_id" {
  type    = string
  default = "d240991c-b9f9-446e-b890-0ff307e34ab4" 
}

variable "app_object_id_github_actions" {
  type    = string
  default = "311e0933-e635-4e8f-af82-a6e2bf200318" 
}

variable "user_object_id" {
  type    = string
  default = "887b7d92-985f-499b-b26d-9cc0785358cc" 
}