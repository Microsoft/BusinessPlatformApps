SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ANSI_NULL_DFLT_ON ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
go

CREATE TABLE pbist_apimgmt.[configuration]
(
  id                     INT IDENTITY(1, 1) NOT NULL,
  configuration_group    VARCHAR(150) NOT NULL,
  configuration_subgroup VARCHAR(150) NOT NULL,
  name                   VARCHAR(150) NOT NULL,
  [value]                VARCHAR(max) NULL,
  visible                BIT NOT NULL DEFAULT 0
);

CREATE TABLE pbist_apimgmt.request
(
    CreatedDate      DATETIME NULL,
    ServiceName      VARCHAR (200) NULL,
    RequestId        VARCHAR (50) NULL,
    IPAddress        VARCHAR (20) NULL,
    Operation        VARCHAR (200) NULL,
    OperationID      VARCHAR (200) NULL,
    Api              VARCHAR (200) NULL,
    ApiID            VARCHAR (200) NULL,
    Product          VARCHAR (200) NULL,
    ProductID        VARCHAR (200) NULL,
    SubscriptionName VARCHAR (200) NULL,
    SubscriptionId   VARCHAR (200) NULL,
    Length           INT NULL,
    Latitude         DECIMAL(9, 6) NULL,
    Longitude        DECIMAL(9, 6) NULL,
    City             VARCHAR(30) NULL
);
CREATE NONCLUSTERED INDEX IX_Request_RequestId_1 ON pbist_apimgmt.request ( RequestId ASC );
CREATE NONCLUSTERED INDEX IX_Request_ApiID_1 ON pbist_apimgmt.request ( ApiID );
CREATE NONCLUSTERED INDEX IX_Request_OperationID_1 ON pbist_apimgmt.request ( OperationID );
CREATE NONCLUSTERED INDEX IX_Request_ProductID_1 ON pbist_apimgmt.request ( ProductID );
CREATE NONCLUSTERED INDEX IX_Request_SubscriptionId_1 ON pbist_apimgmt.request ( SubscriptionId );
CREATE NONCLUSTERED INDEX IX_Request_IPAddress ON pbist_apimgmt.request ( IPAddress ASC );
CREATE NONCLUSTERED INDEX IX_Request_Latitude ON pbist_apimgmt.request ( Latitude ASC );
CREATE NONCLUSTERED INDEX IX_Request_Longitude ON pbist_apimgmt.request ( Longitude ASC );
CREATE NONCLUSTERED INDEX nci_wi_Request_IPAddressApi ON pbist_apimgmt.Request (IPAddress, Api) INCLUDE (CreatedDate, Latitude, Longitude, Operation, Product, RequestId);


CREATE TABLE pbist_apimgmt.response
(
    CreatedDate  DATETIME NULL,
    ServiceName  VARCHAR (200) NULL,
    RequestId    VARCHAR (50) NULL,
    StatusCode   INT NULL,
    StatusReason VARCHAR(200) NULL,
    [Length]     INT NULL
);
CREATE NONCLUSTERED INDEX IX_Response_RequestId_1 ON pbist_apimgmt.response ( RequestId ASC );


CREATE TABLE pbist_apimgmt.error
(
    CreatedDate DATETIME NULL,
    ServiceName VARCHAR (200) NULL,
    RequestId   VARCHAR (50) NULL,
    Source      VARCHAR (200) NULL,
    Reason      VARCHAR (200) NULL,
    Message     VARCHAR(200) NULL
);
CREATE NONCLUSTERED INDEX IX_Error_RequestId_1 ON pbist_apimgmt.error ( RequestId ASC );


CREATE TABLE pbist_apimgmt.[date]
(
    date_key               INT NOT NULL,
    full_date              DATE NOT NULL,
    day_of_week            TINYINT NOT NULL,
    day_num_in_month       TINYINT NOT NULL,
    day_name               NVARCHAR(50) NOT NULL,
    day_abbrev             NVARCHAR(10) NOT NULL,
    weekday_flag           CHAR(1) NOT NULL,
    week_num_in_year       TINYINT NOT NULL,
    week_begin_date        DATE NOT NULL,
    month                  TINYINT NOT NULL,
    month_name             NVARCHAR(50) NOT NULL,
    month_abbrev           NVARCHAR(10) NOT NULL,
    quarter                TINYINT NOT NULL,
    year                   SMALLINT NOT NULL,
    yearmo                 INT NOT NULL,
    same_day_year_ago_date DATE NOT NULL,
    CONSTRAINT pk_dim_date PRIMARY KEY CLUSTERED ( date_key ASC )
);


