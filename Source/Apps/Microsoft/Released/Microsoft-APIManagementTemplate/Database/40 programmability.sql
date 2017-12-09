SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE pbist_apimgmt.sp_edgetablesswap
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    EXEC sp_rename @objname='pbist_apimgmt.callextendededgelist', @newname='callextendededgelist_temp';
    EXEC sp_rename @objname='pbist_apimgmt.callextendededgelist_staging', @newname='callextendededgelist';
    EXEC sp_rename @objname='pbist_apimgmt.callextendededgelist_temp', @newname='callextendededgelist_staging';
    --DROP TABLE pbist_apimgmt.callextendededgelist_temp;
    TRUNCATE TABLE pbist_apimgmt.callextendededgelist_staging;

    EXEC sp_rename @objname='pbist_apimgmt.callprobabilityedgelist', @newname='callprobabilityedgelist_temp';
    EXEC sp_rename @objname='pbist_apimgmt.callprobabilityedgelist_staging', @newname='callprobabilityedgelist';
    EXEC sp_rename @objname='pbist_apimgmt.callprobabilityedgelist_temp', @newname='callprobabilityedgelist_staging';
    --DROP TABLE pbist_apimgmt.callprobabilityedgelist_temp;
    TRUNCATE TABLE pbist_apimgmt.callprobabilityedgelist_staging;
END;
go

CREATE PROCEDURE pbist_apimgmt.sp_ffttableswap
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    EXEC sp_rename @objname='pbist_apimgmt.fft', @newname='fft_temp';
    EXEC sp_rename @objname='pbist_apimgmt.fft_staging', @newname='fft';
    EXEC sp_rename @objname='pbist_apimgmt.fft_temp', @newname='fft_staging';
    --DROP TABLE pbist_apimgmt.fft_temp;
    TRUNCATE TABLE pbist_apimgmt.fft_staging;
END;
go

CREATE PROCEDURE pbist_apimgmt.sp_getdistinctipaddressesinwindow
(
    -- Add the parameters for the stored procedure here
    @Start varchar(255) = NULL,
    @End varchar(255) = NULL,
    @MaxRows int = 100
)
AS
BEGIN
    DECLARE @StartDate datetime
    DECLARE @EndDate datetime
    DECLARE @CURRENT_TIME datetime
    SET @CURRENT_TIME = getdate()

    SET @Startdate = coalesce(try_convert(datetime, @Start, 127), dateadd(day, -7, @CURRENT_TIME))
    SET @EndDate = coalesce(try_convert(datetime, @End, 127), @CURRENT_TIME)

    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT TOP (@MaxRows) IPAddress
    FROM 
        pbist_apimgmt.request
    WHERE
        IPAddress IS NOT NULL
        AND RequestId IS NOT NULL
        AND Product IS NOT NULL
        AND Operation IS NOT NULL
        AND Api IS NOT NULL
        AND CreatedDate >= @StartDate
        AND CreatedDate < @EndDate
        GROUP BY IPAddress
    ORDER BY COUNT(IPAddress) DESC;
END;
go

CREATE PROCEDURE pbist_apimgmt.sp_getrequestsbyipaddressinwindow
(
    -- Add the parameters for the stored procedure here
    @IpAddress varchar(20) = NULL,
    @Start varchar(255) = NULL,
    @End varchar(255) = NULL
)
AS
BEGIN
    DECLARE @StartDate datetime
    DECLARE @EndDate datetime
    DECLARE @CURRENT_TIME datetime
    SET @CURRENT_TIME = getdate()

    SET @Startdate = coalesce(try_convert(datetime, @Start, 127), dateadd(day, -7, @CURRENT_TIME))
    SET @EndDate = coalesce(try_convert(datetime, @End, 127), @CURRENT_TIME)

    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT TOP 100000
        RequestId,
        ProductId As Product,
		OperationId As Operation,
		ApiId As Api,
        CreatedDate,
        IPAddress 
    FROM 
        pbist_apimgmt.request
    WHERE
        IPAddress IS NOT NULL
        AND RequestId IS NOT NULL
        AND Product IS NOT NULL
        AND Operation IS NOT NULL
        AND Api IS NOT NULL
        AND IPAddress = @IpAddress
        AND CreatedDate >= @StartDate
        AND CreatedDate < @EndDate
    ORDER BY CreatedDate;
END;
go

CREATE PROCEDURE pbist_apimgmt.sp_fftdataextraction
AS
BEGIN
    SET NOCOUNT ON;
	SELECT RequestId, CreatedDate, IPAddress
	FROM pbist_apimgmt.request
	WHERE IPAddress IS NOT NULL	AND CreatedDate > DATEADD(day, -3, SYSDATETIME())
END;
go

