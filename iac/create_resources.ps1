$random = Get-Random -Minimum 1000 -Maximum 9999
$resourcegroup = "commitstore-rg"
$functionappname = "commitprocessor-4720"
$location = "westeurope"
$storageaccount = "commitstorestorage$random"
$storagequeue = "commitqueue"
$envvariable = "COMMITSTORE_QUEUE_URL"

$cosmosdb = "commitstore-cda"
$database = "commitstore-db"
$partitionKey = "/repository"
$container = "commits"

#
# Create resource group
az group create --name $resourcegroup --location $location

# Create storage account
az storage account create --name $storageaccount --resource-group $resourcegroup --location $location --sku Standard_LRS

# create db
az cosmosdb create -n $cosmosdb -g $resourcegroup --enable-free-tier true
az cosmosdb sql database create --account-name $cosmosdb --resource-group $resourcegroup --name $database
az cosmosdb sql container create --account-name $cosmosdb --resource-group $resourcegroup --database-name $database --name $container --partition-key-path $partitionKey --throughput 400 #--idx @idxpolicy-$randomIdentifier.json

# create function app
az functionapp create --resource-group $resourcegroup --consumption-plan-location $location --runtime dotnet-isolated --functions-version 4 --name $functionappname --storage-account $storageaccount

az functionapp config appsettings set -g $resourcegroup -n $functionappname --settings 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED=1'
az functionapp config set -g $resourcegroup -n $functionappname  --net-framework-version 'v8.0'
az functionapp config set -g $resourcegroup -n $functionappname --use-32bit-worker-process false

# enable system assigned identity
az functionapp identity assign -g $resourcegroup -n $functionappname

# get the identity
$identity = az functionapp identity show -g $resourcegroup -n $functionappname --query "principalId" -o tsv



# ------------------ THIS IS A TEST COMMITLOGGER QUEUE ----------------
# create queue
az storage queue create --name $storagequeue --account-name $storageaccount

# create shared access policy for pushing messages
az storage queue policy create --account-name $storageaccount --queue-name $storagequeue --name "addcommits" --permissions "a" --expiry "2028-12-31T23:59:00Z"

# create a sas token for pushing messages
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

