import { AzureLogin } from './azure-login';
import { DataStoreType } from '../services/datastore';
import { ActionResponse } from '../services/actionresponse';

export class MsCrmLogin extends AzureLogin {
    entities: string = '';
    hasToken: boolean = false;
    msCrmOrganizationId: string = '';
    msCrmOrganizations: MsCrmOrganization[] = [];

    constructor() {
        super();
    }

    async OnLoaded() {
        this.isValidated = false;
        this.showValidation = false;
        if (this.subscriptionsList.length > 0) {
            this.isValidated = true;
            this.showValidation = true;
        } else {
            let queryParam = this.MS.UtilityService.GetItem('queryUrl');
            if (queryParam) {
                let token = this.MS.UtilityService.GetQueryParameterFromUrl('code', queryParam);
                var tokenObj = {
                    code: token
                };
                this.authToken = await this.MS.HttpService.executeAsync('Microsoft-GetAzureToken', tokenObj);
                if (this.authToken.IsSuccess) {
                    this.hasToken = true;

                    this.MS.DataStore.addToDataStore('MsCrmToken', this.authToken.Body.AzureToken.access_token, DataStoreType.Private);

                    var response = await this.MS.HttpService.executeAsync('Microsoft-CrmGetOrgs', {});
                    if (response.IsSuccess) {
                        this.msCrmOrganizations = JSON.parse(response.Body.value);
                        if (this.msCrmOrganizations && this.msCrmOrganizations.length > 0) {
                            this.msCrmOrganizationId = this.msCrmOrganizations[0].OrganizationId;
                            this.isValidated = true;
                        } else {
                            this.MS.ErrorService.message = 'No Dynamics CRM Organizations Found.';
                        }
                    }
                }
                this.MS.UtilityService.RemoveItem('queryUrl');
            }
        }
    }

    async connect() {
        this.MS.DataStore.addToDataStore('AADTenant', 'common', DataStoreType.Public);
        this.MS.DataStore.addToDataStore('IsMsCrm', true, DataStoreType.Public);
        let response: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureAuthUri', {});
        window.location.href = response.Body.value;
    }

    public async NavigatingNext(): Promise<boolean> {
        let msCrmOrganization: MsCrmOrganization = null;

        for (let i = 0; i < this.msCrmOrganizations.length && msCrmOrganization === null; i++) {
            if (this.msCrmOrganizations[i].OrganizationId === this.msCrmOrganizationId) {
                msCrmOrganization = this.msCrmOrganizations[i];
            }
        }

        if (msCrmOrganization) {
            this.MS.DataStore.addToDataStore('ConnectorUrl', msCrmOrganization.ConnectorUrl, DataStoreType.Private);
            this.MS.DataStore.addToDataStore('Entities', this.entities, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('OrganizationId', msCrmOrganization.OrganizationId, DataStoreType.Private);
            this.MS.DataStore.addToDataStore('OrganizationUrl', msCrmOrganization.OrganizationUrl, DataStoreType.Private);
            return true;
        } else {
            return false;
        }
    }
}

class MsCrmOrganization {
    ConnectorUrl: string;
    OrganizationId: string;
    OrganizationName: string;
    OrganizationUrl: string;
}