CREATE FUNCTION pbist_apimgmt.fn_IPtoBigInt
(
    @Ipaddress NVARCHAR(15) -- should be in the form '123.123.123.123'
)
RETURNS BIGINT
AS
BEGIN
 DECLARE @part1 AS NVARCHAR(3) 
 DECLARE @part2 AS NVARCHAR(3) 
 DECLARE @part3 AS NVARCHAR(3)
 DECLARE @part4 AS NVARCHAR(3)

 SELECT @part1 = LEFT(@Ipaddress, CHARINDEX('.',@Ipaddress) - 1)
 SELECT @Ipaddress = SUBSTRING(@Ipaddress, LEN(@part1) + 2, 15)
 SELECT @part2 = LEFT(@Ipaddress, CHARINDEX('.',@Ipaddress) - 1)
 SELECT @Ipaddress = SUBSTRING(@Ipaddress, LEN(@part2) + 2, 15)
 SELECT @part3 = LEFT(@Ipaddress, CHARINDEX('.',@Ipaddress) - 1)
 SELECT @part4 = SUBSTRING(@Ipaddress, LEN(@part3) + 2, 15)

 DECLARE @ipAsBigInt AS BIGINT
 SELECT @ipAsBigInt =
    (16777216 * (CAST(@part1 AS BIGINT)))
    + (65536 * (CAST(@part2 AS BIGINT)))
    + (256 * (CAST(@part3 AS BIGINT)))
    + (CAST(@part4 AS BIGINT))

 RETURN @ipAsBigInt

END
go

CREATE FUNCTION pbist_apimgmt.fn_IsIpaddressInSubnetShortHand
(
    @network NVARCHAR(18), -- 'eg: '192.168.0.0/24'
    @testAddress NVARCHAR(15) -- 'eg: '192.168.0.1'
)
RETURNS BIT AS
BEGIN
    DECLARE @networkAddress NVARCHAR(15)
    DECLARE @subnetBits TINYINT

    SELECT @networkAddress = LEFT(@network, CHARINDEX('/', @network) - 1)
    SELECT @subnetBits = CAST(SUBSTRING(@network, LEN(@networkAddress) + 2, 2) AS TINYINT)

    RETURN CASE WHEN (pbist_apimgmt.fn_IPtoBigInt(@networkAddress) & pbist_apimgmt.fn_SubnetBitstoBigInt(@subnetBits)) 
        = (pbist_apimgmt.fn_IPtoBigInt(@testAddress) & pbist_apimgmt.fn_SubnetBitstoBigInt(@subnetBits)) 
    THEN 1 ELSE 0 END
END;
go

CREATE FUNCTION pbist_apimgmt.fn_IsIpaddressInSubnet
(
    @networkAddress NVARCHAR(15), -- 'eg: '192.168.0.0'
    @subnetMask NVARCHAR(15), -- 'eg: '255.255.255.0' for '/24'
    @testAddress NVARCHAR(15) -- 'eg: '192.168.0.1'
)
RETURNS BIT AS
BEGIN
    RETURN CASE WHEN (pbist_apimgmt.fn_IPtoBigInt(@networkAddress) & pbist_apimgmt.fn_IPtoBigInt(@subnetMask)) 
        = (pbist_apimgmt.fn_IPtoBigInt(@testAddress) & pbist_apimgmt.fn_IPtoBigInt(@subnetMask)) 
    THEN 1 ELSE 0 END
END;
go

CREATE FUNCTION pbist_apimgmt.fn_SubnetBitstoBigInt
(
    @SubnetBits TINYINT -- max = 32
)
RETURNS BIGINT
AS
BEGIN

 DECLARE @multiplier AS BIGINT = 2147483648
 DECLARE @ipAsBigInt AS BIGINT = 0
 DECLARE @bitIndex TINYINT = 1
 WHILE @bitIndex <= @SubnetBits
 BEGIN
    SELECT @ipAsBigInt = @ipAsBigInt + @multiplier
    SELECT @multiplier = @multiplier / 2
    SELECT @bitIndex = @bitIndex + 1
 END

 RETURN @ipAsBigInt

END;
go

CREATE PROCEDURE pbist_apimgmt.sp_ProcessIPAddressLocations
AS

BEGIN

-- Table var to store our empty row ids
	DECLARE @MissingGeo TABLE
	(
		IPAddress varchar(20)
	)

	DECLARE @CurrentIPAddress varchar(20)

	-- Select our blank IPs
    INSERT INTO @MissingGeo
	SELECT DISTINCT IPAddress
	FROM pbist_apimgmt.Request
	WHERE IPAddress IS NOT NULL AND ([Latitude] IS NULL OR [Longitude] IS NULL)

	-- Loop through each, getting a lat long, and writing back to the Request table
	DECLARE latlong_cursor CURSOR
		FOR SELECT IPAddress
		FROM @MissingGeo
	
	OPEN latlong_cursor 
	FETCH NEXT FROM latlong_cursor
	INTO @CurrentIPAddress

	WHILE @@FETCH_STATUS = 0
	BEGIN
			UPDATE pbist_apimgmt.Request
			SET Latitude = t.Latitude,
				longitude = t.longitude
			FROM (
				SELECT network, latitude, longitude FROM pbist_apimgmt.[GeoLite2-City-Blocks-IPv4] t
				WHERE [IPpart] = LEFT(@CurrentIPAddress, CHARINDEX('.',@CurrentIPAddress)-1)
				AND pbist_apimgmt.fn_IsIpaddressInSubnetShortHand(t.network, @CurrentIPAddress) = 1) AS t
			WHERE pbist_apimgmt.request.IPAddress = @CurrentIPAddress AND (pbist_apimgmt.request.[Latitude] IS NULL OR pbist_apimgmt.request.[Longitude] IS NULL)

		FETCH NEXT FROM latlong_cursor
		INTO @CurrentIPAddress
	END
	CLOSE latlong_cursor
	DEALLOCATE latlong_cursor

END;
go


