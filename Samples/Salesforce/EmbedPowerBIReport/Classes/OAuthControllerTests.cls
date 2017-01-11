@isTest
public class OAuthControllerTests {
	
	public static PageReference pageRef = Page.Report;
	public static OAuthApp_pbi__c app;
	public static OAuthController controller = new OAuthController();
	
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
		
		controller.application_name = 'PowerBI2';
	}
	
	public static testMethod void createController()
	{		
		System.assertNotEquals(controller, null);		
	}   
	
	public static testMethod void checkAccessTokenNotNull()
	{
		System.assertNotEquals(controller.PBIAccess_token, null);	
	}
	
	public static testMethod void checkRefreshTokenNotNull()
	{
		System.assertNotEquals(controller.PBIRefresh_token, null);	
	}
	
	public static testMethod void checkExpiresOnNotNull()
	{
		System.assertNotEquals(controller.PBIExpires_on, null);	
	}
	
	public static testMethod void checkHasTokenReturnsFalse()
	{
		System.assertEquals(controller.getHasToken(), false);	
	}
	
	public static testMethod void checkHasTokenReturnsTrue()
	{
		controller.PBIAccess_token =  'testToken';
		System.assertEquals(controller.getHasToken(), false);	
	}
	
	public static testMethod void getAuthURLReturnSuccess()
	{
		Test.setCurrentPage(pageRef);
		String authUrl = controller.getAuthUrl();
		
		System.assertEquals(authUrl.contains('https://login.windows.net/common/oauth2/authorize?'), true);
	}
	
	public static testMethod void redirectOnCallbackCreatesCookies()
	{	
		Test.startTest();
		Test.setMock(HttpCalloutMock.class, new MockHttpResponseGenerator());
		
		OAuthControllerTests.getAuthURLReturnSuccess();
		pageRef = new PageReference('https://c.eu11.visual.force.com/apex/Report?code=testCode');
		Test.setCurrentPage(pageRef);
		controller.isCallback = true;

		PageReference ref = controller.redirectOnCallback(pageRef);
		
		String accessCookie = controller.PBIAccess_token;
		String refreshCookie =  controller.PBIRefresh_token;
		
		System.assertEquals('accessCookieToken',accessCookie);
		System.assertEquals('refreshCookieToken',refreshCookie);
		Test.stopTest();
	}

    public static testMethod void refreshToken()
	{	
		Test.startTest();
		Test.setMock(HttpCalloutMock.class, new MockHttpResponseGenerator());
		
		OAuthControllerTests.getAuthURLReturnSuccess();
		pageRef = new PageReference('https://c.eu11.visual.force.com/apex/Report?code=testCode');
		Test.setCurrentPage(pageRef);
		controller.isCallback = true;

		controller.refreshAccessToken(pageRef);
		
		String accessCookie = controller.PBIAccess_token;
		String refreshCookie =  controller.PBIRefresh_token;
		
		System.assertEquals('accessCookieToken',accessCookie);
		System.assertEquals('refreshCookieToken',refreshCookie);
		Test.stopTest();
	}
}