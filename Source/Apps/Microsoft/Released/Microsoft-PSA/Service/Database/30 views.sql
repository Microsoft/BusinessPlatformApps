SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- dbo views
CREATE VIEW dbo.accountcategorycode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'accountcategorycode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.accountclassificationcode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'accountclassificationcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.accountratingcode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'accountratingcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.account_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'account'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.address1_addresstypecode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'address1_addresstypecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.address1_freighttermscode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'address1_freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.address1_shippingmethodcode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'address1_shippingmethodcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.address2_addresstypecode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'address2_addresstypecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.address2_freighttermscode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'address2_freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.address2_shippingmethodcode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'address2_shippingmethodcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.bookableresourcebooking_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'bookableresourcebooking'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookableresourcebooking_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'bookableresourcebooking'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookableresourcecategoryassn_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'bookableresourcecategoryassn'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookableresourcecategoryassn_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'bookableresourcecategoryassn'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookableresourcecategory_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'bookableresourcecategory'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookableresourcecategory_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'bookableresourcecategory'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookableresource_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'bookableresource'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookableresource_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'bookableresource'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookingstatus_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'bookingstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookingstatus_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'bookingstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.bookingtype_os_bookableresourcebooking
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'bookingtype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'bookableresourcebooking';
go

CREATE VIEW dbo.budgetstatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'budgetstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.businesstypecode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'businesstypecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.customersizecode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'customersizecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.customertypecode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'customertypecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.freighttermscode_os_quote
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'quote';
go

CREATE VIEW dbo.freighttermscode_os_salesorder
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorder';
go

CREATE VIEW dbo.industrycode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'industrycode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.initialcommunication_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'initialcommunication'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_actual_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_actual'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_actual_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_actual'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_adjustmentstatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_adjustmentstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_allocationmethod_os_msdyn_projectteam
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_allocationmethod'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_projectteam';
go

CREATE VIEW dbo.msdyn_allocationmethod_os_msdyn_resourcerequirement
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_allocationmethod'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_resourcerequirement';
go

CREATE VIEW dbo.msdyn_amountmethod_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_amountmethod'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_billingmethod_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_billingmethod'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_billingstatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_billingstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_billingtype_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_billingtype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_bulkgenerationstatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_bulkgenerationstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_committype_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_committype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_competitive_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_competitive'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_costperformence_os_msdyn_project
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_costperformence'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_project';
go

CREATE VIEW dbo.msdyn_customertype_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_customertype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_entrystatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_entrystatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_estimatedbudget_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_estimatedbudget'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_estimatedschedule_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_estimatedschedule'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_estimateline_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_estimateline'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_estimateline_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_estimateline'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_expensecategory_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_expensecategory'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_expensecategory_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_expensecategory'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_expensestatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_expensestatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_expensetype_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_expensetype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_expense_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_expense'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_expense_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_expense'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_feasible_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_feasible'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_membershipstatus_os_msdyn_projectteam
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_membershipstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_projectteam';
go

CREATE VIEW dbo.msdyn_orderlineresourcecategory_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_orderlineresourcecategory'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_orderlineresourcecategory_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_orderlineresourcecategory'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_ordertype_os_opportunity
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_ordertype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'opportunity';
go

CREATE VIEW dbo.msdyn_ordertype_os_quote
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_ordertype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'quote';
go

CREATE VIEW dbo.msdyn_ordertype_os_salesorder
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_ordertype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorder';
go

CREATE VIEW dbo.msdyn_organizationalunit_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_organizationalunit'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_organizationalunit_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_organizationalunit'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_overallprojectstatus_os_msdyn_project
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_overallprojectstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_project';
go

CREATE VIEW dbo.msdyn_profitability_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_profitability'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_projectteam_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_projectteam'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_projectteam_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_projectteam'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_project_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_project'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_project_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_project'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_psastate_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_psastate'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_psastatusreason_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_psastatusreason'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_receiptrequired_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_receiptrequired'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_relateditemtype_os_msdyn_timeentry
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_relateditemtype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_timeentry';
go

CREATE VIEW dbo.msdyn_resourcerequest_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_resourcerequest'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_resourcerequest_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_resourcerequest'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_resourcerequirementdetail_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_resourcerequirementdetail'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_resourcerequirementdetail_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_resourcerequirementdetail'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_resourcerequirement_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_resourcerequirement'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_resourcerequirement_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_resourcerequirement'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_scheduleperformance_os_msdyn_project
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_scheduleperformance'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_project';
go

CREATE VIEW dbo.msdyn_targetentrystatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_targetentrystatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_targetexpensestatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_targetexpensestatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_timeentry_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_timeentry'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_timeentry_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_timeentry'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_timeoffcalendar_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'msdyn_timeoffcalendar'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_timeoffcalendar_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'msdyn_timeoffcalendar'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_transactionclassification_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_transactionclassification'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_transactiontypecode_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_transactiontypecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_type_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_type'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.msdyn_type_os_msdyn_resourcerequirement
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'msdyn_type'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'msdyn_resourcerequirement';
go

CREATE VIEW dbo.msdyn_vendortype_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'msdyn_vendortype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.need_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'need'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.opportunityclose_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'opportunityclose'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.opportunityclose_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'opportunityclose'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.opportunityratingcode_os_opportunity
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'opportunityratingcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'opportunity';
go

CREATE VIEW dbo.opportunity_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'opportunity'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.opportunity_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'opportunity'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.ownershipcode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'ownershipcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.paymenttermscode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'paymenttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.paymenttermscode_os_quote
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'paymenttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'quote';
go

CREATE VIEW dbo.paymenttermscode_os_salesorder
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'paymenttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorder';
go

