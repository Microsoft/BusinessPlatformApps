SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/* SMGT specific schemas */

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
   [month]                TINYINT NOT NULL,
   month_name             CHAR(9) NOT NULL,
   month_abbrev           CHAR(3) NOT NULL,
   [quarter]              TINYINT NOT NULL,
   [year]                 SMALLINT NOT NULL,
   yearmo                 INT NOT NULL,
   same_day_year_ago_date DATE NOT NULL
   CONSTRAINT pk_dim_date PRIMARY KEY CLUSTERED (date_key)
);

CREATE TABLE smgt.usermapping
(
    userid     VARCHAR(50) NULL,
    domainuser VARCHAR(50) NULL
);