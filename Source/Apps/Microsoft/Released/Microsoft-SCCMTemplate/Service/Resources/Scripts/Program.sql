SET NOCOUNT ON;

SELECT Replace(displayname0, CHAR(9), ' ')  [Program Name],
       Replace(publisher0, CHAR(9), ' ')    Publisher,
       version0                             [Version]
FROM   dbo.v_gs_add_remove_programs
WHERE  displayname0 IS NOT NULL AND displayname0<>'' 
UNION
SELECT Replace(displayname0, CHAR(9), ' ')  [Program Name],
       Replace(publisher0, CHAR(9), ' ')    Publisher,
       version0                             [Version]
FROM   dbo.v_gs_add_remove_programs_64
WHERE  displayname0 IS NOT NULL AND displayname0<>'';