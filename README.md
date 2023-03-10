# Azure Cosmos DB + Azure Open AI ChatGPT

This sample application combines Azure Cosmos DB with Open AI ChatGPT with a Blazor front-end for an intelligent chat bot application that is a slight clone 
of the OpenAi ChatGPT experience.

This application has individual chat sessions which are displayed in the left-hand nav. Clicking on a session will show the messages within that chat, 
separated by human prompts and AI completions within them. 

When a new prompt is sent to the Azure OpenAI service, some of the conversation history is sent with it. This helps provide context for the model that allows 
ChatGPT to respond as though it is having a conversation with correct context. The length of this conversation history can be configured from appsettings.json 
with the `OpenAiMaxTokens` value that is then translated to a maximum conversation string length that is 1/2 of this value. 

Please note that ChatGPT itself has a maximum of 4096 tokens and these are used in both the request and reponse from the service. Overriding the maxConversationLength to values 
approaching maximum token value could result in completions that contain little to no text if all of it has been used in the request.

The history for all User prompts and AI responses in each chat session is stored in Azure Cosmos DB. Deleting a chat session in the UI will delete it's corresponding data as well.

## Deploy the sample

To use this sample you can click the Deploy to Azure button below. This will provision the following resources:
1. Azure Cosmos DB account with database and container at 400 RU/s. This can optionally be configured to run on the Cosmos DB free tier if available for your subscription.
1. Azure App service. This also can be configured to run on App Service free tier.
1. Azure Open AI account. You must also specify a model deployment name which will be used as the name for the "text-davinci-003" model which is used by this application.

Note: You must have access to Azure Open AI service from your subscription before attempting to deploy this application.

The All connection information for Azure Cosmos DB and Open AI is zero-touch and injected as environment variables in the Azure App Service instance at deployment time. 


## Using the Azure Cosmos DB + Azure OpenAI ChatGPT application

1. After deployment, go to the resource group for your deployment and open the Azure App Service in the Azure Portal. Click the web url to launch the website.
1. Click + New Chat to create a new chat session.
1. Type your question in the text box and press Enter.

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzureCosmosDB%2Fcosmos-chatgpt%2Fmain%2Fazuredeploy.json)

Note: To clean up all the resources after deployment, you must first delete the deployed model within the Azure AI service. You can then delete the resource group for your deployment. This will delete all remaining resources associated with the deployment.
