SELECT Count(*) AS ExistingObjectCount
FROM   information_schema.tables
WHERE  ( table_schema = 'bpst_news' AND
            table_name IN ('configuration', 'date', 'documents', 'documentpublishedtimes', 'documentingestedtimes', 'documentkeyphrases', 'documentsentimentscores', 'documenttopics', 'documenttopicimages', 'entities', 'documentcompressedentities')
        );
