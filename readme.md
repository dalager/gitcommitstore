# CommitStore

This is the middle layer of the CommitStore system. It is responsible for retrieving commits from the commit queue and save them to a Azure Cosmos NOSQL database.

![Big Picture Diagram](docs/images/big_picture_diagram.png)

## Todos

### Authentication

The function keeps the connection strings to the Azure Cosmos DB and the Azure Storage Queue in the Function Configuration in the Azure Portal.
This is not a good practice.
The connection strings should be stored in the Azure Key Vault and retrieved from there.
Or the function should use Managed Service Identity to access the Azure Storage Queue and the Azure Cosmos DB.

#### Against Azure Storage Queue

Role: Storage Queue Data Message Processor
https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#storage-queue-data-message-processor

### Against Azure Cosmos DB

https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac

- grant data reader
- grant data contributor

https://learn.microsoft.com/en-us/azure/cosmos-db/managed-identity-based-authentication

### Monitoring

The function should be monitored with Application Insights.
Alerts should be configured for the following metrics:

- Number of failed executions against the Database
- Number of failed executions against the Queue
- Number of successful executions