CREATE VIEW dbo.preferredappointmentdaycode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'preferredappointmentdaycode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.preferredappointmenttimecode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'preferredappointmenttimecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.preferredcontactmethodcode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'preferredcontactmethodcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.pricingerrorcode_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'pricingerrorcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.prioritycode_os_opportunity
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'prioritycode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'opportunity';
go

CREATE VIEW dbo.prioritycode_os_salesorder
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'prioritycode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorder';
go

CREATE VIEW dbo.producttypecode_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'producttypecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.propertyconfigurationstatus_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'propertyconfigurationstatus'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.purchaseprocess_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'purchaseprocess'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.purchasetimeframe_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'purchasetimeframe'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.quoteclose_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'quoteclose'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.quoteclose_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'quoteclose'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.quotestatecode_os_quotedetail
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'quotestatecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'quotedetail';
go

CREATE VIEW dbo.quote_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'quote'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.quote_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'quote'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.resourcetype_os_bookableresource
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'resourcetype'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'bookableresource';
go

CREATE VIEW dbo.salesorderstatecode_os_salesorderdetail
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'salesorderstatecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorderdetail';
go

CREATE VIEW dbo.salesorder_state
AS
  SELECT [state]        AS [Value],
         localizedlabel AS Label
  FROM   dbo.statemetadata
  WHERE  entityname = 'salesorder'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.salesorder_status
AS
  SELECT [status]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.statusmetadata
  WHERE  entityname = 'salesorder'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.salesstagecode_os_opportunity
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'salesstagecode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'opportunity';
go

CREATE VIEW dbo.salesstage_gos
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.globaloptionsetmetadata
  WHERE  optionsetname = 'salesstage'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033;
go

CREATE VIEW dbo.shippingmethodcode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'shippingmethodcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.shippingmethodcode_os_quote
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'shippingmethodcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'quote';
go

CREATE VIEW dbo.shippingmethodcode_os_salesorder
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'shippingmethodcode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorder';
go

CREATE VIEW dbo.shipto_freighttermscode_os_quote
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'shipto_freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'quote';
go

CREATE VIEW dbo.shipto_freighttermscode_os_quotedetail
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'shipto_freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'quotedetail';
go

CREATE VIEW dbo.shipto_freighttermscode_os_salesorder
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'shipto_freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorder';
go

CREATE VIEW dbo.shipto_freighttermscode_os_salesorderdetail
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'shipto_freighttermscode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'salesorderdetail';
go

CREATE VIEW dbo.status_os_bookingstatus
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'status'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'bookingstatus';
go

CREATE VIEW dbo.territorycode_os_account
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'territorycode'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'account';
go

CREATE VIEW dbo.timeline_os_opportunity
AS
  SELECT [option]       AS [Value],
         localizedlabel AS Label
  FROM   dbo.optionsetmetadata
  WHERE  optionsetname = 'timeline'
         AND isuserlocalizedlabel = 1
         AND localizedlabellanguagecode = 1033
         AND entityname = 'opportunity';
go

-- PSA views
CREATE VIEW psa.BookingStatusView
AS
  SELECT bookingstatusid     AS [Booking Status Id],
         [status]            AS [Booking Status Code],
         bookingstatus.label AS [Booking Status Name],
         CommitType.label    AS [Commit Type],
         msdyn_committype    AS [Commit Type Code]
  FROM   dbo.bookingstatus bs
         LEFT OUTER JOIN dbo.status_os_bookingstatus bookingstatus
                      ON status = bookingstatus.value
         LEFT OUTER JOIN dbo.msdyn_committype_gos CommitType
                      ON CommitType.value = msdyn_committype
  WHERE  NOT (( ( statecode != 0 )
                AND ( Datediff(year, Getdate(), modifiedon) < -1 ) ));
go

CREATE VIEW psa.ProjectView
AS
  SELECT msdyn_projectid                     AS [Project Id],
         msdyn_customer                      AS [Project Customer Id],
         msdyn_subject                       AS [Project Name],
         msdyn_description                   AS [Project Description],
         msdyn_projectmanager                AS [Project Manager User Id],
         systemuser.fullname                 AS [Project Manager Name],
         CONVERT(DATE, msdyn_actualstart)    AS [Project Actual Start],
         CONVERT(DATE, msdyn_actualend)      AS [Project Actual End],
         CONVERT(DATE, msdyn_scheduledstart) AS [Project Scheduled Start],
         CONVERT(DATE, msdyn_scheduledend)   AS [Project Scheduled End],
         CostPerformance.label               AS [Project Cost Performance],
         msdyn_costperformence               AS [Project Cost Performance Code],
         SchedulePerformance.label           AS [Project Schedule Performance],
         msdyn_scheduleperformance           AS [Project Schedule Performance Code],
         msdyn_stagename                     AS [Project Stage Name],
         msdyn_contractorganizationalunitid  AS [Project Contracting Unit Id]
  FROM   dbo.msdyn_project LEFT OUTER JOIN msdyn_costperformence_os_msdyn_project CostPerformance ON CostPerformance.[value] = msdyn_costperformence
                           LEFT OUTER JOIN msdyn_scheduleperformance_os_msdyn_project SchedulePerformance ON SchedulePerformance.[value] = msdyn_scheduleperformance
                           LEFT OUTER JOIN dbo.systemuser SystemUser ON msdyn_projectmanager = systemuserid
  WHERE  statecode = 0
         AND ( msdyn_istemplate = 0 )
         AND ( msdyn_actualend IS NULL
                 OR Datediff(year, Getdate(), msdyn_actualend) >= -1 );
go

