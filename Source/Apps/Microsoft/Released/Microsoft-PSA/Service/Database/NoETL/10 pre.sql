SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Must be executed inside the target database
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ZResourceCapacityView' AND TABLE_TYPE='View')
    DROP VIEW psa.ZResourceCapacityView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='TransactionCategoryView' AND TABLE_TYPE='View')
    DROP VIEW psa.TransactionCategoryView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='TimeEntryView' AND TABLE_TYPE='View')
    DROP VIEW psa.TimeEntryView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ResourceView' AND TABLE_TYPE='View')
    DROP VIEW psa.ResourceView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ResourceRequirementView' AND TABLE_TYPE='View')
    DROP VIEW psa.ResourceRequirementView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ResourceRequirementDetailView' AND TABLE_TYPE='View')
    DROP VIEW psa.ResourceRequirementDetailView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ResourceRequestView' AND TABLE_TYPE='View')
    DROP VIEW psa.ResourceRequestView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ResourceBookingView' AND TABLE_TYPE='View')
    DROP VIEW psa.ResourceBookingView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='QuoteView' AND TABLE_TYPE='View')
    DROP VIEW psa.QuoteView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='QuoteLineView' AND TABLE_TYPE='View')
    DROP VIEW psa.QuoteLineView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ProjectView' AND TABLE_TYPE='View')
    DROP VIEW psa.ProjectView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ProjectContractView' AND TABLE_TYPE='View')
    DROP VIEW psa.ProjectContractView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='OrganizationalUnitView' AND TABLE_TYPE='View')
    DROP VIEW psa.OrganizationalUnitView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='OpportunityView' AND TABLE_TYPE='View')
    DROP VIEW psa.OpportunityView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='NamedResourceWorkCapacityView' AND TABLE_TYPE='View')
    DROP VIEW psa.NamedResourceWorkCapacityView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='MeasuresView' AND TABLE_TYPE='View')
    DROP VIEW psa.MeasuresView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='DefaultResourceCategoryView' AND TABLE_TYPE='View')
    DROP VIEW psa.DefaultResourceCategoryView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='DateView' AND TABLE_TYPE='View')
    DROP VIEW psa.DateView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='DateCapacityView' AND TABLE_TYPE='View')
    DROP VIEW psa.DateCapacityView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='CustomerView' AND TABLE_TYPE='View')
    DROP VIEW psa.CustomerView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ContractView' AND TABLE_TYPE='View')
    DROP VIEW psa.ContractView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='ContractLineView' AND TABLE_TYPE='View')
    DROP VIEW psa.ContractLineView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='BusinessTransactionView' AND TABLE_TYPE='View')
    DROP VIEW psa.BusinessTransactionView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='BookingStatusView' AND TABLE_TYPE='View')
    DROP VIEW psa.BookingStatusView;


--- DROP PSA TABLES---
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='WeekDayCapacity' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE psa.WeekDayCapacity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='Date' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE psa.[Date];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='psa' AND TABLE_NAME='configuration' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE psa.[configuration];

