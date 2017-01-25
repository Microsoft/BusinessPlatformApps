SET NOCOUNT ON;

SELECT CollectionID,
       ResourceID
FROM   v_FullCollectionMembership_Valid
WHERE  CollectionID IN (SELECT SiteID                   
                        FROM   v_Collections
                        WHERE SiteID LIKE 'SMS%');



