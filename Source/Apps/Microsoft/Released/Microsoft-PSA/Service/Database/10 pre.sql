SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


DECLARE @stmt AS VARCHAR(500), @p1 AS VARCHAR(100), @p2 AS VARCHAR(100);
DECLARE @cr CURSOR;

-- Must be executed inside the target database
-- drop views
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_TYPE='VIEW' AND
                    TABLE_SCHEMA='psa' AND
                    TABLE_NAME IN ('ZResourceCapacityView', 'TransactionCategoryView', 'TimeEntryView', 'ResourceView', 'ResourceRequirementView', 'ResourceRequirementDetailView', 'ResourceRequestView',
                                   'ResourceBookingView', 'QuoteView', 'QuoteLineView', 'ProjectView', 'ProjectContractView', 'OrganizationalUnitView', 'OpportunityView', 'NamedResourceWorkCapacityView',
                                   'MeasuresView', 'DefaultResourceCategoryView', 'DateView', 'DateCapacityView', 'CustomerView', 'ContractView', 'ContractLineView', 'BusinessTransactionView',
                                   'BookingStatusView');
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP VIEW psa.' + QuoteName(@p1);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;


--- DROP PSA TABLES---
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_TYPE='BASE TABLE' AND TABLE_SCHEMA='psa' AND TABLE_NAME IN ('WeekDayCapacity', 'Date', 'configuration', 'entityinitialcount');
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP TABLE psa.' + QuoteName(@p1);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;


--- DROP DBO VIEWS---
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_TYPE='VIEW' AND
                    TABLE_SCHEMA='dbo' AND
                    TABLE_NAME IN ('timeline_os_opportunity', 'territorycode_os_account', 'status_os_bookingstatus', 'shipto_freighttermscode_os_salesorderdetail', 'shipto_freighttermscode_os_salesorder',
                                   'shipto_freighttermscode_os_quotedetail', 'shipto_freighttermscode_os_quote', 'shippingmethodcode_os_salesorder', 'shippingmethodcode_os_quote', 'shippingmethodcode_os_account',
                                   'salesstage_gos', 'salesstagecode_os_opportunity', 'salesorder_status', 'salesorder_state', 'salesorderstatecode_os_salesorderdetail', 'resourcetype_os_bookableresource',
                                   'quote_status', 'quote_state', 'quotestatecode_os_quotedetail', 'quoteclose_status', 'quoteclose_state', 'purchasetimeframe_gos', 'purchaseprocess_gos', 'propertyconfigurationstatus_gos',
                                   'producttypecode_gos', 'prioritycode_os_salesorder', 'prioritycode_os_opportunity', 'pricingerrorcode_gos', 'preferredcontactmethodcode_os_account', 'preferredappointmenttimecode_os_account',
                                   'preferredappointmentdaycode_os_account', 'paymenttermscode_os_salesorder', 'paymenttermscode_os_quote', 'paymenttermscode_os_account', 'ownershipcode_os_account', 'opportunity_status',
                                   'opportunity_state', 'opportunityratingcode_os_opportunity', 'opportunityclose_status', 'opportunityclose_state', 'need_gos', 'msdyn_vendortype_gos', 'msdyn_type_os_msdyn_resourcerequirement',
                                   'msdyn_type_gos', 'msdyn_transactiontypecode_gos', 'msdyn_transactionclassification_gos', 'msdyn_timeoffcalendar_status', 'msdyn_timeoffcalendar_state', 'msdyn_timeentry_status',
                                   'msdyn_timeentry_state', 'msdyn_targetexpensestatus_gos', 'msdyn_targetentrystatus_gos', 'msdyn_scheduleperformance_os_msdyn_project', 'msdyn_resourcerequirement_status',
                                   'msdyn_resourcerequirement_state', 'msdyn_resourcerequirementdetail_status', 'msdyn_resourcerequirementdetail_state', 'msdyn_resourcerequest_status', 'msdyn_resourcerequest_state',
                                   'msdyn_relateditemtype_os_msdyn_timeentry', 'msdyn_receiptrequired_gos', 'msdyn_psastatusreason_gos', 'msdyn_psastate_gos', 'msdyn_project_status', 'msdyn_project_state',
                                   'msdyn_projectteam_status', 'msdyn_projectteam_state', 'msdyn_profitability_gos', 'msdyn_overallprojectstatus_os_msdyn_project', 'msdyn_organizationalunit_status', 'msdyn_organizationalunit_state',
                                   'msdyn_ordertype_os_salesorder', 'msdyn_ordertype_os_quote', 'msdyn_ordertype_os_opportunity', 'msdyn_orderlineresourcecategory_status', 'msdyn_orderlineresourcecategory_state',
                                   'msdyn_membershipstatus_os_msdyn_projectteam', 'msdyn_feasible_gos', 'msdyn_expense_status', 'msdyn_expense_state', 'msdyn_expensetype_gos', 'msdyn_expensestatus_gos', 'msdyn_expensecategory_status',
                                   'msdyn_expensecategory_state', 'msdyn_estimateline_status', 'msdyn_estimateline_state', 'msdyn_estimatedschedule_gos', 'msdyn_estimatedbudget_gos', 'msdyn_entrystatus_gos', 'msdyn_customertype_gos',
                                   'msdyn_costperformence_os_msdyn_project', 'msdyn_competitive_gos', 'msdyn_committype_gos', 'msdyn_bulkgenerationstatus_gos', 'msdyn_billingtype_gos', 'msdyn_billingstatus_gos',
                                   'msdyn_billingmethod_gos', 'msdyn_amountmethod_gos', 'msdyn_allocationmethod_os_msdyn_resourcerequirement', 'msdyn_allocationmethod_os_msdyn_projectteam', 'msdyn_adjustmentstatus_gos',
                                   'msdyn_actual_status', 'msdyn_actual_state', 'initialcommunication_gos', 'industrycode_os_account', 'freighttermscode_os_salesorder', 'freighttermscode_os_quote', 'customertypecode_os_account',
                                   'customersizecode_os_account', 'businesstypecode_os_account', 'budgetstatus_gos', 'bookingtype_os_bookableresourcebooking', 'bookingstatus_status', 'bookingstatus_state', 'bookableresource_status',
                                   'bookableresource_state', 'bookableresourcecategory_status', 'bookableresourcecategory_state', 'bookableresourcecategoryassn_status', 'bookableresourcecategoryassn_state', 'bookableresourcebooking_status',
                                   'bookableresourcebooking_state', 'address2_shippingmethodcode_os_account', 'address2_freighttermscode_os_account', 'address2_addresstypecode_os_account', 'address1_shippingmethodcode_os_account',
                                   'address1_freighttermscode_os_account', 'address1_addresstypecode_os_account', 'account_state', 'accountratingcode_os_account', 'accountclassificationcode_os_account', 'accountcategorycode_os_account');
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP VIEW dbo.' + QuoteName(@p1);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;




