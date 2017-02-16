/**
    *
    OAuth controller borrow heavily from http://blog.deadlypenguin.com/blog/2016/07/05/oauth-flow-for-service-users-in-salesforce/
    */
public virtual class OAuthController {
    @TestVisible public String access_token;
    @TestVisible public String refresh_token;
    @TestVisible public String expires_on;
    
    @TestVisible private Boolean isCallback;

    public String validateResult;

    public String application_name;
   
    public String PBIaccess_token { 
        get {
            Cookie pbi_AccessToken = ApexPages.currentPage().getCookies().get('pbi_AccessToken');
            if(pbi_AccessToken == null)
                this.access_token = '';
            else
                this.access_token = pbi_AccessToken.getValue();
            
            return this.access_token;
        } 
        set;
        }
        
    public String PBIrefresh_token { 
        get {
            Cookie pbi_RefreshToken = ApexPages.currentPage().getCookies().get('pbi_RefreshToken');
            if(pbi_RefreshToken == null)
                this.refresh_token= '';
            else
                this.refresh_token= pbi_RefreshToken.getValue();
            
            return this.refresh_token;
        } 
        set;
        }
        
    public String PBIexpires_on { 
        get {
            Cookie pbi_ExpiresOn= ApexPages.currentPage().getCookies().get('pbi_ExpiresOn');
            if(pbi_ExpiresOn== null)
                this.expires_on= '';
            else
                this.expires_on= pbi_ExpiresOn.getValue();
            
            return this.expires_on;
        } 
        set;
        }

    /**
    * If the access token is set
    *
    * @return If the access token is set
    */
    public Boolean getHasToken() {
          Boolean check;
         if(PBIaccess_token== null)
              check = false;
           else 
              check = PBIaccess_token.length()>0;
          return check;
    }


    /** The JSON result from a successful oauth call */
    public class OAuthResult {
        /** The access token */
        public String access_token {get; set;}

        /** The refresh token */
        public String refresh_token {get; set;}
        
        /** The token expiry date*/
        public String expires_on {get;set;}
    }

    /**
    * Validates the oauth code
    *
    * @param code The code to validate
    * @param redirect_uri The URL to redirect to after successful validation
    * @return The oauth result
    */
    public OAuthResult validateCode(String code, String redirect_uri) {

        String client_id = OAuthApp_pbi__c.getValues(this.application_name).Client_Id__c;
        String client_secret = OAuthApp_pbi__c.getValues(this.application_name).Client_Secret__c;
        String access_token_url = OAuthApp_pbi__c.getValues(this.application_name).Access_Token_URL__c;
        String resource_URI = OAuthApp_pbi__c.getValues(this.application_name).Resource_URI__c;

        List<String> urlParams = new List<String> {
            'grant_type=authorization_code',
            'code=' + EncodingUtil.urlEncode(code, 'UTF-8'),
            'client_id=' + EncodingUtil.urlEncode(client_id, 'UTF-8'),
            'client_secret=' + EncodingUtil.urlEncode(client_secret, 'UTF-8'),
            'redirect_uri=' + EncodingUtil.urlEncode(redirect_uri, 'UTF-8')
        };

        Http h = new Http();

        HttpRequest req = new HttpRequest();
        req.setEndpoint(access_token_url);
        req.setMethod('POST');
        req.setHeader('Content-Type', 'application/x-www-form-urlencoded');
        req.setHeader('Accept', 'application/json');
        String body = String.join(urlParams, '&');
        req.setBody(body);

        HttpResponse res = h.send(req);
        validateResult = res.getBody();
        return (OAuthResult)(JSON.deserialize(res.getBody(), OAuthResult.class));
    }
    
    /** 
    * Gets new access token through refresh token
    *
    * @param refreshToken The refresh token
    * @return The oauth result
    */
    public PageReference refreshAccessToken(PageReference location){
        Cookie accessToken = new Cookie('pbi_AccessToken', '',null,0,false);
        Cookie expiresOn = new Cookie('pbi_ExpiresOn','',null,0,false);
            
        ApexPages.currentPage().setCookies(new Cookie[]{accessToken,expiresOn}); 
            
        String client_id = OAuthApp_pbi__c.getValues(this.application_name).Client_Id__c;
        String client_secret = OAuthApp_pbi__c.getValues(this.application_name).Client_Secret__c;
        String resource_URI = OAuthApp_pbi__c.getValues(this.application_name).Resource_URI__c;
        String refreshToken = this.PBIrefresh_token;
        String access_token_url = OAuthApp_pbi__c.getValues(this.application_name).Access_Token_URL__c;
        
        List<String> urlParams = new List<String> {
            'grant_type=refresh_token',
            'refresh_token=' + EncodingUtil.urlEncode(refreshToken, 'UTF-8'),
            'client_id=' + EncodingUtil.urlEncode(client_id, 'UTF-8'),
            'client_secret=' + EncodingUtil.urlEncode(client_secret, 'UTF-8'),
            'resource=' + EncodingUtil.urlEncode(resource_URI, 'UTF-8')
        };
        
        Http h = new Http();

        HttpRequest req = new HttpRequest();
        req.setEndpoint(access_token_url);
        req.setMethod('POST');
        req.setHeader('Content-Type', 'application/x-www-form-urlencoded');
        req.setHeader('Accept', 'application/json');
        String body = String.join(urlParams, '&');
        req.setBody(body);

        HttpResponse res = h.send(req);

        OAuthResult result = (OAuthResult)(JSON.deserialize(res.getBody(), OAuthResult.class));
        
        Cookie refreshTokenCookie = new Cookie('pbi_RefreshToken', result.refresh_token,null,-1,false);        
        accessToken = new Cookie('pbi_AccessToken', result.access_token,null,-1,false);
        expiresOn = new Cookie('pbi_ExpiresOn',result.expires_on,null,-1,false);
        
        ApexPages.currentPage().setCookies(new Cookie[]{accessToken,refreshTokenCookie,expiresOn});   
            
        return location;
    }

    /**
    * Generic constructor
    */
    public OAuthController() {
    this.isCallback = ApexPages.currentPage().getParameters().containsKey('code');              

        if (OAuthApp_pbi__c.getValues(this.application_name) != null) {
            Cookie pbi_AccessToken = ApexPages.currentPage().getCookies().get('pbi_AccessToken');
            Cookie pbi_RefreshToken = ApexPages.currentPage().getCookies().get('pbi_RefreshToken');
            Cookie pbi_ExpiresOn = ApexPages.currentPage().getCookies().get('pbi_ExpiresOn');
         
         if(pbi_AccessToken == null)
             this.access_token = '';
         else
            this.access_token = pbi_AccessToken.getValue();
            
         if(pbi_RefreshToken == null)
             this.refresh_token= '';
         else
            this.refresh_token = pbi_RefreshToken.getValue();
            
         if(pbi_ExpiresOn == null)
             this.expires_on = '';
         else
             this.expires_on = pbi_ExpiresOn.getValue();
        }
    }

    /**
    * Gets the authroization URL
    *
    * @return The authorization url
    */
    public String getAuthUrl() {
        Map<String, String> urlParams = new Map<String, String> {
            'client_id' => OAuthApp_pbi__c.getValues(this.application_name).Client_Id__c,
            'redirect_uri' => getPageUrl(),
            'resource' => OAuthApp_pbi__c.getValues(this.application_name).Resource_URI__c,
            'response_type' => 'code'
        };
        String auth_url = OAuthApp_pbi__c.getValues(this.application_name).Authorization_URL__c;

        PageReference ref = new PageReference(auth_url);
        ref.getParameters().putAll(urlParams);

        return ref.getUrl();
    }

    /**
    * Gets the page url
    *
    * @return The page url
    */
    @testVisible
    private String getPageUrl() {
        String host = ApexPages.currentPage().getHeaders().get('Host');
        String path = ApexPages.currentPage().getUrl().split('\\?').get(0);

        return 'https://' + host + path;
    }

    /**
    * Validates the callback code and generates the access and refresh tokens
    *
    * @param location Where to redirect to after success
    * @return null to refresh the page
    */
       public PageReference redirectOnCallback(PageReference location) {
        if (this.isCallback)  {
            String code = ApexPages.currentPage().getParameters().get('code');
            OAuthResult result = validateCode(code, this.getPageUrl());
                                   
            //Store accesstoken in cookie
            Cookie accessToken = new Cookie('pbi_AccessToken', result.access_token,null,-1,false);
            Cookie refreshToken = new Cookie('pbi_RefreshToken', result.refresh_token,null,-1,false);
            Cookie expiresOn = new Cookie('pbi_ExpiresOn',result.expires_on,null,-1,false);
            
            ApexPages.currentPage().setCookies(new Cookie[]{accessToken,refreshToken,expiresOn}); 

            return location;
        }
        return null;
    }
}