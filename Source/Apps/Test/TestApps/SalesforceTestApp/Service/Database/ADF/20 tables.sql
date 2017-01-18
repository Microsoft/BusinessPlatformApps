SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/* SMGT specific schemas */
CREATE TABLE smgt.actualsales
(
  invoiceid   VARCHAR(50) NULL,
  actualsales DECIMAL NULL,
  invoicedate DATE NULL,
  accountid   VARCHAR(50) NULL,
  productid   VARCHAR(50) NULL
);


CREATE TABLE smgt.configuration
(
  id                     INT IDENTITY(1, 1) NOT NULL,
  configuration_group    VARCHAR(150) NOT NULL,
  configuration_subgroup VARCHAR(150) NOT NULL,
  name                   VARCHAR(150) NOT NULL,
  value                  VARCHAR(max) NULL,
  visible                BIT NOT NULL DEFAULT 0
);


CREATE TABLE smgt.[date]
(
   date_key               INT NOT NULL,
   full_date              DATE NOT NULL,
   day_of_week            TINYINT NOT NULL,
   day_num_in_month       TINYINT NOT NULL,
   day_name               CHAR(9) NOT NULL,
   day_abbrev             CHAR(3) NOT NULL,
   weekday_flag           CHAR(1) NOT NULL,
   week_num_in_year       TINYINT NOT NULL,
   week_begin_date        DATE NOT NULL,
   week_begin_date_key    INT NOT NULL,
   [month]                TINYINT NOT NULL,
   month_name             CHAR(9) NOT NULL,
   month_abbrev           CHAR(3) NOT NULL,
   [quarter]              TINYINT NOT NULL,
   [year]                 SMALLINT NOT NULL,
   yearmo                 INT NOT NULL,
   fiscal_month           TINYINT NOT NULL,
   fiscal_quarter         TINYINT NOT NULL,
   fiscal_year            SMALLINT NOT NULL,
   last_day_in_month_flag CHAR(1) NOT NULL,
   same_day_year_ago_date DATE NOT NULL,
   same_day_year_ago_key  INT NOT NULL,
   day_num_in_year           AS Datepart(dayofyear, full_date),
   quarter_name              AS Concat('Q', [quarter]),
   fiscal_quarter_name       AS Concat('Q', fiscal_quarter),
   fiscalquartercompletename AS Concat('FY', Substring(CONVERT(VARCHAR, fiscal_year), 3, 2), ' Q', fiscal_quarter),
   fiscalyearcompletename    AS Concat('FY', Substring(CONVERT(VARCHAR, fiscal_year), 3, 2)),
   fiscalmonthcompletename   AS Concat(month_abbrev, ' ', Substring(CONVERT(VARCHAR, fiscal_year), 3, 2)),
   CONSTRAINT pk_dim_date PRIMARY KEY CLUSTERED (date_key)
);


CREATE TABLE smgt.quotas
(
  [amount]    DECIMAL(18, 0) NULL,
  [date]      DATE NULL,
  ownerid     VARCHAR(50) NULL,
  productid   VARCHAR(50) NULL
);


CREATE TABLE smgt.targets
(
  productid      VARCHAR(50) NULL,
  businessunitid VARCHAR(50) NULL,
  territoryid    VARCHAR(50) NULL,
  [target]       DECIMAL NULL,
  [date]         DATE NULL
);


CREATE TABLE smgt.usermapping
(
    userid     VARCHAR(50) NULL,
    domainuser VARCHAR(50) NULL
);