SET NOCOUNT ON;

SELECT DISTINCT user_name0              AS userName,
                unique_user_name0       AS [full name]       
FROM   v_r_user
WHERE  user_name0 IS NOT NULL AND 
       full_user_name0 NOT LIKE '%$%';