--- DROP DBO VIEWS---
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='timeline_os_opportunity' AND TABLE_TYPE='View')
    DROP VIEW dbo.timeline_os_opportunity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='territorycode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.territorycode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='status_os_bookingstatus' AND TABLE_TYPE='View')
    DROP VIEW dbo.status_os_bookingstatus;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='shipto_freighttermscode_os_salesorderdetail' AND TABLE_TYPE='View')
    DROP VIEW dbo.shipto_freighttermscode_os_salesorderdetail;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='shipto_freighttermscode_os_salesorder' AND TABLE_TYPE='View')
    DROP VIEW dbo.shipto_freighttermscode_os_salesorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='shipto_freighttermscode_os_quotedetail' AND TABLE_TYPE='View')
    DROP VIEW dbo.shipto_freighttermscode_os_quotedetail;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='shipto_freighttermscode_os_quote' AND TABLE_TYPE='View')
    DROP VIEW dbo.shipto_freighttermscode_os_quote;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='shippingmethodcode_os_salesorder' AND TABLE_TYPE='View')
    DROP VIEW dbo.shippingmethodcode_os_salesorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='shippingmethodcode_os_quote' AND TABLE_TYPE='View')
    DROP VIEW dbo.shippingmethodcode_os_quote;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='shippingmethodcode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.shippingmethodcode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='salesstage_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.salesstage_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='salesstagecode_os_opportunity' AND TABLE_TYPE='View')
    DROP VIEW dbo.salesstagecode_os_opportunity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='salesorder_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.salesorder_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='salesorder_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.salesorder_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='salesorderstatecode_os_salesorderdetail' AND TABLE_TYPE='View')
    DROP VIEW dbo.salesorderstatecode_os_salesorderdetail;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='resourcetype_os_bookableresource' AND TABLE_TYPE='View')
    DROP VIEW dbo.resourcetype_os_bookableresource;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='quote_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.quote_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='quote_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.quote_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='quotestatecode_os_quotedetail' AND TABLE_TYPE='View')
    DROP VIEW dbo.quotestatecode_os_quotedetail;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='quoteclose_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.quoteclose_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='quoteclose_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.quoteclose_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='purchasetimeframe_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.purchasetimeframe_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='purchaseprocess_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.purchaseprocess_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='propertyconfigurationstatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.propertyconfigurationstatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='producttypecode_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.producttypecode_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='prioritycode_os_salesorder' AND TABLE_TYPE='View')
    DROP VIEW dbo.prioritycode_os_salesorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='prioritycode_os_opportunity' AND TABLE_TYPE='View')
    DROP VIEW dbo.prioritycode_os_opportunity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='pricingerrorcode_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.pricingerrorcode_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='preferredcontactmethodcode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.preferredcontactmethodcode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='preferredappointmenttimecode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.preferredappointmenttimecode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='preferredappointmentdaycode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.preferredappointmentdaycode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='paymenttermscode_os_salesorder' AND TABLE_TYPE='View')
    DROP VIEW dbo.paymenttermscode_os_salesorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='paymenttermscode_os_quote' AND TABLE_TYPE='View')
    DROP VIEW dbo.paymenttermscode_os_quote;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='paymenttermscode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.paymenttermscode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='ownershipcode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.ownershipcode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunity_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.opportunity_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunity_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.opportunity_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunityratingcode_os_opportunity' AND TABLE_TYPE='View')
    DROP VIEW dbo.opportunityratingcode_os_opportunity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunityclose_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.opportunityclose_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunityclose_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.opportunityclose_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='need_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.need_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_vendortype_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_vendortype_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_type_os_msdyn_resourcerequirement' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_type_os_msdyn_resourcerequirement;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_type_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_type_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_transactiontypecode_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_transactiontypecode_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_transactionclassification_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_transactionclassification_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_timeoffcalendar_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_timeoffcalendar_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_timeoffcalendar_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_timeoffcalendar_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_timeentry_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_timeentry_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_timeentry_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_timeentry_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_targetexpensestatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_targetexpensestatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_targetentrystatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_targetentrystatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_scheduleperformance_os_msdyn_project' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_scheduleperformance_os_msdyn_project;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequirement_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_resourcerequirement_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequirement_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_resourcerequirement_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequirementdetail_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_resourcerequirementdetail_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequirementdetail_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_resourcerequirementdetail_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequest_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_resourcerequest_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequest_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_resourcerequest_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_relateditemtype_os_msdyn_timeentry' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_relateditemtype_os_msdyn_timeentry;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_receiptrequired_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_receiptrequired_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_psastatusreason_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_psastatusreason_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_psastate_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_psastate_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_project_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_project_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_project_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_project_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_projectteam_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_projectteam_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_projectteam_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_projectteam_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_profitability_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_profitability_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_overallprojectstatus_os_msdyn_project' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_overallprojectstatus_os_msdyn_project;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_organizationalunit_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_organizationalunit_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_organizationalunit_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_organizationalunit_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_ordertype_os_salesorder' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_ordertype_os_salesorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_ordertype_os_quote' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_ordertype_os_quote;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_ordertype_os_opportunity' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_ordertype_os_opportunity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_orderlineresourcecategory_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_orderlineresourcecategory_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_orderlineresourcecategory_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_orderlineresourcecategory_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_membershipstatus_os_msdyn_projectteam' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_membershipstatus_os_msdyn_projectteam;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_feasible_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_feasible_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_expense_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_expense_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_expense_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_expense_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_expensetype_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_expensetype_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_expensestatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_expensestatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_expensecategory_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_expensecategory_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_expensecategory_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_expensecategory_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_estimateline_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_estimateline_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_estimateline_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_estimateline_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_estimatedschedule_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_estimatedschedule_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_estimatedbudget_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_estimatedbudget_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_entrystatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_entrystatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_customertype_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_customertype_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_costperformence_os_msdyn_project' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_costperformence_os_msdyn_project;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_competitive_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_competitive_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_committype_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_committype_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_bulkgenerationstatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_bulkgenerationstatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_billingtype_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_billingtype_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_billingstatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_billingstatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_billingmethod_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_billingmethod_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_amountmethod_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_amountmethod_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_allocationmethod_os_msdyn_resourcerequirement' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_allocationmethod_os_msdyn_resourcerequirement;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_allocationmethod_os_msdyn_projectteam' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_allocationmethod_os_msdyn_projectteam;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_adjustmentstatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_adjustmentstatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_actual_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_actual_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_actual_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.msdyn_actual_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='initialcommunication_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.initialcommunication_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='industrycode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.industrycode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='freighttermscode_os_salesorder' AND TABLE_TYPE='View')
    DROP VIEW dbo.freighttermscode_os_salesorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='freighttermscode_os_quote' AND TABLE_TYPE='View')
    DROP VIEW dbo.freighttermscode_os_quote;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='customertypecode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.customertypecode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='customersizecode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.customersizecode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='businesstypecode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.businesstypecode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='budgetstatus_gos' AND TABLE_TYPE='View')
    DROP VIEW dbo.budgetstatus_gos;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookingtype_os_bookableresourcebooking' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookingtype_os_bookableresourcebooking;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookingstatus_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookingstatus_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookingstatus_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookingstatus_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresource_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresource_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresource_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresource_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcecategory_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresourcecategory_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcecategory_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresourcecategory_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcecategoryassn_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresourcecategoryassn_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcecategoryassn_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresourcecategoryassn_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcebooking_status' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresourcebooking_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcebooking_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.bookableresourcebooking_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='address2_shippingmethodcode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.address2_shippingmethodcode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='address2_freighttermscode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.address2_freighttermscode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='address2_addresstypecode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.address2_addresstypecode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='address1_shippingmethodcode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.address1_shippingmethodcode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='address1_freighttermscode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.address1_freighttermscode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='address1_addresstypecode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.address1_addresstypecode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='account_state' AND TABLE_TYPE='View')
    DROP VIEW dbo.account_state;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='accountratingcode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.accountratingcode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='accountclassificationcode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.accountclassificationcode_os_account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='accountcategorycode_os_account' AND TABLE_TYPE='View')
    DROP VIEW dbo.accountcategorycode_os_account;


