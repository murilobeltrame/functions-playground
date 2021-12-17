Push-Location .\.environment
terraform destroy -var="resource_group_name=functst" -auto-approve
Pop-Location