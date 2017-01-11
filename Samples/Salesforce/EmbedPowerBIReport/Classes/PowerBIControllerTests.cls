@isTest
public class PowerBIControllerTests {	
	
	public static PageReference pageRef = Page.Report;
	public static OAuthApp_pbi__c app;
	public static PowerBIController pbicontroller = new PowerBIController();
		
	@testSetup public static void setUp()
	{	
		app = new OAuthApp_pbi__c();
		app.Name = 'PowerBI2';
		app.Token_Expires_On__c = '0';
		app.Client_Id__c = 'clientId';
		app.Client_Secret__c = 'clientSecret';
		app.Authorization_URL__c = 'https://login.windows.net/common/oauth2/authorize';
		app.Access_Token_URL__c = 'https://login.microsoftonline.com/common/oauth2/token';
		app.Resource_URI__c = 'https://analysis.windows.net/powerbi/api';
		insert app;

		pbicontroller.application_name = 'PowerBI2';
	}
		
	public static testMethod void powerBiControllerNotNull()
	{
		System.assertNotEquals(pbicontroller, null);
	}
	
	public static testMethod void getValidateResultReturnsNotNull()
	{
		pbicontroller.validateResult = 'testResult';
		String validate = pbicontroller.getValidateResult();
		System.assertEquals('testResult', pbicontroller.getValidateResult());		
	}
	
	public static testMethod void callRedirect()
	{
		PageReference page = pbicontroller.redirectOnCallback();		
	}
	
	public static testMethod void callRefreshToken()
	{	Test.startTest();
		Test.setMock(HttpCalloutMock.class, new MockHttpResponseGenerator());
		
		pbicontroller.application_name = 'PowerBI2';
		Test.setCurrentPage(pageRef);		

		PageReference page = pbicontroller.refreshAccessToken();
				
		String accessCookie = pbicontroller.PBIAccess_token;
		String refreshCookie =  pbicontroller.PBIRefresh_token;
		
		System.assertEquals('accessCookieToken',accessCookie);
		System.assertEquals('refreshCookieToken',refreshCookie);	
		Test.stopTest();
	}
}