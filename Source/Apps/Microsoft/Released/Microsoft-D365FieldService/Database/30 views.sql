CREATE VIEW [fso].[BookingStatusView]
AS
SELECT   
	bookingstatusid					as [Booking Status Id],
	status							as [Booking Status Code],
	bookingstatus.Label				as [Booking Status Name],
	fsStatus.Label					as [Booking System Status],
	msdyn_fieldservicestatus		as [Booking System Status Code] --msdyn_bookingsystemstatus global optionset
FROM dbo.bookingstatus bs
	left outer join 
	   (select  [Option] as [Value], [LocalizedLabel] as [Label] from [dbo].[OptionSetMetadata] 
	    where [OptionsetName] = 'status'
       --and [isUserLocalizedLabel] = 1
       and [LocalizedLabelLanguageCode] = 1033
       and [EntityName] = 'bookingstatus')
	   bookingstatus	on status = bookingstatus.Value
	left outer join 
	(
	select 
	   [Option] as [Value], 
	   [LocalizedLabel] as [Label] 
       from [dbo].[GlobalOptionSetMetadata] 
       where [OptionsetName] = 'msdyn_fieldservicestatus'
       --and [isUserLocalizedLabel] = 1
       and [LocalizedLabelLanguageCode] = 1033
	)  fsStatus		on fsStatus.Value = msdyn_fieldservicestatus
WHERE 
 not (((statecode != 0) and (DATEDIFF(YEAR, GETDATE(), ModifiedOn) < -1)))
GO

CREATE VIEW [fso].[ResourceBookingView]
AS
SELECT
	bookingstatus						as [Booking Status Id],
	bookingtype							as [Booking Type Code],
	bsv.[Booking Status Name]           as [Booking Status],
	bsv.[Booking System Status]         as [Booking System Status],
	BookingType.Label					as [Booking Type],
    starttime                           as [Start Time],
    endtime                             as [End Time],
	duration							as [Booking Duration Minutes],
	duration/60.0						as [Booking Duration Hours],
	CONVERT(date, starttime)			as [Booking Date],
	msdyn_resourcegroup					as [Booked Resource Group Id],
	resource							as [Booked Resource Id],
	msdyn_workorder						as [Booked Resource Work Order Id],
	msdyn_actualtravelduration			as [Actual Travel Time],
	msdyn_estimatedtravelduration		as [Estimated Travel Time],
	msdyn_actualarrivaltime				as [Actual Arrival Time],
	msdyn_estimatedarrivaltime			as [Estimated Arrival Time],
	DATEDIFF(MI, msdyn_estimatedarrivaltime, msdyn_actualarrivaltime)		as [Minutes Late],
	bookableresourcebookingid			as [Resource Booking Id],
	msdyn_totalcost_base				as [Booked Resource Total Cost]

FROM	dbo.bookableresourcebooking
	LEFT OUTER JOIN 
	(
       select 
       [Option] as [Value], 
       [LocalizedLabel] as [Label] 
       from [dbo].[OptionSetMetadata] 
       where [OptionsetName] = 'bookingtype'
       --and [isUserLocalizedLabel] = 1
       and [LocalizedLabelLanguageCode] = 1033
       and [EntityName] = 'bookableresourcebooking'
	) BookingType on BookingType.Value = bookingtype
	LEFT OUTER JOIN fso.bookingstatusview	bsv on bsv.[Booking Status Id] = bookingstatus 
WHERE
	(DATEDIFF(YEAR, GETDATE(), endtime) >= -1)	
AND (DATEDIFF(DAY, GETDATE(), starttime) <= 366)
AND NOT msdyn_workorder IS NULL
GO

