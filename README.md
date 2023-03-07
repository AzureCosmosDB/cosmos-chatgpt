# Azure Cosmos DB + Open AI ChatGPT

This sample application combines Azure Cosmos DB with Open AI ChatGPT with a Blazor front-end for an intelligent chat bot application.

This application organizes individual chat sessions with human prompts and AI responses within them. To provide a more conversational feel to this application, each chat session can maintain a certain amount of history from prior User prompts and AI responses. The length of this history can be configured from appsettings.json with the `OpenAiMaxTokens` value where the maximum conversation length in bytes is 1/2 of this value. Please note that ChatGPT itself has a maximum of 4096 tokens.

To use this sample you can click the Deploy to Azure button below. This will provision the following resources:
1. Azure Cosmos DB account with database and container at 400 RU/s. This can optionally be configured to run on the Cosmos DB free tier if available for your subscription.
1. Azure App service. This also can be configured to run on App Service free tier.
1. Azure Open AI account. You must also specify a model deployment name which will be used as the name for the "text-davinci-003" model which is used by this application.

Note: You must have access to Azure Open AI service from your subscription before attempting to deploy this application.

The All connection information for Azure Cosmos DB and Open AI is zero-touch and injected as environment variables in the Azure App Service instance at deployment time. 



## Using the Cosmos DB + ChatGPT application

1. After deployment, go to the resource group for your deployment and open the Azure App Service in the Azure Portal. Click the web url to launch the website.
1. Click + New Chat to create a new chat session. You can then rename it to have a meaningful name for that conversation.
1. Type your question in the text box and press Enter.

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)]
(https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzureCosmosDB%2Fcosmos-chatgpt%2Fmain%2Fazuredeploy.json)


