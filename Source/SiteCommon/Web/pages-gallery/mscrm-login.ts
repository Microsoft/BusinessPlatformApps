import { QueryParameter } from '../constants/query-parameter';

import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';
import { MsCrmOrganization } from '../models/ms-crm-organization';

import { AzureLogin } from './azure-login';

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
                        this.msCrmOrganizations = JSON.parse(response.Body.value);

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
                                    this.selectedSubscriptionId = this.subscriptionsList[0].SubscriptionId;
                                    this.showPricingConfirmation = true;
                                    this.isValidated = true;
                                    this.showValidation = true;
                                }
                            }
                        } else {
                            this.MS.ErrorService.message = this.MS.Translate.MSCRM_LOGIN_NO_AUTHORIZATION;
                        }
                    } else {
                        this.MS.ErrorService.message = this.MS.Translate.MSCRM_LOGIN_NO_ORGANIZATIONS;
                    }
                }
            }
            this.MS.UtilityService.RemoveItem('queryUrl');
        }
    }


    async connect() {
        this.MS.DataStore.addToDataStore('oauthType', this.oauthType, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('AADTenant', 'common', DataStoreType.Public);
        let response: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureAuthUri', {});
        window.location.href = response.Body.value;
    }

    public async NavigatingNext(): Promise<boolean> {
        let msCrmOrganization: MsCrmOrganization = this.msCrmOrganizations.find(o => o.OrganizationId === this.msCrmOrganizationId);

        if (msCrmOrganization) {
            this.MS.DataStore.addToDataStore('Entities', this.entities, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('OrganizationId', msCrmOrganization.OrganizationId, DataStoreType.Private);
            this.MS.DataStore.addToDataStore('OrganizationUrl', msCrmOrganization.OrganizationUrl, DataStoreType.Private);

            let response2 = await this.MS.HttpService.executeAsync('Microsoft-CrmGetOrganization', {});

            if (!response2.IsSuccess) {
                return false;
            }

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