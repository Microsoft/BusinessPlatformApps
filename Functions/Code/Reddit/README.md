#Introduction 
Pipeline that ingests Reddit data for use in PowerBI.

## Important Note
This folder structure has been exported from another git repository.  It is meant to be a reference for anyone who wants to see how this pipeline works, make changes, publish their own, and update their own Reddit ingest and ML pipeline.  Synchronization of internal repository to BusinessPlatformApps Github repository is manual.

## Projects
 - RedditAzureFunctions
 - RedditCore
 - RedditDatabase  - *Note*: This should be a 1:1 clone of the BusinessPlatformApps/Source/Apps/Microsoft/Release/Microsoft-RedditTemplate/Database folder.  Deltas should be treated (and reported!) as bugs.  This is included solely to simplify the already manual sync process.
 - RedditCoreTest

## Description
RedditCore contains the bulk of the processing logic.  It exists primarily so that our unit test project, RedditCoreTest, can use and access it.  A project of type Azure Function did not seem to give us any unit test capability.  The RedditAzureFunction is all about the set up and configuration of the function app that powers this Reddit ingest and analytics pipeline.  A local.settings.json file exists in the root of the RedditAzureFunctions folder and contains all of the properties necessary to run this (to run locally, please see: [Code and Test Azure Functions Locally](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local) ).  

You will need to create an instance of the Brand and Campaign Management for Reddit from Appsource to have a SocialGist key provisioned.  You can use this key for development, but the key is limited in uses per day and could be disabled if it runs afoul of SocialGist usage rules.  Please contact SocialGist for if you wish to do something more strenuous.

If you are merely testing the initial ingest portion, you can comment out the FunctionName annotation on ScheduledAzureML and AzureMLJobChecker.  You can then run completely locally using an embedded SQL database.

However, if you want to test the AzureML portion as well, you will need to create a SQL Server and Database in Azure.  You will also need to provision the AzureML portion, which can be found in BusinessPlatformApps/Source/Apps/Microsoft/Release/Microsoft-RedditTemplate/Service/AzureML.  This file will need to be modified by hand to point to your own database, storage account connection strings, TwitterModel.ilearner file, etc.  After this AzureML Web Service is created, you can use the address and web service key in the local.settings.json.  You will also need to change the SqlConnection in ConnectionStrings to point to your Azure SQL Database.  You can still run the FunctionApp locally, but the AzureML processing will be done in Azure.  Making changes to AzureML are quite problematic, though if you are interested you could probably reverse engineer the current web service into an AzureML experiment in your own workspace, modify it as needed, and build your own web service.  This is a complex process and well beyond the scope of this README.