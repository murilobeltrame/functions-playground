Push-Location .\.environment
terraform init
terraform apply -var="resource_group_name=functst" -auto-approve
Pop-Location