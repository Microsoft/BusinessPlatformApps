public class PowerBIController extends OAuthController {
    @TestVisible private static String APPLICATION_NAME = 'PowerBI';

    public PowerBIController () {
        this.application_name = APPLICATION_NAME;
    }
    
    public String getValidateResult()
    {
        return validateResult;
    }

    /**
    * Validates the callback code and generates the access and refresh tokens
    *
    * @return null to refresh the page
    */
    public PageReference redirectOnCallback() {
        return super.redirectOnCallback(null);
    }
    
    public PageReference refreshAccessToken() {
        return super.refreshAccessToken(ApexPages.currentPage());
    }
}