CREATE VIEW psa.BusinessTransactionView
AS
-- Actuals
  SELECT 0                                 AS [Is Estimate],
         CONVERT(DATE, msdyn_documentdate) AS [Document Date],
         BillingType.label                 AS [Billing Type],
         BillingStatus.label               AS [Billing Status],
         msdyn_billingtype                 AS [Billing Type Code],
         msdyn_billingstatus               AS [Billing Status Code],
         msdyn_transactioncategory         AS [Transaction Category Id],
         msdyn_transactiontypecode         AS [Transaction Type Code],
         msdyn_transactionclassification   AS [Transaction Class Code],
         TransactionTypeCode.label         AS [Transaction Type],
         TransactionClass.label            AS [Transaction Class],
         msdyn_accountcustomer             AS [Customer Id],
         msdyn_quantity                    AS Quantity,
         msdyn_bookableresource            AS [Resource Id],
         msdyn_project                     AS [Project Id],
         msdyn_salescontractline           AS [Contract Line Id],
         msdyn_salescontract               AS [Contract Id],
         msdyn_amount_base                 AS Amount,
         msdyn_resourcecategory            AS [Resource Category Id]
  FROM   dbo.msdyn_actual
         LEFT OUTER JOIN dbo.msdyn_transactionclassification_gos TransactionClass ON TransactionClass.value = msdyn_transactionclassification
         LEFT OUTER JOIN dbo.msdyn_transactiontypecode_gos TransactionTypeCode ON TransactionTypeCode.value = msdyn_transactiontypecode
         LEFT OUTER JOIN dbo.msdyn_billingtype_gos BillingType ON BillingType.value = msdyn_billingtype
         LEFT OUTER JOIN dbo.msdyn_billingstatus_gos BillingStatus ON BillingStatus.value = msdyn_billingstatus
  WHERE  ( Datediff(year, Getdate(), msdyn_documentdate) >= -1 )
         AND ( Datediff(day, Getdate(), msdyn_documentdate) <= 366 )
  UNION ALL -- non-Time Estimates
  SELECT 1                                 AS [Is Estimate],
         CONVERT(DATE, msdyn_documentdate) AS [Document Date],
         BillingType.label                 AS [Billing Type],
         NULL                              AS [Billing Status],
         msdyn_billingtype                 AS [Billing Type Code],
         NULL                              AS [Billing Status Code],
         msdyn_transactioncategory         AS [Transaction Category Id],
         msdyn_transactiontypecode         AS [Transaction Type Code],
         msdyn_transactionclassification   AS [Transaction Class Code],
         TransactionTypeCode.label         AS [Transaction Type],
         TransactionClass.label            AS [Transaction Class],
         msdyn_accountcustomer             AS [Customer Id],
         msdyn_quantity                    AS Quantity,
         msdyn_bookableresource            AS [Resource Id],
         msdyn_project                     AS [Project Id],
         NULL                              AS [Contract Line Id],
         NULL                              AS [Contract Id],
         msdyn_amount_base                 AS Amount,
         msdyn_resourcecategory            AS [Resource Category Id]
  FROM   dbo.msdyn_estimateline
         LEFT OUTER JOIN dbo.msdyn_transactionclassification_gos TransactionClass
                      ON TransactionClass.value = msdyn_transactionclassification
         LEFT OUTER JOIN dbo.msdyn_transactiontypecode_gos TransactionTypeCode
                      ON TransactionTypeCode.value = msdyn_transactiontypecode
         LEFT OUTER JOIN dbo.msdyn_billingtype_gos BillingType
                      ON BillingType.value = msdyn_billingtype
  WHERE statecode = 0 AND msdyn_transactionclassification != 192350000 -- Time
        AND ( Datediff(year, Getdate(), msdyn_documentdate) >= -1 )
        AND ( Datediff(day, Getdate(), msdyn_documentdate) <= 366 )
  UNION ALL -- Time Cost Estimates
  SELECT 
          1                                 AS [Is Estimate],
          CONVERT(DATE, msdyn_scheduledstart) AS [Document Date],
          BillingType.label                 AS [Billing Type],
          NULL                              AS [Billing Status],
          CASE 
          WHEN (msdyn_plannedsales_Base > 0) -- if we have sales amount then it is chargeable
          THEN 192350001 -- Chargeable 
          ELSE 192350000 -- Non-Chargeable
          END                             AS [Billing Type Code],
          NULL                              AS [Billing Status Code],
          msdyn_transactioncategory         AS [Transaction Category Id],
          192350000                         AS [Transaction Type Code], -- Cost
          192350000                         AS [Transaction Class Code], -- Time		 
          TransactionTypeCode.label         AS [Transaction Type],
          TransactionClass.label            AS [Transaction Class],	     
          project.[Project Customer Id]     AS [Customer Id],
          msdyn_Effort					   AS Quantity,
          null						       AS [Resource Id],
          msdyn_project                     AS [Project Id],
          NULL                              AS [Contract Line Id],
          NULL                              AS [Contract Id],
          msdyn_plannedcost_base            AS Amount,
          msdyn_resourcecategory            AS [Resource Category Id]
  FROM   dbo.msdyn_projecttask task
              INNER JOIN psa.ProjectView project on task.msdyn_project = project.[Project Id]
              LEFT OUTER JOIN dbo.msdyn_transactionclassification_gos TransactionClass ON TransactionClass.value = 192350000 -- Time
              LEFT OUTER JOIN dbo.msdyn_transactiontypecode_gos TransactionTypeCode ON TransactionTypeCode.value = 192350000 -- Cost
              LEFT OUTER JOIN dbo.msdyn_billingtype_gos BillingType ON BillingType.value = CASE 
                                                                                              WHEN (msdyn_plannedsales_Base > 0) THEN 192350001 -- if we have sales amount then it is chargeable
                                                                                              ELSE 192350000 -- Non-Chargeable
                                                                                          END
  WHERE msdyn_islinetask = 1											 											 
  UNION ALL -- Time UnbilledSales Estimates
  SELECT 
          1                                 AS [Is Estimate],
          CONVERT(DATE, msdyn_scheduledstart) AS [Document Date],
          BillingType.label                 AS [Billing Type],
          NULL                              AS [Billing Status],
          192350001				           AS [Billing Type Code], -- Chargeable
          NULL                              AS [Billing Status Code],
          msdyn_transactioncategory         AS [Transaction Category Id],
          192350005                         AS [Transaction Type Code], -- UnbilledSales
          192350000                         AS [Transaction Class Code], -- Time		 
          TransactionTypeCode.label         AS [Transaction Type],
          TransactionClass.label            AS [Transaction Class],	     
          project.[Project Customer Id]     AS [Customer Id],
          msdyn_Effort                      AS Quantity,
          null						       AS [Resource Id],
          msdyn_project                     AS [Project Id],
          NULL                              AS [Contract Line Id],
          NULL                              AS [Contract Id],
          msdyn_plannedsales_Base           AS Amount,
          msdyn_resourcecategory            AS [Resource Category Id]
  FROM   dbo.msdyn_projecttask task
              INNER Join psa.ProjectView project on task.msdyn_project = project.[Project Id]
              LEFT OUTER JOIN dbo.msdyn_transactionclassification_gos TransactionClass ON TransactionClass.value = 192350000 -- Time
              LEFT OUTER JOIN dbo.msdyn_transactiontypecode_gos TransactionTypeCode ON TransactionTypeCode.value = 192350005 -- UnbilledSales
              LEFT OUTER JOIN dbo.msdyn_billingtype_gos BillingType ON BillingType.value = 192350001 -- Chargeable
  WHERE msdyn_islinetask = 1;
