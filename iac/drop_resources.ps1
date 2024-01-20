$resourcegroup = "commitstore-rg"
$envvariable = "COMMITSTORE_QUEUE_URL"

function prompt([string]$query) {
    while ($true) {
        $choice = read-host -prompt $query
        switch ($choice) {
            Y { return $true }
            N { return $false }
            default { Write-Host "Please enter Y or N" }
        }
    }
}

Write-Output "This script will"
Write-Output "1. Delete the resource group $resourcegroup"
Write-Output "2. Delete the environment variable $envvariable"
Write-Output "`nAll data and queue messages in the storage account will be lost!!"

$continue = prompt "Are you sure you want to drop all resources in $resourcegroup  (Y/N)"

if ($continue -eq $false) {
    exit
}


# delete resource group
write-output "deleting resource group $resourcegroup"
az group delete --name $resourcegroup --yes


# delete environment variable
write-output "deleting environment variable $envvariable"
[Environment]::SetEnvironmentVariable("$envvariable", $null, "User")
