import { QueryParameter } from '../base/query-parameter';

import { AzureLogin } from './azure-login';
import { ActionResponse } from '../services/actionresponse';
import { DataStoreType } from '../services/datastore';

export class MsCrmLogin extends AzureLogin {
    entities: string = '';
    msCrmOrganizationId: string = '';
    msCrmOrganizations: MsCrmOrganization[] = [];
    showAzureTrial: boolean = false;

    constructor() {
        super();
    }

    async OnLoaded() {
        this.MS.ErrorService.Clear();

        this.isValidated = false;
        this.showAzureTrial = false;
        this.showValidation = false;

        if (this.subscriptionsList.length > 0 && this.msCrmOrganizations.length > 0) {
            this.isValidated = true;
            this.showValidation = true;
        } else {
            let queryParam = this.MS.UtilityService.GetItem('queryUrl');
            if (queryParam) {
                let token = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.CODE, queryParam);
                if (token === '') {
                    this.MS.ErrorService.message = this.MS.Translate.MSCRM_LOGIN_ERROR;
                    this.MS.ErrorService.details = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.ERRORDESCRIPTION, queryParam);
                    this.MS.ErrorService.showContactUs = true;
                    return;
                }
                var tokenObj = {
                    code: token
                };
                this.authToken = await this.MS.HttpService.executeAsync('Microsoft-GetAzureToken', tokenObj);
                if (this.authToken.IsSuccess) {
                    var response = await this.MS.HttpService.executeAsync('Microsoft-CrmGetOrgs', {});
                    if (response.IsSuccess) {
                        let msCrmOrganizationsAll: MsCrmOrganization[] = JSON.parse(response.Body.value);

                        if (msCrmOrganizationsAll && msCrmOrganizationsAll.length > 0) {
                            let countNoAdminister: number = 0;
                            let countNoOther: number = 0;
                            for (let i = 0; i < msCrmOrganizationsAll.length; i++) {
                                switch (msCrmOrganizationsAll[i].ErrorCode) {
                                    case 1:
                                        countNoAdminister++;
                                        break;
                                    case 2:
                                        countNoOther++;
                                        break;
                                    default:
                                        this.msCrmOrganizations.push(msCrmOrganizationsAll[i]);
                                        break;
                                }
                            }

                            if (this.msCrmOrganizations.length > 0) {
                                this.msCrmOrganizationId = this.msCrmOrganizations[0].OrganizationId;

                                let subscriptions: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureSubscriptions', {});
                                if (subscriptions.IsSuccess) {
                                    this.subscriptionsList = subscriptions.Body.value;
                                    if (!this.subscriptionsList ||
                                        (this.subscriptionsList && this.subscriptionsList.length === 0)) {
                                        this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_SUBSCRIPTION_ERROR_CRM;
                                        this.showAzureTrial = false;
                                    } else {
                                        this.showPricingConfirmation = true;
                                    }
                                }
                            } else {
                                if (countNoAdminister === 0) {
                                    this.MS.ErrorService.message = this.MS.Translate.MSCRM_LOGIN_NO_OTHER;
                                } else {
                                    this.MS.ErrorService.message = this.MS.Translate.MSCRM_LOGIN_NO_AUTHORIZATION;
                                }
                            }
                        } else {
                            this.MS.ErrorService.message = this.MS.Translate.MSCRM_LOGIN_NO_ORGANIZATIONS;
                        }
                    }
                }
                this.MS.UtilityService.RemoveItem('queryUrl');
            }
        }
    }

    async connect() {
        this.MS.DataStore.addToDataStore('oauthType', this.oauthType, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('AADTenant', 'common', DataStoreType.Public);
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

            let subscriptionObject = this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId);
            this.MS.DataStore.addToDataStore('SelectedSubscription', subscriptionObject, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('SelectedResourceGroup', this.selectedResourceGroup, DataStoreType.Public);

            let locationsResponse: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetLocations', {});
            if (locationsResponse.IsSuccess) {
                this.MS.DataStore.addToDataStore('SelectedLocation', locationsResponse.Body.value[5], DataStoreType.Public);
            }

            let response = await this.MS.HttpService.executeAsync('Microsoft-CreateResourceGroup', {});

            if (!response.IsSuccess) {
                return false;
            }

            return true;
        } else {
            return false;
        }
    }
}

class MsCrmOrganization {
    ConnectorUrl: string;
    ErrorCategory: number;
    ErrorCode: number;
    ErrorMessage: string;
    OrganizationId: string;
    OrganizationName: string;
    OrganizationUrl: string;
}