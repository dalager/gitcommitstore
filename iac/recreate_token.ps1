$resourcegroup = "commitstore-rg"
$storagequeue = "commitqueue"
$envvariable = "COMMITSTORE_QUEUE_URL"
#$envvariable = "COMMITLOGGER_QUEUE_URL"

# get the first storage account in the resource group
$storageaccount = az storage account list --resource-group $resourcegroup --query "[0].name" -o tsv

$token = az storage queue generate-sas --account-name $storageaccount --name $storagequeue --policy-name "addcommits" -o tsv 

Write-Output "SAS token for pushing messages:`n $token"

$storageendpoint = az storage account show --name $storageaccount --resource-group $resourcegroup --query "primaryEndpoints.queue" -o tsv

$queueposturl = "$storageendpoint$storagequeue/messages?$token"
write-output "This url can be used for posting messages to the queue for the next 5 years:`n $queueposturl"

Write-Output "saving to environment variable $envvariable"
# add as environment variable
[Environment]::SetEnvironmentVariable("$envvariable", $queueposturl, "User")

# ensure that the environment variable is available in the current session
$env:$envvariable = $queueposturl

Write-Output "Environment variable `n$envvariable `nis now set to `n$queueposturl"
write-output "It will be available to the git post-commit hook script"

