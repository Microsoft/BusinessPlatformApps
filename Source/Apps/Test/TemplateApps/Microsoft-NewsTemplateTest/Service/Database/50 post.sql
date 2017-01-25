SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/************************************
* Tables to drop                    *
*************************************/
-- DROP TABLE ...;

/************************************
* Tables to truncate                *
*************************************/
-- TRUNCATE TABLE ...;

INSERT INTO [bpst_news].[documents]
           ([id]
           ,[text]
           ,[textLength]
           ,[cleanedText]
           ,[cleanedTextLength]
           ,[title]
           ,[sourceUrl]
           ,[sourceDomain]
         
		   )
     VALUES
           (00
           ,'test'
           ,4
           ,'test'
           ,4
           ,'test'
           ,'www.test.com'
           ,'test'
           
           )
GO