CREATE VIEW [fso].[CustomerEquipmentView]
	AS 
	SELECT 
		msdyn_account							as [Customer Id],
		msdyn_product							as [Product Id],
		msdyn_customerassetId					as [Equipment Id],
		msdyn_name								as [Equipment Name],
		msdyn_parentasset						as [Parent Asset Id],
		msdyn_masterasset						as [Master Asset Id],
		msdyn_latitude							as [Equipment Latitude],
		msdyn_longitude							as [Equipment Longitude],
		statuscode								as [Equipment Status Code],
		statuscode.Label						as [Equipment Status]
	
	FROM dbo.msdyn_customerasset
		left outer join 
		(  select 
           [State] as [Value], 
           [LocalizedLabel] as [Label] 
           from [dbo].[StateMetadata]
           where [EntityName] = 'msdyn_customerasset'
           and [isUserLocalizedLabel] = 1
           and [LocalizedLabelLanguageCode] = 1033
		) statuscode on statuscode.Value = statuscode
	WHERE
		not (((statecode != 0) and (DATEDIFF(YEAR, GETDATE(), ModifiedOn) < -1)))
GO


CREATE VIEW [fso].[CustomerView]
AS
SELECT
	accountid							as [Customer Id],
	name								as [Customer Name],
	sic									as [Customer SIC],
	address1_postalcode					as [Customer Postal Code],
	address1_telephone1					as [Customer Phone],
	address1_stateorprovince			as [Customer State or Province],
	address1_country					as [Customer Country],
	address1_city						as [Customer City],
	emailaddress1						as [Customer E-Mail],
	customertypecode.Label				as [Customer Type],
	statecode							as [Customer State Code],
	statecode.Label						as [Customer State]
FROM dbo.account
	left outer join (
	
        select 
        [Option] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[OptionSetMetadata] 
        where [OptionsetName] = 'customertypecode'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
        and [EntityName] = 'account'
    ) CustomerTypeCode on CustomerTypeCode.Value = customertypecode
	left outer join (
        select 
        [State] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[StateMetadata]
        where [EntityName] = 'account'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) statecode on statecode.Value = statecode

WHERE
not (((statecode != 0) and (DATEDIFF(YEAR, GETDATE(), ModifiedOn) < -1)))
/*Active customers and customers that became inactive during last year*/

GO


CREATE VIEW [fso].[DateView]
AS
    SELECT date_key,
           full_date        AS [Date],
           day_of_week      AS [Day of the Week],
           day_num_in_month AS [Day Number of the Month],
           day_name         AS [Day Name],
           day_abbrev       AS [Day Abbreviated],
           weekday_flag     AS [Weekday Flag],
           [month],
           month_name       AS [Month Name],
           month_abbrev,
           [quarter],
           [year],
           same_day_year_ago_date,
           week_begin_date  AS [Week Begin Date],
           dateadd(dd,-day_num_in_month+1, full_date) AS [Month Begin Date]
	FROM   fso.[date];
GO


CREATE VIEW [fso].[IncidentTypeView]
	AS 
SELECT 
	msdyn_incidenttypeId		as [Incident Type Id],
	msdyn_name					as [Incident Type],
	msdyn_estimatedduration		as [Incident Type Estimated Duration],
	msdyn_defaultworkordertype	as [Incident Default Work Order Type Id]

FROM dbo.msdyn_incidenttype
WHERE
	not (((statecode != 0) and (DATEDIFF(YEAR, GETDATE(), ModifiedOn) < -1)))

GO

CREATE VIEW [fso].[MeasuresView]
	AS
  SELECT TOP 0 1 AS MeasureValues

GO
CREATE VIEW [fso].[ProductView]
AS 
SELECT 
	
	productid						as [Product Id],
	name							as [Service Name],
	productnumber					as [Service Number],
	msdyn_fieldserviceproducttype	as [Product Type Code],
	producttype.Label				as [Product Type],
	statecode						as [Product State Code],
	statecode.Label					as [Product State]

FROM dbo.product
	left outer join (
        select 
        [Option] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[GlobalOptionSetMetadata] 
        where [OptionsetName] = 'msdyn_fieldserviceproducttype'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) producttype on producttype.Value = msdyn_fieldserviceproducttype
	left outer join (
        select 
        [State] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[StateMetadata]
        where [EntityName] = 'product'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) statecode on statecode.Value = statecode
