SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE VIEW pbist_apimgmt.vw_apimerrordetail AS
SELECT a.[RequestId],
       a.[CreatedDate] AS CreatedDateTime,
       a.[ServiceName],
       a.[Source],
       a.[Reason],
      a.[Message],
       b.Operation,
       b.OperationId,
       b.Api,
       b.ApiId,
       b.Product,
       b.ProductId,
       b.SubscriptionId,
       b.SubscriptionName,
       b.IPAddress,
       CAST(a.CreatedDate as date) AS CreatedDate  
FROM pbist_apimgmt.[error] a LEFT OUTER JOIN pbist_apimgmt.request b ON A.RequestId = B.RequestId;
go


CREATE VIEW pbist_apimgmt.[vw_date]
AS
    SELECT date_key,
           full_date        AS [date],
           day_of_week      AS [day of the week],
           day_num_in_month AS [day number of the month],
           day_name         AS [day name],
           day_abbrev       AS [day abbreviated],
           weekday_flag     AS [weekday flag],
           [month],
           month_name       AS [month name],
           month_abbrev,
           [quarter],
           [year],
           same_day_year_ago_date,
           week_begin_date  AS [week begin date]
    FROM   pbist_apimgmt.date;
go

CREATE VIEW pbist_apimgmt.vw_apisummary
AS
    SELECT DISTINCT ApiID, LAST_VALUE(Api) OVER (PARTITION BY ApiID ORDER BY ApiID ASC ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING) AS Api
    FROM pbist_apimgmt.request;
go

CREATE VIEW pbist_apimgmt.vw_operationsummary
AS
    SELECT DISTINCT OperationID, LAST_VALUE(Operation) OVER (PARTITION BY OperationID ORDER BY OperationID ASC ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING) AS Operation
    FROM pbist_apimgmt.request;
go

CREATE VIEW pbist_apimgmt.vw_productsummary
AS
    SELECT DISTINCT ProductID, LAST_VALUE(Product) OVER (PARTITION BY ProductID ORDER BY ProductID ASC ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING) AS Product
    FROM pbist_apimgmt.request;
go

CREATE VIEW pbist_apimgmt.vw_subscriptionsummary
AS
    SELECT DISTINCT SubscriptionID, LAST_VALUE(SubscriptionName) OVER (PARTITION BY SubscriptionID ORDER BY SubscriptionID ASC ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING) AS Subscription
    FROM pbist_apimgmt.request;
go

CREATE VIEW pbist_apimgmt.vw_allrequestdata AS
SELECT a.RequestId,
		a.CreatedDate AS CreatedDateTime,
		DATEDIFF(ms, a.CreatedDate, b.CreatedDate) AS ExecutionTime,
		a.Operation,
		a.OperationId,
		a.Api,
		a.ApiId,
		a.Product,
		a.ProductId,
		a.SubscriptionId,
		a.SubscriptionName,
		a.IPAddress,
		a.Latitude,
		a.Longitude,
		a.City,
		b.StatusCode,
		b.StatusReason,
		a.Length AS RequestLength,
		b.Length AS ResponseLength,
		IsError = 
		CASE   
			WHEN StatusCode >= 200 AND StatusCode < 300 THEN 0
			ELSE 1
		END,
		CAST(a.CreatedDate as date) AS CreatedDate   
FROM pbist_apimgmt.request a INNER JOIN pbist_apimgmt.response b ON a.RequestId = b.RequestId
WHERE a.CreatedDate > DATEADD(day, -90, SYSDATETIME())
go

CREATE VIEW pbist_apimgmt.vw_visualoperationcallvolume AS
SELECT 
    DATEADD(hour, DATEDIFF(hour, 0, CreatedDate), 0) as datetime_hour, ProductID, ApiID, OperationId, count(*) as operation_counts
FROM pbist_apimgmt.request
WHERE CreatedDate > DATEADD(day, -7, SYSDATETIME())
GROUP BY DATEADD(hour, DATEDIFF(hour, 0, CreatedDate), 0), ProductID, ApiID, OperationId;
go

CREATE VIEW pbist_apimgmt.vw_visualcallprobabilityedgelist AS
SELECT
        el.Product AS ProductID, el.Api AS ApiID, el.Operation AS OperationID, el.RelatedProduct AS RelatedProductID,el.RelatedApi AS RelatedApiID, el.RelatedOperation AS RelatedOperationID,
        Count(distinct el.IPAddress) as users,
        Cast(100.0 * SUM(CallRelationshipCount) / t2.SourceCount as INT) as CallProbability
From 
    pbist_apimgmt.callprobabilityedgelist el INNER JOIN ( SELECT t1.Product, t1.Api, t1.Operation, SUM(t1.StartingCallTotalCount) as SourceCount
                                                            FROM ( SELECT DISTINCT Product, Api, Operation, IPAddress, StartingCallTotalCount
                                                                    FROM pbist_apimgmt.callprobabilityedgelist
                                                                ) as t1
                                                            GROUP BY  t1.Product, t1.Api, t1.Operation) t2 
    ON el.Product = t2.Product AND el.Api  = t2.Api AND el.Operation = t2.Operation
GROUP BY
    el.Product, el.Api, el.Operation, el.RelatedProduct, el.RelatedApi, el.RelatedOperation, t2.SourceCount;
go

CREATE VIEW pbist_apimgmt.vw_visualipedgecounts
AS
    SELECT
        Product AS ProductID,
        Api AS ApiID,
        Operation AS OperationID,
        RelatedProduct AS RelatedProductID,
        RelatedApi AS RelatedApiID,
        RelatedOperation AS RelatedOperationID,
        DATEADD(hour, DATEDIFF(hour, 0, CreatedDate), 0) as datetime_hour_bin,
        IPAddress,
        Count(*) as counts
    FROM pbist_apimgmt.callextendededgelist
    GROUP BY 
        Product, Api, Operation, RelatedProduct, RelatedApi, RelatedOperation, IPAddress, DATEADD(hour, DATEDIFF(hour, 0, CreatedDate), 0);
go

CREATE VIEW pbist_apimgmt.vw_visualfreqhour
AS
    Select
        IPAddress
    ,    Position as 'Hour'
    ,    CallFreq as SignalStrength
    FROM pbist_apimgmt.FFT
    Where TimeUnit = 'h';
go

CREATE VIEW pbist_apimgmt.vw_visualfreqminute
AS
    Select
        IPAddress,
        Position as 'Minute',
        CallFreq as SignalStrength
    FROM pbist_apimgmt.FFT
    WHERE TimeUnit = 'min';
go

CREATE VIEW pbist_apimgmt.vw_visualfreqsecond
AS
    SELECT
        IPAddress,
        Position as 'Second',
        CallFreq as SignalStrength
    FROM pbist_apimgmt.FFT
    WHERE TimeUnit = 's';
go

CREATE VIEW pbist_apimgmt.vw_ipaddresssummary
AS
    SELECT DISTINCT IPAddress FROM pbist_apimgmt.request;
go

CREATE VIEW pbist_apimgmt.vw_subscriptionipaddress
AS
    SELECT DISTINCT SubscriptionId, IPAddress FROM pbist_apimgmt.request;
go
