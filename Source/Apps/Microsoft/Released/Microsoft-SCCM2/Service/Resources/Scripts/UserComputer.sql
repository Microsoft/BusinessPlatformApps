SET NOCOUNT ON;

SELECT resourceid                       AS machineid,
       user_name0                       AS username,
       user_domain0 + '\' + user_name0  AS [full name]
FROM   v_r_system
WHERE  user_domain0 IS NOT NULL AND
       user_name0 IS NOT NULL AND
       decommissioned0=0 AND 
       obsolete0=0;