SET NOCOUNT ON;

SELECT su.ci_id,
       su.articleid,
       su.bulletinid,
       title,
       severity,
       customname [Severity Name],
       infourl
FROM   v_updateinfo su LEFT OUTER JOIN customseverityreference s ON su.severity=s.severityid;