CREATE TABLE pbist_apimgmt.[geolite2-city-blocks-ipv4]
(
    network                        CHAR(20) NOT NULL,
    geoname_id                     INT NULL,
    registered_country_geoname_id  INT NULL,
    represented_country_geoname_id INT NULL,
    is_anonymous_proxy             TINYINT NULL,
    is_satellite_provider          TINYINT NULL,
    postal_code                    CHAR(10) NULL,
    latitude                       NUMERIC(8,4) NULL,
    longitude                      NUMERIC(8,4) NULL,
    accuracy_radius                INT NULL,
    IPpart                         CHAR(3) NULL
);
CREATE NONCLUSTERED INDEX [IX_GeoLite2-City-Blocks-IPv4] ON pbist_apimgmt.[geolite2-city-blocks-ipv4] ( network ASC );
CREATE NONCLUSTERED INDEX [IX_GeoLite2-City-Blocks-IPv4_IPPart] ON pbist_apimgmt.[geolite2-city-blocks-ipv4] ( IPPart ASC ) INCLUDE ( latitude, longitude, network);


CREATE TABLE pbist_apimgmt.callextendededgelist
(
    RequestId          VARCHAR(50) NOT NULL,
    Product            VARCHAR(200) NOT NULL,
    Api                VARCHAR(200) NOT NULL,
    Operation          VARCHAR(200) NOT NULL,
    CreatedDate        DATETIME NOT NULL,
    RelatedRequestId   VARCHAR(50) NOT NULL,
    RelatedProduct     VARCHAR(200) NOT NULL,
    RelatedApi         VARCHAR(200) NOT NULL,
    RelatedOperation   VARCHAR(200) NOT NULL,
    RelatedCreatedDate DATETIME NOT NULL,
    IPAddress          VARCHAR(20) NOT NULL
);


CREATE TABLE pbist_apimgmt.callextendededgelist_staging
(
    RequestId          VARCHAR(50) NOT NULL,
    Product            VARCHAR(200) NOT NULL,
    Api                VARCHAR(200) NOT NULL,
    Operation          VARCHAR(200) NOT NULL,
    CreatedDate        DATETIME NOT NULL,
    RelatedRequestId   VARCHAR(50) NOT NULL,
    RelatedProduct     VARCHAR(200) NOT NULL,
    RelatedApi         VARCHAR(200) NOT NULL,
    RelatedOperation   VARCHAR(200) NOT NULL,
    RelatedCreatedDate DATETIME NOT NULL,
    IPAddress          VARCHAR(20) NOT NULL
);


CREATE TABLE pbist_apimgmt.callprobabilityedgelist
(
    Product                VARCHAR(200) NOT NULL,
    Api                    VARCHAR(200) NOT NULL,
    Operation              VARCHAR(200) NOT NULL,
    RelatedProduct         VARCHAR(200) NOT NULL,
    RelatedApi             VARCHAR(200) NOT NULL,
    RelatedOperation       VARCHAR(200) NOT NULL,
    IPAddress              VARCHAR(20) NOT NULL,
    CallRelationshipCount  INT NOT NULL,
    StartingCallTotalCount INT NOT NULL
);


CREATE TABLE pbist_apimgmt.callprobabilityedgelist_staging
(
    Product                VARCHAR(200) NOT NULL,
    Api                    VARCHAR(200) NOT NULL,
    Operation              VARCHAR(200) NOT NULL,
    RelatedProduct         VARCHAR(200) NOT NULL,
    RelatedApi             VARCHAR(200) NOT NULL,
    RelatedOperation       VARCHAR(200) NOT NULL,
    IPAddress              VARCHAR(20) NOT NULL,
    CallRelationshipCount  INT NOT NULL,
    StartingCallTotalCount INT NOT NULL
);


CREATE TABLE pbist_apimgmt.fft
(
    IPAddress VARCHAR(20) NOT NULL,
    TimeUnit  VARCHAR(50) NOT NULL,
    CallFreq  DECIMAL(18, 0) NOT NULL,
    Position  INT NOT NULL
);


CREATE TABLE pbist_apimgmt.fft_staging
(
    IPAddress VARCHAR(20) NOT NULL,
    TimeUnit  VARCHAR(50) NOT NULL,
    CallFreq  DECIMAL(18, 0) NOT NULL,
    Position  INT NOT NULL
);
go
