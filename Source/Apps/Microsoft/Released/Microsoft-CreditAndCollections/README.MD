Dynamics 365 for Finance and Operations – Credit and Collections
=======================================================================
### Intro
With the release of the latest update for Dynamics 365 for Finance and Operations the capability for Solution Templates has also been added.
Thus, the first template based on Dynamics 365 for Finance and Operations, Credit and Collections, is now available.

### Requirements
To deploy the template the following are required:
- An active Azure Subscription
- An active Dynamics 365 for Finance and Operations instance
  - The user needs to be at least a System IT Administrator
- PowerBI desktop / subscription

### Technical walkthrough
With the update, a new OData entity has been added to the D365 Finance and Operations instance, **BpstConfigurationEntity**. Similar to the bring your own database capability in D365, this new entity allows you to store information about an aggregated measurement and a target database where that measurement is to be copied.
The entity is comprised of 4 fields:

- Id – automatically generated when a new entry is added
-	SqlConnectionString – the target database connection string
-	MeasurementName – the name of the aggregated measurement
-	ReportName – name of an associated PowerBI report

The entity also features an action - StartCopyJob. This takes in 2 parameters, connection string and measurement name. When called, it starts a job to copy the data from the specified measurement to the provided database.

#### Components and flow

The template is comprised of:
- An Azure SQL Database (bring your own database available)
- An Azure Logic App
  - Daily triggered, starts the copy process
- An Azure Logic App used for notifying the user once the data pull is complete (optional)
- An instance of Azure Analysis Services (optional)
- An Azure Function that is used to control the scheduling of the Analysis Services refresh (optional)

During the template deployment, a new entry is created in the entity with the user provided connection string, and **CustCollectionsBIMeasurements** as the MeasurementName. The entry is created in the users default organization.

The same entry is then used in the logic app to start the job and copy the required measurement.

The optional resources will get deployed according to the template configuration during deployment.
