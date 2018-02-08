SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Stored procedures

CREATE PROCEDURE [it].[SyncData]
AS

BEGIN TRY
	BEGIN TRANSACTION

	DECLARE  @UserName VARCHAR(50) = SUSER_SNAME()
			,@DateTimeNow DATETIME = GETDATE()
			,@ProcName VARCHAR(60) = OBJECT_NAME(@@PROCID)
			,@StgTableLoadMsg VARCHAR(200) = 'Staging table rows count.'
			,@CommnunityPageTitle VARCHAR(200)
			,@MessageDays VARCHAR(6)

	-- Merge Categories tables
	PRINT 'Merging categories data from STG_Categories to Categories table.'
	MERGE [it].[Categories] T
		USING (SELECT DISTINCT id, title, [hidden], MAX([messages]) [messages], MAX(topics) topics, MAX([views]) [views], MAX(depth) depth FROM [it].[STG_Categories] group by id, title, [hidden]) S
		ON S.Id = T.CategoryID
	WHEN NOT MATCHED THEN
		INSERT (CategoryID, CategoryTitle, IsHidden, CategoryMessages, CategoryTopics, CategoryViews, CategoryDepth, ModifiedBy, ModifiedDate)
		VALUES (S.id, S.title, CASE WHEN S.[hidden] = 'true' THEN 1 WHEN S.[hidden] = 'false' THEN 0 END, S.[messages], S.topics, S.views, s.depth,  @UserName, @DateTimeNow )
	WHEN MATCHED -- AND S.title <> T.CategoryTitle AND S.messages <> T.CategoryMessages AND S.topics <> T.CategoryTopics AND S.[views] <> T.CategoryViews AND S.depth <> T.CategoryDepth
		THEN
		UPDATE SET T.CategoryTitle = s.title, T.CategoryMessages = s.messages, T.CategoryTopics = s.topics, T.CategoryViews = s.views, T.CategoryDepth = s.depth, ModifiedBy = @UserName, ModifiedDate=@DateTimeNow ;	

	-- Merge Boards tables
	PRINT 'Merging Boards data from STG_Boards to Boards table.'
	MERGE [it].[Boards] T
		USING (SELECT DISTINCT id, conversation_style, title, parent_category, [hidden], MAX([messages]) [messages], MAX(topics) topics, MAX([views]) [views], MAX(depth) depth FROM [it].[STG_Boards] GROUP BY id, conversation_style, title, parent_category, [hidden] ) S
		ON S.Id = T.BoardID
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (BoardID, BoardTitle, ConversationStyle, ParentCategoryID, IsHidden, BoardMessages, BoardTopics, BoardViews, BoardDepth, ModifiedBy, ModifiedDate)
		VALUES (S.id, S.title, s.conversation_style, S.parent_category, CASE WHEN S.[hidden] = 'true' THEN 1 WHEN S.[hidden] = 'false' THEN 0 END, S.[messages], S.topics, S.views, s.depth,  @UserName, @DateTimeNow )
	WHEN MATCHED -- AND S.title <> T.BoardTitle AND S.messages <> T.BoardMessages AND s.topics <> T.BoardTopics AND S.views <> T.BoardViews
		THEN
		UPDATE SET T.BoardTitle = s.title, T.BoardMessages = s.messages, T.BoardTopics = s.topics, T.BoardViews = s.views, T.BoardDepth = s.depth, ModifiedBy = @UserName, ModifiedDate=@DateTimeNow ;	

		-- Merge messages tables
	PRINT 'Merging Messages data from STG_Messages to STG_Messages table.'
	MERGE [it].[Messages] T
		USING 
			(
				SELECT DISTINCT id, author, [subject], board, topic, parent, 
								CONVERT(date,post_time) post_time, depth, is_solution, solution_data, MAX(metrics) AS metrics, MAX(kudos) AS kudos, device_id
						FROM [it].[STG_Messages]					
					GROUP BY id, author, [subject], board, topic, parent, post_time, depth, is_solution, solution_data, device_id
			) S
		ON S.Id = T.MessageID
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (MessageID, UserID, MessageSubject, BoardID, Topic, ParentMessageID, PostedDate, MessageDepth, IsSolution, SolutionDate, MessageViews, MessageKudos, [DeviceType], ModifiedBy, ModifiedDate)
		VALUES (S.id, S.author, S.[subject], S.board, S.topic, S.parent, S.post_time, S.depth, CASE WHEN S.[is_solution] = 'true' THEN 1 WHEN S.[is_solution] = 'false' THEN 0 END, ISNULL(S.solution_data,NULL), S.[metrics], S.kudos
		,CASE WHEN (S.device_id LIKE '%firefox%' OR S.device_id LIKE '%edge%' OR S.device_id LIKE '%browser%' 
					OR S.device_id LIKE '%chrome%' OR S.device_id LIKE '%msie%' OR S.device_id LIKE '%opera%'
					OR S.device_id LIKE '%safari%' ) THEN 'Desktop'
			  WHEN (S.device_id LIKE '%apple_ipad%' OR S.device_id LIKE '%tablet%') THEN 'Tablet'
			  WHEN (S.device_id LIKE '%apple_iphone%' OR S.device_id LIKE '%aquaris%' OR S.device_id LIKE '%elepjhone%' 
					OR S.device_id LIKE '%android%' OR S.device_id LIKE '%pixel%' OR S.device_id LIKE '%huawei%' 
					OR S.device_id LIKE '%lg%' OR S.device_id LIKE '%lenovo%' OR S.device_id LIKE '%motorola%'
					OR S.device_id LIKE '%nokia%' OR S.device_id LIKE '%oneplus%' OR S.device_id LIKE '%oppo%'
					OR S.device_id LIKE '%samsung%' OR S.device_id LIKE '%sony%' OR S.device_id LIKE '%xiaomi%' ) THEN 'Mobile'
			  WHEN ISNULL(S.device_id, '') = '' THEN 'Unidentified'
			  ELSE 'NA' END		
		, @UserName, @DateTimeNow )
	WHEN MATCHED
		THEN 
		UPDATE SET T.MessageSubject = s.subject, T.UserID = s.author, T.BoardID = s.board, T.Topic = s.topic, T.ParentMessageID = s.parent, T.MessageDepth = s.depth, T.IsSolution = S.is_solution, T.MessageViews = S.[metrics], T.MessageKudos = S.kudos, ModifiedBy = @UserName, ModifiedDate=@DateTimeNow ;	

	-- Merge Kudos	
	PRINT 'Merging kudos data from STG_Kudos to Kudos table.'
	MERGE [it].[Kudos] T
		USING (
				SELECT DISTINCT KudoID, MessageID, CONVERT(date,KudoTime) KudoTime, KudoUserID, KudoWeight FROM [it].[STG_Kudos]
			) S
		ON S.KudoID = T.KudoID
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([KudoID], [MessageID], KudoDate, [KudoUserID], KudoWeight, [ModifiedBy], [ModifiedDate])
		VALUES (S.KudoID, S.MessageID, S.KudoTime, S.KudoUserID, S.KudoWeight, @UserName, @DateTimeNow );


	-- Merge Users
	PRINT 'Merging user data from STG_Users to Users table.'
	MERGE [it].[Users] T
		USING (	SELECT DISTINCT id, [login], deleted, CONVERT(date,registration_time) RegistrationDate, CONVERT(date,last_visit_time) LastVisited FROM [it].[STG_Users]				
			) S
		ON S.Id = T.UserID	
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([UserID], [UserName], Deleted, RegistrationDate, LastVisited, [ModifiedBy], [ModifiedDate])
		VALUES (S.Id, S.[login], S.deleted, S.RegistrationDate, LastVisited, @UserName, @DateTimeNow );

	-- Merge User Badges
	PRINT 'Merging user data from STG_UserBadges to UserBadges table.'
	MERGE [it].[UserBadges] T
		USING (	SELECT DISTINCT user_id, badge_id, title, icon_url, CONVERT(date,activation_date) activation_date, CONVERT(date,earned_date) earned_date FROM [it].[STG_UserBadges]				
			) S
		ON (S.user_id = T.UserID AND S.badge_id = T.BadgeID)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([UserID], [BadgeID], BadgeActivationDate, BadgeEarnedDate, [ModifiedBy], [ModifiedDate])
		VALUES (S.user_id, S.badge_id, S.activation_date, S.earned_date, @UserName, @DateTimeNow );

	-- Merge Badges
	PRINT 'Merging user badges from STG_UserBadges to Badges table.'
	MERGE [it].[Badges] T
		USING (SELECT DISTINCT badge_id, title, icon_url FROM it.STG_UserBadges ) S
			ON S.badge_id = T.BadgeID
		WHEN NOT MATCHED BY TARGET THEN
		INSERT (BadgeID, BadgeTitle, BadgeIconUrl, ModifiedBy, ModifiedDate)
		VALUES (S.badge_id, S.title, S.icon_url, @UserName, @DateTimeNow);

	PRINT 'Log staging table row counts'
	INSERT INTO [it].[ETLAudit] (ETLJobName, TableName, [RowCount], [TableLastUpdated], ETLAuditDesc)
		VALUES	(@ProcName,'it.STG_Categories', (SELECT COUNT(*) FROM [it].[STG_Categories]), NULL, @StgTableLoadMsg),
				(@ProcName,'it.STG_Boards', (SELECT COUNT(*) FROM [it].[STG_Boards]), NULL, @StgTableLoadMsg),
				(@ProcName,'it.STG_Messages', (SELECT COUNT(*) FROM [it].[STG_Messages]), (SELECT MAX(post_time) FROM [it].[STG_Messages]), @StgTableLoadMsg),
				(@ProcName,'it.STG_Users', (SELECT COUNT(*) FROM [it].[STG_Users]), (SELECT MAX(registration_time) FROM [it].[STG_Users]), @StgTableLoadMsg),
				(@ProcName,'it.STG_Kudos', (SELECT COUNT(*) FROM [it].[STG_Kudos]), NULL, @StgTableLoadMsg),
				(@ProcName,'it.STG_UserBadges', (SELECT COUNT(*) FROM [it].[STG_UserBadges]), NULL, @StgTableLoadMsg);

	-- Update Community page title
	SELECT @CommnunityPageTitle = ParamValue FROM [it].[Parameters] WHERE ParamName = 'CommunityTitle'
	
	SELECT @MessageDays = DATEDIFF(d,MIN(ISNULL(PostedDate,GETDATE())), GETDATE()) FROM [it].[Messages]
	SET @CommnunityPageTitle = @CommnunityPageTitle + ' Overview for Past ' + @MessageDays + ' Days'
	UPDATE [it].[Parameters] SET ParamValue = @CommnunityPageTitle WHERE ParamName = 'CommunityPageTitle'

	PRINT 'Truncating staging tables.'
	TRUNCATE TABLE [it].[STG_Categories]
	TRUNCATE TABLE [it].[STG_Boards]
	TRUNCATE TABLE [it].[STG_Messages]
	TRUNCATE TABLE [it].[STG_Users]
	TRUNCATE TABLE [it].[STG_Kudos]
	TRUNCATE TABLE [it].[STG_UserBadges]

	COMMIT TRANSACTION;
END TRY
BEGIN CATCH
	SELECT   
        ERROR_NUMBER() AS ErrorNumber
        ,ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState  
        ,ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine  
        ,ERROR_MESSAGE() AS ErrorMessage;

	IF @@TRANCOUNT > 0  
        ROLLBACK TRANSACTION;
END CATCH   

GO


