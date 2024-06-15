# CommitStore

This is the middle layer of the CommitStore system. It is responsible for retrieving commits from the commit queue and save them to a Azure CosmosDB.

It will receive git commit messages from the CommitLogger (https://github.com/dalager/GitHookedCommits2AzureQueue) and save them to Cosmos DB.

![Big Picture Diagram](docs/images/big_picture_diagram.png)

## Deployment

Deploys with github actions to Azure Functions.

Using this guide: https://github.com/Azure/actions-workflow-samples

See [deploy_functions_to_azure.yml](.github/workflows/deploy_functions_to_azure.yml)

![Azure Deployed](https://github.com/dalager/gitcommitstore/actions/workflows/deploy_functions_to_azure.yml/badge.svg)
