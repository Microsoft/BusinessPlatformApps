SET NOCOUNT ON;

    SELECT DISTINCT
           Replace(Left(displayname0, 250), CHAR(9), ' ')  [Program Name],
           Replace(Left(publisher0, 250), CHAR(9), ' ')    Publisher,
           version0                                        [Version]          
	FROM  v_Add_Remove_Programs
	WHERE displayname0 IS NOT NULL AND
	      displayname0 <> '';

/* -- The query below returns duplicated programId.
SELECT prodid00      ProgramId,
       displayname00 [Program Name],
       publisher00   Publisher,
       version00     [Version]
FROM   dbo.add_remove_programs_data
UNION
SELECT prodid00      ProgramId,
       displayname00 [Program Name],
       publisher00   Publisher,
       version00     [Version]
FROM   dbo.add_remove_programs_64_data;
*/

/* -- This was the original query, but I think the above is sufficient
WITH distinctprogramids AS
(
    SELECT prodid00 ProgramId
    FROM   dbo.add_remove_programs_data
    UNION
    SELECT prodid00 ProgramId
    FROM   dbo.add_remove_programs_64_data
),
programswithdupes AS
(
    SELECT 
    prodid00      ProgramId,
    displayname00 [Program Name],
    publisher00   Publisher,
    version00     [Version]
    FROM   dbo.add_remove_programs_data
    UNION
    SELECT
    prodid00      ProgramId,
    displayname00 [Program Name],
    publisher00   Publisher,
    version00     [Version]
    FROM   dbo.add_remove_programs_64_data
)
SELECT dp.programid,
       (SELECT TOP 1 [publisher] FROM programswithdupes pwd WHERE pwd.programid = dp.programid) [Publisher],
       (SELECT TOP 1 [program name] FROM   programswithdupes pwd WHERE  pwd.programid = dp.programid) [Program Name],
       (SELECT TOP 1 [version] FROM programswithdupes pwd WHERE pwd.programid = dp.programid) [Version]
FROM   distinctprogramids dp
       LEFT OUTER JOIN (SELECT TOP 1 pwd.programid,
                                     [publisher],
                                     [program name],
                                     [version]
                        FROM   programswithdupes pwd
                        WHERE  pwd.programid = programid) dp2
                    ON dp.programid = dp2.programid;
*/