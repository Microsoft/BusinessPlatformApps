@isTest
global class MockHttpResponseGenerator implements HttpCalloutMock {
    // Implement this interface method
    global HTTPResponse respond(HTTPRequest req) {
        // Create a fake response
        HttpResponse res = new HttpResponse();
        
        res.setHeader('Content-Type', 'application/json');
        res.setBody('{"token_type":"Bearer","scope":"Content.Create Dashboard.Read.All Data.Alter_Any Dataset.Read.All Dataset.ReadWrite.All Group.Read Group.Read.All Metadata.View_Any Report.Read.All","expires_in":"3599","ext_expires_in":"0","expires_on":"1482242395","not_before":"1482238495","resource":"https://analysis.windows.net/powerbi/api","access_token":"accessCookieToken","refresh_token":"refreshCookieToken","id_token":"idToken"}');
        res.setStatusCode(200);
        return res;
    }
}