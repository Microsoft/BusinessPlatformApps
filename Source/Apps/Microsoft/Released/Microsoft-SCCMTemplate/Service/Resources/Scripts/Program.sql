SET NOCOUNT ON;

    SELECT DISTINCT
           Replace(Left(displayname0, 250), CHAR(9), ' ')  [Program Name],
           Replace(Left(publisher0, 250), CHAR(9), ' ')    Publisher,
           version0                                        [Version]          
	FROM  v_Add_Remove_Programs
	WHERE displayname0 IS NOT NULL AND
	      displayname0 <> '';