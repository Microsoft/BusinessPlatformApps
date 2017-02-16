import { ActionResponse } from '../services/actionresponse';
import { DataStoreType } from '../services/datastore';
import { ViewModelBase } from '../services/viewmodelbase';

export class Salesforce extends ViewModelBase {
    salesforceUsername: string = '';
    salesforcePassword: string = '';
    salesforceToken: string = '';
    salesforceUrl: string = '';
    salesforceObjects: string = '';

    constructor() {
        super();
        this.isValidated = false;
        this.showValidation = false;
        this.salesforceUrl = 'login.salesforce.com';
    }

    async OnLoaded() {
        this.isValidated = false;
        this.showValidation = false;
    }

    async OnValidate(): Promise<boolean> {
        this.MS.DataStore.addToDataStore('SalesforceUser', this.salesforceUsername, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SalesforcePassword', this.salesforcePassword, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('SalesforceToken', this.salesforceToken, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('SalesforceUrl', this.salesforceUrl, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('ObjectTables', this.salesforceObjects, DataStoreType.Public);

        let salesforceLoginResponse = await this.MS.HttpService.executeAsync('Microsoft-ValidateSalesforceCredentials', {});

        if (!salesforceLoginResponse.IsSuccess) {
            return false;
        }

        if (!super.OnValidate()) {
            return false;
        }

        this.isValidated = true;
        this.showValidation = true;
        this.MS.DataStore.addToDataStore('SalesforceBaseUrl', salesforceLoginResponse.Body.serverUrlField, DataStoreType.Public);

        return true;
    }
}