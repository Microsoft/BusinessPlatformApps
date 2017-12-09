SET NOCOUNT ON;

SELECT Count(*)
FROM
    ( SELECT displayname0, publisher0, version0
      FROM dbo.v_Add_Remove_Programs
      WHERE displayname0 IS NOT NULL AND displayname0 <> ''
      UNION
      SELECT displayname0, publisher0, version0
      FROM dbo.v_gs_add_remove_programs_64
      WHERE displayname0 IS NOT NULL AND displayname0 <> ''
    ) T;