--- DROP DBO TABLES---
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_TYPE='BASE TABLE' AND 
                    TABLE_SCHEMA='dbo' AND 
                    TABLE_NAME IN ('systemuser', 'StatusMetadata','AttributeMetadata', 'TargetMetadata','StateMetadata', 'salesorderdetail', 'salesorder', 'quotedetail', 'quote', 'OptionSetMetadata', 'opportunity', 'msdyn_transactioncategory',
                                   'msdyn_timeentry', 'msdyn_resourcerequirementdetail', 'msdyn_resourcerequirement', 'msdyn_resourcerequest', 'msdyn_project', 'msdyn_projecttask', 'msdyn_organizationalunit', 'msdyn_orderlineresourcecategory',
                                   'msdyn_estimateline', 'msdyn_actual', 'GlobalOptionSetMetadata', 'bookingstatus', 'bookableresourcecategoryassn', 'bookableresourcecategory', 'bookableresourcebooking', 'bookableresource',
                                   'account');
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP TABLE dbo.' + QuoteName(@p1);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;


-- drop SPs
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT ROUTINE_SCHEMA, ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES
              WHERE ROUTINE_TYPE='PROCEDURE' AND 
                    ( (ROUTINE_SCHEMA='psa' AND ROUTINE_NAME IN ('sp_get_replication_counts', 'sp_get_prior_content', 'sp_get_last_updatetime','sp_get_pull_status')) OR
                      (ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME IN ('UpsertAttributeMetadata', 'UpsertGlobalOptionSetMetadata', 'UpsertOptionSetMetadata', 'UpsertStateMetadata', 'UpsertStatusMetadata', 'UpsertTargetMetadata'))
                    );
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1, @p2;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP PROCEDURE ' + QuoteName(@p1) + '.' + QuoteName(@p2);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1, @p2;
END;
CLOSE @cr;
DEALLOCATE @cr;

-- drop known (to this solution) user defined types 
SET @cr = CURSOR FAST_FORWARD FOR
   SELECT [name] FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name] IN ('accountIdType', 'accountType', 'AttributeMetadataList', 'bookableresourcebookingIdType',
                                                                                           'bookableresourcebookingType', 'bookableresourcecategoryassnIdType', 'bookableresourcecategoryassnType',
                                                                                           'bookableresourcecategoryIdType', 'bookableresourcecategoryType', 'bookableresourceIdType', 'bookableresourceType',
                                                                                           'bookingstatusIdType', 'bookingstatusType', 'msdyn_actualIdType', 'msdyn_actualType', 'msdyn_estimatelineIdType',
                                                                                           'msdyn_estimatelineType', 'msdyn_orderlineresourcecategoryIdType', 'msdyn_orderlineresourcecategoryType',
                                                                                           'msdyn_organizationalunitIdType', 'msdyn_organizationalunitType', 'msdyn_projectIdType', 'msdyn_projectType', 'msdyn_projecttaskIdType', 'msdyn_projecttaskType',
                                                                                           'msdyn_resourcerequestIdType', 'msdyn_resourcerequestType', 'msdyn_resourcerequirementdetailIdType', 
                                                                                           'msdyn_resourcerequirementdetailType', 'msdyn_resourcerequirementIdType', 'msdyn_resourcerequirementType', 
                                                                                           'msdyn_timeentryIdType', 'msdyn_timeentryType', 'msdyn_transactioncategoryIdType', 'msdyn_transactioncategoryType', 
                                                                                           'opportunityIdType', 'opportunityType', 'OptionSetMetadataList', 'quotedetailIdType', 'quotedetailType', 'quoteIdType',
                                                                                           'quoteType', 'salesorderdetailIdType', 'salesorderdetailType', 'salesorderIdType', 'salesorderType', 'StateMetadataList',
                                                                                           'StatusMetadataList', 'systemuserIdType', 'systemuserType', 'TargetMetadataList');
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'DROP TYPE dbo.' + QuoteName(@p1);
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;


IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='psa')
BEGIN
    EXEC ('CREATE SCHEMA psa AUTHORIZATION dbo'); -- Avoid batch error
END;