WHERE 
(not msdyn_fieldserviceproducttype is null)
and not (((statecode != 0) and (DATEDIFF(YEAR, GETDATE(), ModifiedOn) < -1)))
AND msdyn_fieldserviceproducttype <> 690970002

GO



CREATE VIEW [fso].[ServiceView]
AS 
SELECT 
	
	productid						as [Product Id],
	name							as [Service Name],
	productnumber					as [Service Number],
	msdyn_fieldserviceproducttype	as [Product Type Code],
	producttype.Label				as [Product Type],
	statecode						as [Product State Code],
	statecode.Label					as [Product State]

FROM dbo.product
	left outer join (
        select 
        [Option] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[GlobalOptionSetMetadata] 
        where [OptionsetName] = 'msdyn_fieldserviceproducttype'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) producttype on producttype.Value = msdyn_fieldserviceproducttype
	left outer join (
        select 
        [State] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[StateMetadata]
        where [EntityName] = 'product'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) statecode on statecode.Value = statecode
WHERE 
(not msdyn_fieldserviceproducttype is null)
and not (((statecode != 0) and (DATEDIFF(YEAR, GETDATE(), ModifiedOn) < -1)))
AND msdyn_fieldserviceproducttype = 690970002

GO

CREATE VIEW [fso].[ResourceView]
AS
SELECT       
	br.bookableresourceid						as [Resource Id], 
	br.name										as [Resource Name],
	resourceType.Label							as [Resource Type],
	br.resourcetype								as [Resource Type Code],
	statecode.Label								as [Resource State],
	br.statecode								as [Resource State Code],
	br.msdyn_hourlyrate							as [Resource Hourly Wage]
	

FROM            dbo.bookableresource br
	left outer join (
        select 
        [Option] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[OptionSetMetadata] 
        where [OptionsetName] = 'resourcetype'
        --and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
        and [EntityName] = 'bookableresource'
	) resourceType on resourceType.Value = br.resourcetype
	left outer join (
        select 
        [State] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[StateMetadata]
        where [EntityName] = 'bookableresource'
        --and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) statecode on statecode.Value = br.statecode
WHERE
  not (((br.statecode != 0) and (DATEDIFF(YEAR, GETDATE(), br.ModifiedOn) < -1)))
GO


CREATE VIEW [fso].[TerritoryView]
	AS 
SELECT 
	name						as [Service Territory],
	territoryid					as [Territory Id]
	
FROM dbo.territory
GO

CREATE VIEW [fso].[WorkOrderProductsView]
	AS SELECT
	msdyn_product					as [Work Order Product Item Id],
	linestatus.Label				as [Work Order Product Line Status],
	msdyn_linestatus				as [Work Order Product Line Status Code],
	msdyn_totalamount_base			as [Work Order Product Revenue],
	msdyn_totalcost_base			as [Work Order Total Product Cost],
	msdyn_quantity					as [Work Order Product Quantity],
	msdyn_workorder					as [Work Order Id],
	msdyn_customerasset				as [Work Order Product Customer Asset Id]
	 
	FROM dbo.msdyn_workorderproduct
	left outer join (
        select 
        [Option] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[GlobalOptionSetMetadata] 
        where [OptionsetName] = 'msdyn_linestatus'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) linestatus on linestatus.Value = msdyn_linestatus
	WHERE
	(DATEDIFF(YEAR, GETDATE(), createdon) >= -1)
AND (DATEDIFF(DAY, GETDATE(), createdon) <= 366)
GO

