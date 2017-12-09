SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_apimerrordetail' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_apimerrordetail;                                                        
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_date' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_date;                                                                  
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_apisummary' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_apisummary;                                                               
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_operationsummary' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_operationsummary;                                                         
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_productsummary' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_productsummary;                                                           
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_subscriptionsummary' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_subscriptionsummary;                                                      
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_allrequestdata' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_allrequestdata;                                                           
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_visualoperationcallvolume' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_visualoperationcallvolume;                                                
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_visualcallprobabilityedgelist' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_visualcallprobabilityedgelist;                                            
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_visualipedgecounts' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_visualipedgecounts;                                                       
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_visualfreqhour' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_visualfreqhour;                                                           
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_visualfreqminute' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_visualfreqminute;                                                         
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_visualfreqsecond' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_visualfreqsecond;                                                         
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_ipaddresssummary' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_ipaddresssummary;                                                         
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='vw_subscriptionipaddress' AND TABLE_TYPE='VIEW')
    DROP VIEW pbist_apimgmt.vw_subscriptionipaddress;

-- Tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='configuration' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.[configuration];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='request' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.request;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='response' and table_type='BASE TABLE')
    drop table pbist_apimgmt.response;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='error' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.error;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='callextendededgelist' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.callextendededgelist;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='callextendededgelist_staging' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.callextendededgelist_staging;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='callprobabilityedgelist' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.callprobabilityedgelist;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='callprobabilityedgelist_staging' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.callprobabilityedgelist_staging;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='fft' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.fft;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='fft_staging' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.fft_staging;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='date' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.[date];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='geolite2-city-blocks-ipv4' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.[geolite2-city-blocks-ipv4];

-- Stored procedures
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_edgetablesswap' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE pbist_apimgmt.sp_edgetablesswap;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_ffttableswap' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE pbist_apimgmt.sp_ffttableswap;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_getdistinctipaddressesinwindow' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE pbist_apimgmt.sp_getdistinctipaddressesinwindow;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_getrequestsbyipaddressinwindow' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE pbist_apimgmt.sp_getrequestsbyipaddressinwindow;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_fftdataextraction' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE pbist_apimgmt.sp_fftdataextraction;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_ProcessIPAddressLocations' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE pbist_apimgmt.sp_ProcessIPAddressLocations;

-- Functions
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='fn_IPtoBigInt' AND ROUTINE_TYPE='FUNCTION')
    DROP FUNCTION pbist_apimgmt.fn_IPtoBigInt;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='fn_IsIpaddressInSubnetShortHand' AND ROUTINE_TYPE='FUNCTION')
    DROP FUNCTION pbist_apimgmt.fn_IsIpaddressInSubnetShortHand;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='fn_IsIpaddressInSubnet' AND ROUTINE_TYPE='FUNCTION')
    DROP FUNCTION pbist_apimgmt.fn_IsIpaddressInSubnet;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='fn_SubnetBitstoBigInt' AND ROUTINE_TYPE='FUNCTION')
    DROP FUNCTION pbist_apimgmt.fn_SubnetBitstoBigInt;

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='pbist_apimgmt')
BEGIN
    EXEC ('CREATE SCHEMA pbist_apimgmt AUTHORIZATION dbo'); -- Avoid batch error
END;
