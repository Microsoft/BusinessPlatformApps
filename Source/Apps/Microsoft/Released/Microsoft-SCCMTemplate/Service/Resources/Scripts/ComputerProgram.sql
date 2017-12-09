SET NOCOUNT ON;

SELECT resourceid                             MachineID,
       Replace(p.displayname0, CHAR(9), ' ')  [Program Name],
       Replace(p.publisher0, CHAR(9), ' ')    Publisher,
       version0                               [Version],
       CAST(NULL as DATETIME)                 [timestamp]
FROM   dbo.v_gs_add_remove_programs p INNER JOIN dbo.vsms_r_system s ON p.resourceid = s.itemkey
WHERE  s.obsolete0 = 0 AND
       s.decommissioned0 = 0 AND
       p.displayname0 IS NOT NULL AND
       p.displayname0<>'' 
UNION
SELECT resourceid                             MachineID,
       Replace(p.displayname0, CHAR(9), ' ')  [Program Name],
       Replace(p.publisher0, CHAR(9), ' ')    Publisher,
       version0                               [Version],
       CAST(NULL as DATETIME)                 [timestamp]
FROM   dbo.v_gs_add_remove_programs_64 p INNER JOIN dbo.vsms_r_system s ON p.resourceid = s.itemkey
WHERE  s.obsolete0 = 0 AND
       s.decommissioned0 = 0 AND
       p.displayname0 IS NOT NULL AND
       p.displayname0<>'';
