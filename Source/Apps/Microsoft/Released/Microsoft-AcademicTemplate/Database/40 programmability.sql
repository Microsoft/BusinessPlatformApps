SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE [ak].[Merge]
AS
BEGIN

---------------------Merge Journals-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.Journal as t
INNER JOIN ak.JournalStaging s 
ON s.JId = t.JId;

INSERT INTO ak.Journal
    SELECT DISTINCT *
    FROM ak.JournalStaging;
TRUNCATE TABLE ak.JournalStaging;
 COMMIT

 ---------------------Merge Conference-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.Conference as t
INNER JOIN ak.ConferenceStaging s 
ON s.CId = t.CId;

INSERT INTO ak.Conference
    SELECT DISTINCT *
    FROM ak.ConferenceStaging;
TRUNCATE TABLE ak.ConferenceStaging;
 COMMIT

 ---------------------Merge Authors-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.Author as t
INNER JOIN ak.AuthorStaging s 
ON s.AuId = t.AuId;

INSERT INTO ak.Author
    SELECT DISTINCT *
    FROM ak.AuthorStaging;

INSERT INTO ak.Author
SELECT
    dAuthor.AuId,
	dAuthor.AuthorName,
	dAuthor.AuthorDisplayName
FROM 
(
    SELECT 
        au.AuId, 
        AuthorName, 
        au.AuthorDisplayName,
        rn = ROW_NUMBER() OVER (
            PARTITION BY au.AuId 
            ORDER BY au.AuId)
    FROM ak.AuthorStaging AS au
) AS dAuthor
WHERE
    dAuthor.rn = 1;
TRUNCATE TABLE ak.AuthorStaging;
COMMIT

 ---------------------Merge Affiliation-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.Affiliation as t
INNER JOIN ak.AffiliationStaging s 
ON s.AfId = t.AfId;

INSERT INTO ak.Affiliation
    SELECT DISTINCT *
    FROM ak.AffiliationStaging;
TRUNCATE TABLE ak.AffiliationStaging;
 COMMIT

  ---------------------Merge FieldsOfStudy-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.FieldsOfStudy as t
INNER JOIN ak.FieldsOfStudyStaging s 
ON s.FId = t.FId;

INSERT INTO ak.FieldsOfStudy
    SELECT DISTINCT *
    FROM ak.FieldsOfStudyStaging;
TRUNCATE TABLE ak.FieldsOfStudyStaging;
 COMMIT

   ---------------------Merge PaperAuthorAffiliationRelationship-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.PaperAuthorAffiliationRelationship as t
INNER JOIN ak.PaperAuthorAffiliationRelationshipStaging s 
ON s.PId = t.PId AND s.AuId = t.AuId AND s.AfId = t.AfId;

INSERT INTO ak.PaperAuthorAffiliationRelationship
    SELECT DISTINCT *
    FROM ak.PaperAuthorAffiliationRelationshipStaging;
TRUNCATE TABLE ak.PaperAuthorAffiliationRelationshipStaging;
 COMMIT

    ---------------------Merge PaperFieldsOfStudyRelationship-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.PaperFieldsOfStudyRelationship as t
INNER JOIN ak.PaperFieldsOfStudyRelationshipStaging s 
ON s.PId = t.PId AND s.FId = t.FId;

INSERT INTO ak.PaperFieldsOfStudyRelationship
    SELECT DISTINCT *
    FROM ak.PaperFieldsOfStudyRelationshipStaging;
TRUNCATE TABLE ak.PaperFieldsOfStudyRelationshipStaging;
 COMMIT


     ---------------------Merge PaperCitationRelationshipStaging-------------------------------------
BEGIN TRAN

DELETE t 
FROM ak.PaperCitationRelationshipStaging as t
INNER JOIN ak.PaperCitationRelationshipStagingStaging s 
ON s.PId = t.PId AND s.RPId = t.RPId;

INSERT INTO ak.PaperCitationRelationshipStaging
    SELECT DISTINCT *
    FROM ak.PaperCitationRelationshipStagingStaging;

TRUNCATE TABLE ak.PaperCitationRelationshipStagingStaging;

 COMMIT



END
GO