go


/*
Pickup lines for all not closed contracts and the ones that were closed last year
*/
CREATE VIEW psa.ContractLineView
AS
  SELECT salesorderid        AS [Contract Id],
         salesorderdetailid  AS [Contract Line Id],
         productdescription  AS [Contract Line Description],
         BillingMethod.label AS [Billing Method],
         msdyn_project       AS [Project Id]
  FROM   dbo.salesorderdetail
         LEFT OUTER JOIN dbo.msdyn_billingmethod_gos BillingMethod
                      ON BillingMethod.value = msdyn_billingmethod
  WHERE  EXISTS(SELECT salesorderid
                FROM   dbo.salesorder
                WHERE  NOT (( ( msdyn_psastate = 192350003 )
                              AND ( Datediff(year, Getdate(), modifiedon) < -1 ) )));
go


/*
Pickup all not closed contracts and the ones that were closed last year
*/
CREATE VIEW psa.ContractView
AS
  SELECT salesorderid           AS [Contract Id],
         NAME                   AS [Contract Name],
         msdyn_psastate         AS [Contract State Code],
         PSA_State.label        AS [Contract State],
         msdyn_psastatusreason  AS [Contract Status Code],
         PSA_StatusReason.label AS [Contract Status],
         customerid             AS [Contract Customer Id],
         totalamount_base       AS [Contract Total Amount]
  FROM   dbo.salesorder
         LEFT OUTER JOIN dbo.msdyn_psastate_gos PSA_State
                      ON PSA_State.value = msdyn_psastate
         LEFT OUTER JOIN dbo.msdyn_psastatusreason_gos PSA_StatusReason
                      ON PSA_StatusReason.value = msdyn_psastatusreason
  WHERE  NOT (( ( msdyn_psastate = 192350003 )
                AND ( Datediff(year, Getdate(), modifiedon) < -1 ) ));
go


/*
Active customers and customers that became inactive during last year
*/
CREATE VIEW psa.CustomerView
AS
  SELECT accountid                AS [Customer Id],
         NAME                     AS [Customer Name],
         sic                      AS SIC,
         address1_postalcode      AS [Postal Code],
         address1_telephone1      AS Phone,
         address1_stateorprovince AS [State/Province],
         address1_country         AS Country,
         address1_city            AS City,
         emailaddress1            AS [E-Mail],
         customertypecode.label   AS [Customer Type],
         industrycode             AS [Industry Code],
         IndustryCode.label       AS Industry
  FROM   dbo.account
         LEFT OUTER JOIN dbo.customertypecode_os_account CustomerTypeCode
                      ON CustomerTypeCode.value = customertypecode
         LEFT OUTER JOIN dbo.industrycode_os_account IndustryCode
                      ON IndustryCode.value = industrycode
  WHERE  NOT (( ( statecode != 0 )
                AND ( Datediff(year, Getdate(), modifiedon) < -1 ) ));
go

CREATE VIEW psa.DateCapacityView
AS
  SELECT dt.full_date              AS [Capacity Date],
         dc.capacityminutes        AS [Capacity Minutes],
         dc.capacityminutes / 60.0 AS [Capacity Hours]
  FROM   psa.date dt
         JOIN psa.weekdaycapacity dc
           ON dt.day_of_week = dc.day_of_week
  WHERE  ( Datediff(year, Getdate(), full_date) >= -1 )
         AND ( Datediff(day, Getdate(), full_date) <= 366 );
go