CREATE VIEW [fso].[WorkOrderServiceView]
	AS SELECT 
	msdyn_estimateunitcost_base							as [Work Order Service Estimated Unit Cost],
	msdyn_totalcost_base								as [Work Order Service Total Cost],
	msdyn_workorderincident								as [Work Order Service Work Order Incident Id],
	msdyn_service										as [Work Order Service Product Id],
	msdyn_durationtobill								as [Work Order Service Duration to Bill],
	msdyn_duration										as [Work Order Service Duration],
	msdyn_estimateduration								as [Work Order Service Estimated Duration],
	msdyn_linestatus									as [Work Order Service Line Status Code],
	linestatus.Label									as [Work Order Service Line Status],
	msdyn_lineorder										as [Work Order Service Line Order],
	msdyn_workorder										as [Work Order Id],
	msdyn_booking										as [Work Order Service Resource Booking Id],
	msdyn_totalamount_base								as [Work Order Service Revenue]
		
	FROM dbo.msdyn_workorderservice
	left outer join (
        select 
        [Option] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[GlobalOptionSetMetadata] 
        where [OptionsetName] = 'msdyn_linestatus'
        and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) linestatus on linestatus.Value = msdyn_linestatus
	WHERE
	(DATEDIFF(YEAR, GETDATE(), createdon) >= -1)
AND (DATEDIFF(DAY, GETDATE(), createdon) <= 366)

GO

CREATE VIEW [fso].[WorkOrderTypeView]
	AS 
SELECT 
	msdyn_workordertypeId		as [Work Order Type Id],
	msdyn_name					as [Work Order Type],
	CASE(msdyn_taxable) 				
		WHEN 0 THEN 'No'
		WHEN 1 THEN 'Yes'
		ELSE NULL
	END							as [Is Taxable]

FROM dbo.msdyn_workordertype
WHERE
	not (((statecode != 0) and (DATEDIFF(YEAR, GETDATE(), ModifiedOn) < -1)))

GO

CREATE VIEW [fso].[WorkOrderView]
	AS 
	SELECT
		msdyn_systemstatus						as [Work Order System Status Code],
		systemstatus.Label						as [Work Order System Status],
		msdyn_serviceaccount					as [Customer Id],
		msdyn_city								as [Work Order City],
		msdyn_stateorprovince					as [Work Order State or Province],
		msdyn_postalcode						as [Work Order Postal Code],
		msdyn_country							as [Work Order Country],
		msdyn_primaryincidentdescription		as [Work Order Incident Description],
		CONVERT(date,createdon)					as [Work Order Date Created On],
		msdyn_workorderid						as [Work Order Id],
		msdyn_name								as [Work Order Number],
		msdyn_parentworkorder					as [Parent Work Order Id],
		msdyn_address1							as [Work Order Address],
		msdyn_workordertype						as [Work Order Type Id],
		msdyn_primaryincidenttype				as [Incident Type Id],
		msdyn_primaryincidentestimatedduration	as [Work Order Estimated Duration],
		statuscode								as [Work Order Status Code],
		statuscode.Label						as [Work Order Status],
		msdyn_subtotalamount_base				as [Work Order Subtotal],
		msdyn_totalamount_base					as [Work Order Revenue],
		msdyn_latitude							as [Work Order Latitude],
		msdyn_longitude							as [Work Order Longitude],
		msdyn_serviceterritory					as [Service Territory Id],
		CONVERT(date,msdyn_timeclosed)			as [Work Order Date Closed]
	
	FROM dbo.msdyn_workorder
	left outer join (
        select 
        [Status] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[StatusMetadata]
        where [EntityName] = 'msdyn_workorder'
        --and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) statuscode on statuscode.Value = statuscode
	left outer join (
        select 
        [Option] as [Value], 
        [LocalizedLabel] as [Label] 
        from [dbo].[GlobalOptionSetMetadata] 
        where [OptionsetName] = 'msdyn_systemstatus'
        --and [isUserLocalizedLabel] = 1
        and [LocalizedLabelLanguageCode] = 1033
	) systemstatus on systemstatus.Value = msdyn_systemstatus
	WHERE
		(DATEDIFF(YEAR, GETDATE(), modifiedon) >= -1)
	AND (DATEDIFF(DAY, GETDATE(), modifiedon) <= 366)
	AND msdyn_systemstatus <> 690970005  -- remove canceled
GO

