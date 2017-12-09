SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/* PBST specific schemas */

CREATE TABLE edfi.[configuration]
(
  id                     INT IDENTITY(1, 1) NOT NULL,
  configuration_group    VARCHAR(150) NOT NULL,
  configuration_subgroup VARCHAR(150) NOT NULL,
  [name]                 VARCHAR(150) NOT NULL,
  [value]                VARCHAR(max) NULL,
  visible                BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (id)
);
go