CREATE VIEW psa.DateView
AS
  SELECT full_date                             AS [Date],
         [year]                                AS [Year],
         [quarter]                             AS [Quarter],
         quarter_start_date                    AS [Quarter Start Date],
         quarter_end_date                      AS [Quarter End Date],
         quarter_name                          AS [Quarter Name],
         quarter_abbrevyear                    AS [Quarter With Year],
         [month]                               AS [Month],
         month_start_date                      AS [Month Start Date],
         month_end_date                        AS [Month End Date],
         month_name                            AS [Month Name],
         month_abbrev                          AS [Month Abbreviation],
         month_abbrevyear                      AS [Month With Year],
         day_of_year                           AS [Day of Year],
         day_of_month                          AS [Day of Month],
         day_of_week                           AS [Day of Week],
         day_name                              AS [Day Name],
         day_abbrev                            AS [Day Abbreviation],
         weeek_of_year                         AS [Week of Year],
         week_start_date                       AS [Week Start Date],
         week_end_date                         AS [Week End Date],
         week_time_frame                       AS [Week Time Frame],
         CASE ( Datediff(year, Getdate(), full_date) )
           WHEN -1 THEN 'Previous Year'
           WHEN 0 THEN 'Current Year'
           WHEN 1 THEN 'Next Year'
           ELSE NULL
         END                                     AS [Year Slice],
         CASE ( Datediff(month, Getdate(), full_date) )
           WHEN -1 THEN 'Previous Month'
           WHEN 0 THEN 'Current Month'
           WHEN 1 THEN 'Next Month'
           WHEN -12 THEN 'Same Month Previous Year'
           ELSE NULL
         END                                     AS [Month Slice],
         CASE ( Datediff(quarter, Getdate(), full_date) )
           WHEN -1 THEN 'Previous Quarter'
           WHEN 0 THEN 'Current Quarter'
           WHEN 1 THEN 'Next Quarter'
           WHEN -4 THEN 'Same Quarter Previous Year'
           ELSE NULL
         END                                     AS [Quarter Slice],
         CASE ( Datediff(week, Getdate(), full_date) )
           WHEN -1 THEN 'Previous Week'
           WHEN 0 THEN 'Current Week'
           WHEN 1 THEN 'Next Week'
           ELSE NULL
         END                                     AS [Week Slice],
         Datediff(day, Getdate(), full_date)     AS [Relative Day Number],
         Datediff(week, Getdate(), full_date)    AS [Relative Week Number],
         Datediff(month, Getdate(), full_date)   AS [Relative Month Number],
         Datediff(quarter, Getdate(), full_date) AS [Relative Quarter Number]
  FROM   psa.[date]
  WHERE  ( Datediff(year, Getdate(), full_date) >= -1 )
         AND ( Datediff(day, Getdate(), full_date) <= 366 );
go

CREATE VIEW psa.DefaultResourceCategoryView
AS
  SELECT bookableresourcecategoryid     AS [Resource Category Id],
         NAME                           AS [Resource Category Name],
         msdyn_targetutilization * 0.01 AS [Resource Category Target Utilization],
         ent_state.label                AS [Resource Category State],
         statecode                      AS [Resource Category State Code],
         BillingType.label              AS [Resource Category Chargeability]
  FROM   dbo.bookableresourcecategory
         LEFT OUTER JOIN dbo.bookableresource_state ent_state
                      ON ent_state.[value] = statecode
         LEFT OUTER JOIN dbo.msdyn_billingtype_gos BillingType
                      ON BillingType.[value] = msdyn_billingtype
  WHERE  NOT (( ( statecode != 0 )
                AND ( Datediff(year, Getdate(), modifiedon) < -1 ) ));
go

CREATE VIEW psa.MeasuresView
AS
  SELECT TOP 0 1 AS MeasureValues;
go

CREATE VIEW psa.NamedResourceWorkCapacityView
AS
  SELECT br.bookableresourceid AS [Resource Id],
         dc.*
  FROM   dbo.bookableresource br
         CROSS JOIN psa.datecapacityview dc
  WHERE  NOT (( ( br.statecode != 0 )
                AND ( Datediff(year, Getdate(), br.modifiedon) < -1 ) ));
go

CREATE VIEW psa.OpportunityView
AS
  SELECT opportunityid                       AS [Opportunity Id],
         NAME                                AS [Opportunity Name],
         [description]                       AS [Opportunity Description],
         msdyn_ordertype                     AS [Opportunity Type Code],
         OrderType.label                     AS [Opportunity Type],
         salesstage                          AS [Sales Stage Code],
         SalesStage.label                    AS [Sales Stage],
         CONVERT(DATE, actualclosedate)      AS [Opportunity Actual Close Date],
         actualvalue_base                    AS [Opportunity Actual Revenue],
         stepname                            AS [Step Name],
         statuscode                          AS [Opportunity Status Code],
         ent_status.label                    AS [Opportunity Status],
         opportunityratingcode               AS [Opportunity Rating Code],
         OpportunityRating.label             AS [Opportunity Rating],
         closeprobability * 0.01             AS [Opportunity Close Probability],
         estimatedvalue_base                 AS [Opportunity Estimated Revenue],
         CONVERT(DATE, opportunity.createdon)AS [Opportunity Created On Date],
         CONVERT(DATE, estimatedclosedate)   AS [Opportunity Estimated Close Date],
         customerid                          AS [Customer Id],
         customerneed                        AS [Customer Need],
         CONVERT(DATE, finaldecisiondate)    AS [Opportunity Final Decision Date],
         purchasetimeframe                   AS [Opportunity Purchase Time Frame Code],
         PurchaseTimeframe.label             AS [Opportunity Purchase Time Frame],
         totalamount_base                    AS [Opportunity Total Amount],
         statecode                           AS [Opportunity State Code],
         ent_state.label                     AS [Opportunity State],
         customerpainpoints                  AS [Customer Pain Points],
         msdyn_accountmanagerid              AS [Opportunity Account Manager Id],
         systemuser.fullname                 AS [Opportunity Account Manager],
         timeline                            AS [Opportunity Timeline Code],
         Timeline.label                      AS [Opportunity Timeline]
  FROM   dbo.opportunity opportunity
         LEFT OUTER JOIN dbo.opportunity_status ent_status
                      ON ent_status.[value] = statuscode
         LEFT OUTER JOIN dbo.opportunity_state ent_state
                      ON ent_state.[value] = statecode
         LEFT OUTER JOIN dbo.timeline_os_opportunity Timeline
                      ON Timeline.[value] = timeline
         LEFT OUTER JOIN dbo.purchasetimeframe_gos PurchaseTimeframe
                      ON PurchaseTimeframe.[value] = purchasetimeframe
         LEFT OUTER JOIN dbo.opportunityratingcode_os_opportunity OpportunityRating
                      ON OpportunityRating.[value] = opportunityratingcode
         LEFT OUTER JOIN dbo.salesstage_gos SalesStage
                      ON SalesStage.[value] = salesstage
         LEFT OUTER JOIN dbo.msdyn_ordertype_os_opportunity OrderType
                      ON OrderType.[value] = msdyn_ordertype
         LEFT OUTER JOIN dbo.systemuser SystemUser
                      ON msdyn_accountmanagerid = systemuserid
  WHERE  ( actualclosedate IS NULL
            OR Datediff(year, Getdate(), actualclosedate) >= -1 );
