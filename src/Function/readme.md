# Azure function - Commitstore

## CommitTrigger function

- will be triggered by a message in azure commtstorage queue
- will write the json in the message payload to cosmos db

## SearchCommit function

- is a search endpoint returning the commits from cosmos db
