# Azure function - Commitstore

## overview

- will be triggered by a message in azure commtstorage queue
- will write the json in the message payload to cosmos db

## Setup trigger

- create a new function app
- create a new function
- select "Azure Queue Storage trigger"
- select the queue you want to listen to

## create function app with az