go

CREATE VIEW psa.OrganizationalUnitView
AS
  SELECT msdyn_organizationalunitid AS [Organizational Unit Id],
         msdyn_name                 AS [Organizational Unit Name],
         msdyn_description          AS [Organizational Unit Description],
         StateCode.label            AS [Organizational Unit State],
         statecode                  AS [Organizational Unit State Code]
  FROM   dbo.msdyn_organizationalunit  LEFT OUTER JOIN msdyn_organizationalunit_state StateCode ON StateCode.[value] = statecode
  WHERE  NOT (( ( statecode != 0 )
                AND ( Datediff(year, Getdate(), modifiedon) < -1 ) ));
go

CREATE VIEW psa.ProjectContractView
AS
  SELECT cv.[contract id],
         msdyn_project AS [Project Id]
  FROM   psa.contractview cv INNER JOIN dbo.salesorderdetail od ON od.salesorderid = cv.[contract id]
  WHERE  msdyn_project IS NOT NULL
  GROUP  BY cv.[contract id], msdyn_project;
go

CREATE VIEW psa.QuoteLineView
AS
  SELECT quoteid                   AS [Quote Id],
         producttypecode           AS [Quote Product Type Code],
         ProductTypeCode.label     AS [Quote Product Type],
         extendedamount_base       AS [Quote Line Extended Amount],
         baseamount_base           AS [Quote Line Base Amount],
         manualdiscountamount_base AS [Quote Line Manual Discount Amount],
         volumediscountamount_base AS [Quote Line Volume Discount Amount]
  FROM   dbo.quotedetail LEFT OUTER JOIN producttypecode_gos ProductTypeCode ON ProductTypeCode.[value] = producttypecode
  WHERE  NOT (( ( quotestatecode != 0 )
                AND ( Datediff(year, Getdate(), modifiedon) < -1 ) ));
go

CREATE VIEW psa.QuoteView
AS
  SELECT quoteid                                        AS [Quote Id],
         description                                    AS [Quote Description],
         quotenumber                                    AS [Quote Number],
         NAME                                           AS [Quote Name],
         msdyn_ordertype                                AS [Quote Type Code],
         OrderType.label                                AS [Quote Type],
         customerid                                     AS [Customer Id],
         EstimatedBudget.label                          AS [Quote Budget Estimation],
         msdyn_estimatedbudget                          AS [Quote Budget Estimation Code],
         EstimatedSchedule.label                        AS [Quote Schedule Estimation],
         msdyn_estimatedschedule                        AS [Quote Schedule Estimation Code],
         Feasible.label                                 AS [Quote Feasibility],
         msdyn_feasible                                 AS [Quote Feasibility Code],
         Profitability.label                            AS [Quote Profitability],
         msdyn_profitability                            AS [Quote Profitability Code],
         Competitive.label                              AS [Quote Competitivness],
         msdyn_competitive                              AS [Quote Competitivness Code],
         msdyn_grossmargin * 0.01                       AS [Quote Gross Margin (%)],
         msdyn_adjustedgrossmargin * 0.01               AS [Quote Adjusted Gross Margin (%)],
         msdyn_customerbudgetrollup_base                AS [Quote Customer Budget],
         totalamount_base                               AS [Quote Total Amount],
         msdyn_totalchargeablecostrollup_base           AS [Quote Total Chargeable Cost],
         msdyn_totalnonchargeablecostrollup_base        AS [Quote Total Non-Chargeable Cost],
         msdyn_accountmanagerid                         AS [Quote Account Manager User Id],
         systemuser.fullname                            AS [Quote Account Manager],
         CONVERT(DATE, quote.createdon)                 AS [Quote Created On Date],
         CONVERT(DATE, effectiveto)                     AS [Quote Effective To Date],
         CONVERT(DATE, msdyn_estimatedcompletionrollup) AS [Quote Estimated Completion Date],
         CONVERT(DATE, effectivefrom)                   AS [Quote Effective From Date],
         CONVERT(DATE, CASE
                         WHEN closedon IS NULL THEN
                           /*if the user didn't enter quote close date then we assume it is the date when quote state was changed to Won or Closed*/
                           CASE
                             WHEN statecode IN ( 2, 3 ) THEN quote.modifiedon
                             ELSE NULL
                           END
                         ELSE closedon
                       END)                             AS [Quote Closed On Date],
         CONVERT(DATE, requestdeliveryby)               AS [Quote Requested Delivery Date],
         opportunityid                                  AS [Opportunity Id],
         statecode                                      AS [Quote State Code],
         StateCode.label                                AS [Quote State],
         discountamount_base                            AS [Quote Discount Amount],
         discountpercentage * 0.01                      AS [Quote Discount (%)],
         totaldiscountamount_base                       AS [Quote Total Discount Amount],
         totallineitemdiscountamount_base               AS [Quote Total Line Item Discount Amount],
         totallineitemamount_base                       AS [Quote Total Line Item Amount]
  FROM   dbo.quote quote LEFT OUTER JOIN dbo.quote_state StateCode ON StateCode.[value] = statecode
                         LEFT OUTER JOIN dbo.msdyn_competitive_gos Competitive ON Competitive.[value] = msdyn_competitive
                         LEFT OUTER JOIN dbo.msdyn_profitability_gos Profitability ON Profitability.[value] = msdyn_profitability
                         LEFT OUTER JOIN dbo.msdyn_feasible_gos Feasible ON Feasible.[value] = msdyn_feasible
                         LEFT OUTER JOIN dbo.msdyn_estimatedschedule_gos EstimatedSchedule ON EstimatedSchedule.[value] = msdyn_estimatedschedule
                         LEFT OUTER JOIN dbo.msdyn_estimatedbudget_gos EstimatedBudget ON EstimatedBudget.[value] = msdyn_estimatedbudget
                         LEFT OUTER JOIN dbo.msdyn_ordertype_os_quote OrderType ON OrderType.[value] = msdyn_ordertype
                         LEFT OUTER JOIN dbo.systemuser SystemUser ON msdyn_accountmanagerid = systemuserid
  WHERE  NOT (( ( statecode != 0
                   OR statecode != 1 )
                AND ( Datediff(year, Getdate(), quote.modifiedon) < -1 ) ));