--- DROP DBO TABLES---
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='systemuser' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.systemuser;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='StatusMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.StatusMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='StateMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.StateMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='salesorderdetail' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.salesorderdetail;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='salesorder' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.salesorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='quotedetail' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.quotedetail;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='quote' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.quote;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='OptionSetMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.OptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunity' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.opportunity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_transactioncategory' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_transactioncategory;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_timeentry' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_timeentry;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequirementdetail' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_resourcerequirementdetail;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequirement' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_resourcerequirement;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_resourcerequest' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_resourcerequest;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_project' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_project;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_organizationalunit' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_organizationalunit;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_orderlineresourcecategory' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_orderlineresourcecategory;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_estimateline' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_estimateline;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_actual' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_actual;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GlobalOptionSetMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.GlobalOptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookingstatus' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookingstatus;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcecategoryassn' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookableresourcecategoryassn;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcecategory' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookableresourcecategory;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcebooking' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookableresourcebooking;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresource' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookableresource;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='account' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.account;


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='psa' AND ROUTINE_NAME='sp_get_replication_counts' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE psa.sp_get_replication_counts;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='psa' AND ROUTINE_NAME='sp_get_prior_content' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE psa.sp_get_prior_content;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='psa' AND ROUTINE_NAME='sp_get_last_updatetime' AND ROUTINE_TYPE='PROCEDURE')
   DROP PROCEDURE psa.sp_get_last_updatetime;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertAttributeMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertAttributeMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertGlobalOptionSetMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertGlobalOptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertOptionSetMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertOptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertStateMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertStateMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertStatusMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertStatusMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertTargetMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertTargetMetadata;



IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='psa')
BEGIN
    EXEC ('CREATE SCHEMA psa AUTHORIZATION dbo'); -- Avoid batch error
END;