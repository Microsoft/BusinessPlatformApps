Azure Activity Log Solution Template Documentation
===========================================================

# Table of Contents
1. [Introduction](#introduction)
2. [Architecture](#architecture)
3. [System Requirements](#system-requirements)
4. [How to Install](#how-to-install)
5. [Architecture Deep Dive](#architecture-deep-dive)
6. [Model Schema](#model-schema)
7. [Reports Walkthrough](#report-walkthrough)
8. [Customizations](#customizations)
9. [Estimated Costs](#estimated-costs)


### Introduction
The Azure Activity Log Solution Template offers analytics on top of historical and live Activity Log events. An Activity Log is a collection of subscription-level events that have occured in Azure, providing full visibility into any write operations performed on the resources in your subscription. The solution template is built on a streaming platform involving Event Hub and Stream Analytics. Streaming events are collected in an Azure SQL database on top of the past 90 days of Acitivty Log data we pull in to get you started with historical trend analysis. Refresh your Power BI report to see Activity Log events appear in near real-time, or set up a streaming data set in Power BI (see Customization section). 

With the Activity Log solution template, perform subscription-level analytics such as:

-   Observe week over week changes in the aggregate number of failures or percentage of failures.

-   Compare successful versus failed opereations for particular resource groups or callers in your subscription.

-   Stay up to date with relevant Service Health announcements by regions, type, or a specific impacted resource. 

The following document provides a walkthrough of the architecture, a deep dive into every component, comments on customizability as well as information on additional topics like estimated costs. For any questions not covered in this document, please contact the team at <PBISolnTemplates@microsoft.com>

### Architecture

![Image](Resources/media/azure-activity-log.png)

The flow of the Azure Activity Log solution template consists of the following resources:

-   Activity Log events are streamed to Event Hub via Log Profiles

-   Event Hub forwards the events to Stream Analytics

-   Stream Analytics processes the data and populates a SQL database

-   Power BI imports data from Azure SQL and renders pre-defined reports

### System Requirements

Setting up the template requires the following:

-   Access to an Azure subscription

-   Power BI Desktop (latest version)

-   Power BI Pro (to share the template with others)

### How to Install

Before diving into the components of the solution, we will go through how to set things up. To get started with the solution, navigate to the Activity Log template page and click **Install Now**.

**Getting Started:** Starting page introducing the template and explaining the architecture.

![Image](Resources/media/getting-started.PNG)

**Login:** Use OAuth to sign into your Azure account. You will notice you have a choice between signing into an organizational account and a Microsoft (work/school account).

If you select a Microsoft account, you will need to provide the application with a domain directory. You can find your domain by logging into the Azure portal and choosing from those listed when you click your e-mail in the top right hand corner.

![Image](Resources/media/login.PNG)
Logging into Azure gives the application access to your Azure subscription and permits spinning up Azure services on your behalf. If you want a more granular breakdown of the costs, please scroll down to the Estimated Costs section.

As a user navigates away from this page a new resource group gets spun up on their Azure subscription (the name is random but always prefixed by �SolutionTemplate-�). This name can be changed under the advanced settings tab. All newly created resources go into this container.

**Target:** Connect to an existing SQL Server or provide details which the application will use to spin up an Azure SQL on your behalf. Only Azure SQL is supported for this template. If a user chooses to spin up a new Azure SQL, this will get deployed in their Azure subscription inside the newly created resource group.

![Image](Resources/media/target.PNG)

**Summary:** Summary page outlining all the choices the user made.

![Image](Resources/media/summary.PNG)

**Deploy:** When you navigate to the deployment page the setup process gets kicked off. SQL scripts run to create the necessary tables and views. Event Hub and Stream Analytics are deployed and connected to begin streaming your Activity Log data. Historical data is pulled in from the last 90 days of your activity log. 

**It is important that you do not navigate away from this page while deployment takes place.** Once everything gets deployed a download link will appear for a Power BI file which consists of the pre-defined reports.

![Image](Resources/media/deploy.PNG)


### Architecture Deep Dive

The following section will break down how the template works by going through all the components of the solution.

![Image](Resources/media/azure-activity-log.png)

Azure Resources:
----------------

You can access all of the resources that have been spun up by logging into the Azure portal. Everything should be under one resource group (unless a user was using an existing SQL server. In this case the SQL Server will appear in whatever resource group it already existed in).

![Image](Resources/media/resources.PNG)


### Model Schema

The Azure Activity Logs team has published documentation on the Activity Log event schema [here](https://docs.microsoft.com/en-us/azure/monitoring-and-diagnostics/monitoring-activity-log-schema#administrative).

### Reports Walkthrough


### Customizations

How to set up real time streaming from the solution template to Power BI:

1.	Navigate to the Stream Analytics job through Azure Portal
    1.	In the Azure Portal, select “Resource groups” from the left pane
    2.	Locate “SolutionTemplate-xxxxxxx”, the resource group we created for you
    3.	Inside the resource group, navigate to “SAJob-xxxxx”, the Stream Analytics job
2.	Stop the Stream Analytics job
    1.	In the “Overview” pane ensure the job is stopped, which enables editing.
3.	Add a PowerBI output
    1.	In the “Outputs” pane, add a new output with the “Sink” parameter set to PowerBI.
    2.	Authenticate with PowerBI
    3.	Output alias: PBIOutput
        1.	Note: If you use a different output alias, you will have to use that same output alias in the SELECT … INTO [<output alias here>] when defining the query in the next step
    4.	Group Workspace: Select the workspace where you would like to set up the real-time streaming dataset 
    5.	Dataset Name: Enter a new name for the streaming dataset. 
        1.	Take note of this dataset name, as you must reference it in PowerBI when setting up real-time analytics.
    6.	Table Name: Enter a new name for the table, no need to take note.
    7.	Press “Save” at the bottom to save the new output
4.	Update your Stream Analytics Query
    1.	Navigate to the “Query” pane. 
    2.	Paste the query given below on a new line in the query window. NOTE: If you used a different output alias than “PBIOutput”, use that alias in the INTO clause at the bottom of this query:
    3.	Press “Save” to update the query

```sql
SELECT
    arrayElement.ArrayValue.[identity].claims.[http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn] as caller,
    arrayElement.ArrayValue.correlationId, 
    arrayElement.ArrayValue.resultDescription as description,
    arrayElement.ArrayValue.Properties.eventCategory,
    CASE WHEN arrayElement.ArrayValue.level = 'Information'
    THEN 'Informational'
    ELSE arrayElement.ArrayValue.level
    END as level,
    arrayElement.ArrayValue.category as operationCategory,
    arrayElement.ArrayValue.Properties.operationId,
    arrayElement.ArrayValue.operationName,
    CASE WHEN CHARINDEX('/', SUBSTRING(arrayElement.ArrayValue.resourceId, 68, 1000)) = 0   
     THEN SUBSTRING(arrayElement.ArrayValue.resourceId, 68, 1000)  
     ELSE SUBSTRING(arrayElement.ArrayValue.resourceId, 68, CHARINDEX('/', SUBSTRING(arrayElement.ArrayValue.resourceId, 68, 1000))-1)  
    END as resourceGroup,
    arrayElement.ArrayValue.resourceId,
    CASE WHEN CHARINDEX('/PROVIDERS', arrayElement.ArrayValue.resourceId) = 0
      THEN null
      ELSE SUBSTRING(arrayElement.ArrayValue.resourceId, CHARINDEX('/PROVIDERS', arrayElement.ArrayValue.resourceId) + 11, CHARINDEX('/', SUBSTRING(arrayElement.ArrayValue.resourceId, CHARINDEX('/PROVIDERS', arrayElement.ArrayValue.resourceId) + 11, 1000)) - 1)
    END as resourceProvider,
    CASE WHEN arrayElement.ArrayValue.resultType = 'Start'
        THEN 'Started'
        WHEN arrayElement.ArrayValue.resultType = 'Accept'
        THEN 'Accepted'
        ELSE arrayElement.ArrayValue.resultType
    END as status,
    arrayElement.ArrayValue.Properties.statusCode,
    arrayElement.ArrayValue.time as [timestamp]
INTO [PBIOutput] 
FROM [EventHubInput] event CROSS APPLY GetArrayElements(event.records) AS arrayElement
```

5.	Start the Stream Analytics job
    1.	In the “Overview” pane, start the Stream Analytics job. This restarts the pipeline we initially created for you, but now you are additionally sending data to a real-time data set. You are now just a few steps away from real-time analytics!
6.	Publish your existing .pbix
    1.	In Power BI Desktop, open your ActivityLogTemplate.pbix that you received after completing the solution template installation wizard 
    2.	In the “Home” tab press “Publish” to send the file to the web version of PowerBI 
    3.	When prompted, choose the same workspace that you provided in Step 3 when setting up a Power BI output for your Stream Analytics job
7.	Pin to a dashboard
    1.	After publishing from Power BI Desktop, refresh the web version of PowerBI and ensure that there is now a report listed under “Reports” on the left pane that matches the name of the Power BI Desktop file. 
    2.	Select a page from this report that you would like to place next to your real-time visualization, and press “Pin Live Page” at the top.
    3.	When prompted, opt to pin to a new dashboard and provide a name. 
8.	Create a report
    1.	Navigate to powerbi.microsoft.com and sign in. 
    2.	Expand your selected workspace, and select the dataset you created in Step 3 when setting up a Power BI output for your Stream Analytics job. 
        1.	Note: The dataset will only appear if data has flowed through Stream Analytics since starting the job in Step 5. If there has been no data transmitted, the dataset will not appear. If you believe your issue is unrelated, check the troubleshooting checklist at the bottom of this guide. 
    3.	Create your visual(s). In the top right corner of the visual, select the pin to pin the visual. 
    4.	When prompted, provide a new name for the report 
    5.	When prompted, choose to pin to an existing dashboard, the same one created in Step 7 above.
9.	Enjoy real-time streaming!

Here’s a checklist you can use to troubleshoot issues connecting Stream Analytics and Power BI: 
1. Restart the Azure Stream Analytics job (jobs created before the streaming GA release will require a restart) 
2. Try re-authorizing your Power BI connection in Azure Stream Analytics 
3. Which workspace did you specify in the Azure Stream Analytics output? In the Power BI service, are you checking in that (same) workspace? 
4. Does the Azure Stream Analytics query explicitly output to the Power BI output? (using the INTO keyword) 
5. Does the Azure Stream Analytics job have data flowing through it? The dataset will only get created when there is data being transmitted. 
6. Can you look into the Azure Stream Analytics logs to see if there are any warnings or errors?

For more details on using Power BI as a real-time data streaming service, see https://powerbi.microsoft.com/en-us/documentation/powerbi-service-real-time-streaming/ 


### Estimated Costs

Here is an estimate of the Azure costs (Event Hub, Stream Analytics, Azure SQL) involved:

The following defaults are set for you in the template (you can modify any of these after things get set up):

-   Stream Analytics : 1 Throughput Unit

-   Event Hub: 1 Throughput Unit

-   Azure SQL: Standard S1

Whilst the default setting should cater to most activity log template requirements, we encourage you to familiarize yourself with the various pricing options and tweak things to suit your needs.