go

CREATE VIEW psa.ResourceBookingView
AS
  SELECT bk.bookingstatus            AS [Booking Status Id],
         bk.bookingtype              AS [Booking Type Code],
         BookingType.label           AS [Booking Type],
         bk.duration                 AS [Booking Duration Minutes],
         bk.duration / 60.0          AS [Booking Duration Hours],
         CONVERT(DATE, bk.starttime) AS [Booking Date],
         bk.msdyn_projectid          AS [Booked Project Id],
         bk.msdyn_resourcecategoryid AS [Booked Resource Category Id],
         bk.[resource]               AS [Booked Resource Id],
         rcs.msdyn_billingtype       AS [Booking Billing Type Code],
         BillingType.label           AS [Booking Billing Type]
  FROM   dbo.bookableresourcebooking bk
         LEFT OUTER JOIN dbo.salesorderdetail cl
                      ON cl.msdyn_project = bk.msdyn_projectid
                         AND cl.msdyn_includetime = 1
                         /* Have to filter by lines that include Time transactions, as only 1 Contract Line for Time transactions is allowed per Project */
                         AND cl.msdyn_project IS NOT NULL
         LEFT OUTER JOIN dbo.msdyn_orderlineresourcecategory rcs
                      ON cl.salesorderdetailid = rcs.msdyn_contractline
                         AND bk.msdyn_resourcecategoryid = rcs.msdyn_resourcecategory
                         AND rcs.msdyn_transactionclassification = 192350000 /* Time */
                         AND rcs.statecode = 0 /* Active */
                         AND ( rcs.msdyn_contractline IS NOT NULL )
         LEFT OUTER JOIN dbo.bookingtype_os_bookableresourcebooking BookingType
                      ON BookingType.[value] = bk.bookingtype
         LEFT OUTER JOIN dbo.msdyn_billingtype_gos BillingType
                      ON BillingType.[value] = rcs.msdyn_billingtype
  WHERE  ( Datediff(year, Getdate(), bk.endtime) >= -1 )
         AND ( Datediff(day, Getdate(), bk.starttime) <= 366 );
go

CREATE VIEW psa.ResourceRequestView
AS
  SELECT msdyn_name                      AS [Request Name],
         msdyn_resourcerequirementid     AS [Resource Requirement Id],
         msdyn_requestedby               AS [Requested By User Id],
         requestusr.fullname             AS [Requested By Name],
         msdyn_claimedby                 AS [Request Claimed By User Id],
         claimusr.fullname               AS [Request Claimed By Name],
         CONVERT(DATE, request.createdon)AS [Request Created On Date],
         statecode                       AS [Request State Code],
         StateCode.label                 AS [Request State],
         statuscode                      AS [Request Status Code],
         StatusCode.label                AS [Request Status]
  FROM   dbo.msdyn_resourcerequest request LEFT OUTER JOIN dbo.msdyn_resourcerequest_state StateCode ON StateCode.[value] = statecode
                                           LEFT OUTER JOIN dbo.msdyn_resourcerequest_status StatusCode ON StatusCode.[value] = statuscode
                                           LEFT OUTER JOIN dbo.systemuser requestusr ON msdyn_requestedby = requestusr.systemuserid
                                           LEFT OUTER JOIN dbo.systemuser claimusr ON msdyn_claimedby = claimusr.systemuserid
  WHERE  ( Datediff(year, Getdate(), request.createdon) >= -1 )
         AND ( Datediff(day, Getdate(), request.createdon) <= 366 );
go

CREATE VIEW psa.ResourceRequirementDetailView
AS
  SELECT msdyn_resourcerequirementid AS [Resource Requirement Id],
         CONVERT(DATE, msdyn_from)   AS [Required Date],
         msdyn_hours                 AS [Required Hours]
  FROM   dbo.msdyn_resourcerequirementdetail
  WHERE  ( Datediff(year, Getdate(), msdyn_from) >= -1 )
         AND ( Datediff(day, Getdate(), msdyn_from) <= 366 );
go

