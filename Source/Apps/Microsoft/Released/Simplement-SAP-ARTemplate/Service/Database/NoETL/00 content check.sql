SELECT Count(*) AS ExistingObjectCount
FROM   INFORMATION_SCHEMA.TABLES
WHERE  ( table_schema = 'sap' AND
            table_name IN ('AccountGroup', 'ARClearedLineItem', 'ARLineItem', 'Client', 'Company', 'CurrencyCode', 'CurrencyCodeName', 'Customer', 'CustomerAccountGroupName',
                        'DocumentType', 'DocumentTypeName', 'ExchangeRate', 'GLAccount', 'GLAccountName', 'PaymentTerm', 'PaymentTermName', 'ProfitCenter', 'ProfitCenterName', 'SpecialGLIndicator',
                        'SpecialGLIndicatorName', 'configuration', 'sap_replicationstatus')
        );