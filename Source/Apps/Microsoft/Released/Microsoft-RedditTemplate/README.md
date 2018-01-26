Reddit Solution Template Documentation
===========================================================

# Table of Contents
1. [Introduction](#introduction)


### Introduction

The Reddit template provides brand and campaign management for Reddit posts and comments.  The template stands up an end-to-end solution that queries the Social Gist's Reddit API, enriches the data using machine learning and stores these enrichments in Azure SQL. Users can then use pre-built Power BI reports that leverage Microsoft research technology to start exploring the data and finding insights.
The template is aimed at anyone who is interesting in following or tracking posts against Reddit.
The following document provides a walkthrough of the architecture. For any questions not covered in this document, please contact the team at <PBISolnTemplates@microsoft.com>


The flow of the Reddit solution template is as follows:

-   Azure Function #1 queries Reddit based on your search criteria.
-   Matching posts are placed on [Queue Storage](https://azure.microsoft.com/en-us/services/storage/queues/) for processing.
-   Azure Function #2 processes every comment from each post, enriching the data and writing it to Azure SQL.
-   Azure Function #3 runs periodically and calls AzureML textual analytics suite.
-   Power BI imports data into it from Azure SQL and renders pre-defined reports

###  Modifying Search Parameters
In general, modifying search parameters require you to make changes in two places; the Function App's App Settings property *QueryTerms* and the SQL Database table *UserDefinedEntityDefinitions*.

It's not strictly required that these be kept perfectly in sync.  UserDefinedEntityDefinitions are actually allowed to be a regex, while the QueryTerms must be a boolean search string following the rules specified by http://sphinxsearch.com/docs/current.html#boolean-syntax

Not only must the QueryTerms be in that form, but there can be only 32 terms.  UserDefinedEntityDefinitions are unlimited in size.  It might be that your search terms are few in number, but searching for a broad set of data.  You can then further restrict your results via the regex rules in *UserDefinedEntityDefinitions*.  

### Changing Frequency or Time
Your Reddit Solution Template is scheduled to begin execution at 01:00 GMT.  If you would like to run it at another time, you must change the Function App's App Settings property *ScheduledRedditQueryFrequency*.  The default value is _"0 0 1 * * *"_.  If you would like to set your start time to 22:00 GMT, you would change it to _"0 0 22 * * *"_.  

It is not recommended that you change the frequency to more than once per day.  Your SocialGist API key has a limited number of requests that can be made per day, and the system has no way of noting that this limit has been reached and to cease processing.  Not only will you stop receiving updates for that day, you will also incur computational and execution costs for the time spent making the attempts, even though they are doomed to fail.

It is also not recommended that you modify the *ScheduledAzureMLFrequency* setting.  This task runs every 30 minutes and attempts to update any new data that has not been enriched via Machine Learning processes.

See https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer for more details on valid values for the *ScheduledRedditQueryFrequency* property.

### Known Limitations
-  32 query parameters - if you have more, see if you can group them into broader terms and later further refine them via the *UserDefinedEntityDefinitions* entries

### Estimated Costs

Here is an estimate of the Azure costs:

Please keep in mind these are **estimated costs and subject to change.** For a more detailed breakdown of the various components please refer to the [Azure calculator](https://azure.microsoft.com/en-us/pricing/calculator/) and select the resources listed below. You can tweak all the options to see what the costs will look like and what modifications may suit your needs best.

The following defaults are set for you in the template (you can modify any of these after things get set up):

-   Azure SQL: Standard S1 database and server

-   App Service Plan: Consumption Small

-   Azure Functions (One call per Reddit post found + up to 150 overhead calls per day)

-   Two Standard LRS storage accounts

-   Azure ML (S1)

-   Socialgist API key (free during the trial period)

Whilst the default setting should cater to most Reddit template requirements, we encourage you to familiarize yourself with the various pricing options and tweak things to suit your needs.