CREATE VIEW psa.ResourceRequirementView
AS
  SELECT msdyn_resourcerequirementid   AS [Resource Requirement Id],
         msdyn_projectid               AS [Required Project Id],
         msdyn_quantity                AS [Required Resource Count],
         msdyn_roleid                  AS [Required Resource Category Id],
         msdyn_hours                   AS [Required Hours (Total)],
         CONVERT(DATE, msdyn_fromdate) AS [Required Start Date],
         CONVERT(DATE, msdyn_todate)   AS [Required End Date],
         msdyn_costprice_base          AS [Required Cost Price],
         msdyn_country                 AS [Required Country],
         msdyn_stateorprovince         AS [Required State/Province],
         msdyn_city                    AS [Required City],
         msdyn_requeststatus           AS [Requirement Request Status],
         msdyn_type                    AS [Requirement Type Code],
         ReqType.label                 AS [Requirement Type],
         statecode                     AS [Requirement State Code],
         StateCode.label               AS [Requirement State]
  FROM   dbo.msdyn_resourcerequirement LEFT OUTER JOIN dbo.msdyn_type_os_msdyn_resourcerequirement ReqType ON ReqType.[value] = msdyn_type
                                       LEFT OUTER JOIN dbo.msdyn_resourcerequirement_state StateCode ON StateCode.[value] = statecode
  WHERE  ( Datediff(year, Getdate(), msdyn_todate) >= -1 )
         AND ( Datediff(day, Getdate(), msdyn_fromdate) <= 366 );
go

CREATE VIEW psa.ResourceView
AS
  SELECT br.bookableresourceid       AS [Resource Id],
         CASE
           WHEN br.msdyn_targetutilization IS NULL THEN rc.[resource category target utilization]
           ELSE br.msdyn_targetutilization * 0.01
         END                         AS [Resource Target Utilization],
         br.NAME                     AS [Resource Name],
         ResourceType.label          AS [Resource Type],
         br.resourcetype             AS [Resource Type Code],
         br.msdyn_organizationalunit AS [Resource Organizational Unit Id],
         StateCode.label             AS [Resource State],
         br.statecode                AS [Resource State Code],
         rc.[resource category id]   AS [Default ResourceCategory Id],
         rc.[resource category name] AS [Default Resource Category Name]
  FROM   dbo.bookableresource br LEFT OUTER JOIN dbo.bookableresourcecategoryassn brca ON br.bookableresourceid = brca.resource
                                 LEFT OUTER JOIN psa.defaultresourcecategoryview rc ON rc.[resource category id] = brca.resourcecategory
                                 LEFT OUTER JOIN dbo.resourcetype_os_bookableresource ResourceType ON ResourceType.[value] = br.resourcetype
                                 LEFT OUTER JOIN dbo.bookableresource_state StateCode ON StateCode.[value] = br.statecode
  WHERE  ( brca.statecode = 0 )
         AND ( brca.msdyn_isdefault = 1 )
         AND NOT (( ( br.statecode != 0 )
                    AND ( Datediff(year, Getdate(), br.modifiedon) < -1 ) ));
go

CREATE VIEW psa.TimeEntryView
AS
  SELECT msdyn_timeentryid         AS [TimeEntry Id],
         CONVERT(DATE, msdyn_date) AS [Time Entry Date],
         msdyn_bookableresource    AS [Resource Id],
         msdyn_description         AS [Time Entry Description],
         msdyn_duration            AS [Time Entry Minutes],
         msdyn_duration / 60.0     AS [Time Entry Hours],
         msdyn_entrystatus         AS [Time Entry Status Code],
         EntryStatus.label         AS [Time Entry Status],
         msdyn_manager             AS [Time Entry Approver User Id],
         systemuser.fullname       AS [Time Entry Approver],
         msdyn_project             AS [Project Id],
         msdyn_resourcecategory    AS [Time Entry Resource Category Id],
         EntryType.label           AS [Time Entry Type],
         msdyn_type                AS [Time Entry Type Code]
  FROM   dbo.msdyn_timeentry LEFT OUTER JOIN dbo.msdyn_type_gos EntryType ON EntryType.[value] = msdyn_type
                             LEFT OUTER JOIN dbo.msdyn_entrystatus_gos EntryStatus ON EntryStatus.[value] = msdyn_entrystatus
                             LEFT OUTER JOIN dbo.systemuser SystemUser ON msdyn_manager = systemuserid
  WHERE  ( Datediff(year, Getdate(), msdyn_date) >= -1 )
         AND ( Datediff(day, Getdate(), msdyn_date) <= 366 );
go

CREATE VIEW psa.TransactionCategoryView
AS
  SELECT msdyn_transactioncategoryid AS [TransactionCategory Id],
         msdyn_name                  AS [Transaction Category Name]
  FROM   dbo.msdyn_transactioncategory
  WHERE  NOT (( ( statecode != 0 )
                AND ( Datediff(year, Getdate(), modifiedon) < -1 ) ));
go

/*
  This view provides estimated maximum possible free capacity per resource per day
  according to weekday capacity setup and resource's approved absence.
*/
CREATE VIEW psa.ZResourceCapacityView
AS
  SELECT [resource id],
         [time entry date]     AS [Resource Capacity Date],
         -[time entry minutes] AS [Resource Capacity Minutes],
         -[time entry hours]   AS [Resource Capacity Hours]
  FROM   psa.timeentryview
  WHERE  [time entry type code] <> 192350000 /*Work*/
         AND [time entry status code] = 192350002 /*Approved*/
  UNION ALL
  SELECT [resource id],
         [capacity date]    AS [Resource Capacity Date],
         [capacity minutes] AS [Resource Capacity Minutes],
         [capacity hours]   AS [Resource Capacity Hours]
  FROM   psa.namedresourceworkcapacityview;
go
