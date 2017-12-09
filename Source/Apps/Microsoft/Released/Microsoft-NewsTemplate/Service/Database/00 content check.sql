SELECT Count(*) AS ExistingObjectCount
FROM   INFORMATION_SCHEMA.TABLES
WHERE  ( table_schema = 'bpst_news' AND
            table_name IN ('configuration', 'date', 'documents', 'documentpublishedtimes', 'documentingestedtimes', 'documentkeyphrases', 'documentsentimentscores', 'documenttopics', 'documenttopicimages', 'entities', 'documentcompressedentities', 'topickeyphrases', 'documentsearchterms')
        );
