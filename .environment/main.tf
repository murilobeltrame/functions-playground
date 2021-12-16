terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.65"
    }
  }
  required_version = ">= 0.14.9"
}

provider "azurerm" {
  features {}
}

variable "resource_group_name" {}
variable "resources_location" {
  default = "westus2"
}
variable "sb_authorization_rule_name" {
  default = "SendAndListenPolicy"
}
variable "mssql_server_username" {
  default = "4dm1n157r470r"
}
variable "mssql_server_password" {
  default = "4-v3ry-53cr37-p455w0rd"
}
variable "mssql_database_name" {
  default = "mytstdb"
}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.resources_location
}

## SERVICE BUS CONFIGURATION

resource "azurerm_servicebus_namespace" "sb" {
  name                = "${azurerm_resource_group.rg.name}sb"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.resources_location
  sku                 = "Standard"
}

resource "azurerm_servicebus_namespace_authorization_rule" "sb_auth_rule" {
  name                = var.sb_authorization_rule_name
  namespace_name      = azurerm_servicebus_namespace.sb.name
  resource_group_name = azurerm_resource_group.rg.name
  listen              = true
  send                = true
  manage              = false
}

resource "azurerm_servicebus_topic" "sb_topic_product_changed" {
  name                = "product_changed"
  resource_group_name = azurerm_resource_group.rg.name
  namespace_name      = azurerm_servicebus_namespace.sb.name
  default_message_ttl = "P14D"
  auto_delete_on_idle = "P14D"
}

resource "azurerm_servicebus_subscription" "sb_subscription_product_changed" {
  name                                      = "product_changed_subscription"
  resource_group_name                       = azurerm_resource_group.rg.name
  namespace_name                            = azurerm_servicebus_namespace.sb.name
  topic_name                                = azurerm_servicebus_topic.sb_topic_product_changed.name
  max_delivery_count                        = 10
  default_message_ttl                       = "P14D"
  auto_delete_on_idle                       = "P14D"
  dead_lettering_on_filter_evaluation_error = true
  dead_lettering_on_message_expiration      = true
}

## MSSQL DATABASE

resource "azurerm_mssql_server" "mssql_server" {
  name                         = "${azurerm_resource_group.rg.name}db"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.mssql_server_username
  administrator_login_password = var.mssql_server_password
}

resource "azurerm_mssql_database" "mssql_database" {
  name                        = var.mssql_database_name
  server_id                   = azurerm_mssql_server.mssql_server.id
  collation                   = "SQL_Latin1_General_CP1_CI_AS"
  sku_name                    = "GP_S_Gen5_2"
  min_capacity                = "1"
  auto_pause_delay_in_minutes = "60"
}

data "http" "myip" {
  url = "http://ipv4.icanhazip.com"
}

resource "azurerm_mssql_firewall_rule" "mssql_firewall" {
  name             = "AzureResourceAccessFirewallRule"
  server_id        = azurerm_mssql_server.mssql_server.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_mssql_firewall_rule" "mssql_firewall_local_env" {
  name             = "LocalEnvFirewallRule"
  server_id        = azurerm_mssql_server.mssql_server.id
  start_ip_address = chomp(data.http.myip.body)
  end_ip_address   = chomp(data.http.myip.body)
}

## FUNCTION APP

resource "azurerm_storage_account" "app_storage" {
  name                     = "${azurerm_resource_group.rg.name}sa"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "app_service_plan" {
  name                = "${azurerm_resource_group.rg.name}sp"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  kind                = "FunctionApp"
  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

data "azurerm_servicebus_namespace_authorization_rule" "sb_auth_rule" {
  name                = var.sb_authorization_rule_name
  namespace_name      = azurerm_servicebus_namespace.sb.name
  resource_group_name = azurerm_resource_group.rg.name
}

data "azurerm_mssql_server" "mssql_server" {
  name                = "${azurerm_resource_group.rg.name}db"
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_function_app" "app" {
  name                       = "${azurerm_resource_group.rg.name}app"
  resource_group_name        = azurerm_resource_group.rg.name
  location                   = azurerm_resource_group.rg.location
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.app_storage.name
  storage_account_access_key = azurerm_storage_account.app_storage.primary_access_key
  app_settings = {
    BrokerConnectionString   = data.azurerm_servicebus_namespace_authorization_rule.sb_auth_rule.primary_connection_string
    DatabaseConnectionString = "Server=tcp:${data.azurerm_mssql_server.mssql_server.fully_qualified_domain_name},1433;Initial Catalog=${var.mssql_database_name};Persist Security Info=False;User ID=${var.mssql_server_username};Password=${var.mssql_server_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}

# Outputs

output "BrokerConnectionString" {
  value     = data.azurerm_servicebus_namespace_authorization_rule.sb_auth_rule.primary_connection_string
  sensitive = true
}

output "DatabaseConnectionString" {
  value     = "Server=tcp:${data.azurerm_mssql_server.mssql_server.fully_qualified_domain_name},1433;Initial Catalog=${var.mssql_database_name};Persist Security Info=False;User ID=${var.mssql_server_username};Password=${var.mssql_server_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  sensitive = true
}

# Server=tcp:myfunctstdb.database.windows.net,1433;
# Initial Catalog=mytstdb;Persist Security Info=False;
# User ID=4dm1n157r470r;Password=4-v3ry-53cr37-p455w0rd;
# MultipleActiveResultSets=False;Encrypt=True;
# TrustServerCertificate=False;Connection Timeout=30;
