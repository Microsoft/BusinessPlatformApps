import { ViewModelBase } from '../services/viewmodelbase';
import { DataStoreType } from '../services/datastore';
import { ActionResponse } from '../services/actionresponse';

import { QueryParameter } from '../base/query-parameter';

export class AzureLogin extends ViewModelBase {
    authToken: any = {};
    azureConnection = AzureConnection;
    azureDirectory: string = '';
    connectionType: AzureConnection = AzureConnection.Organizational;
    selectedResourceGroup: string = `SolutionTemplate-${this.MS.UtilityService.GetUniqueId(5)}`;
    selectedSubscriptionId: string = '';
    showAdvanced: boolean = false;
    subscriptionsList: any[] = [];
    showPricingConfirmation: boolean = false;
    isPricingChecked: boolean = false;

    // Variables to override
    pricingUrl: string = '';
    pricingCost: string = '';

    constructor() {
        super();
        this.MS.DataStore.addToDataStore('oauthType', 'keyvault', DataStoreType.Public);

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
                let token = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.CODE, queryParam);
                var tokenObj = { code: token };
                this.authToken = await this.MS.HttpService.executeAsync('Microsoft-GetAzureToken', tokenObj);
                if (this.authToken.IsSuccess) {
                    this.MS.DataStore.addToDataStore('AzureToken', this.authToken.Body.AzureToken, DataStoreType.Private);
                    let subscriptions: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureSubscriptions', {});
                    if (subscriptions.IsSuccess) {
                        this.showPricingConfirmation = true;
                        this.subscriptionsList = subscriptions.Body.value;
                        if (!this.subscriptionsList || (this.subscriptionsList && this.subscriptionsList.length === 0)) {
                            this.MS.ErrorService.message = 'You do not have any Azure subscriptions linked to your account. You can get started with a free trial by clicking the link at the top of the page.';
                        }
                    }
                }

                this.MS.UtilityService.RemoveItem('queryUrl');
            }
        }
    }

    AzureTrialClicked(event) {
        this.MS.LoggerService.TrackEvent('AzureTrialClicked');
        return event;
    }

    AzurePricingClicked() {
        this.MS.LoggerService.TrackEvent('AzurePricingClicked');
    }

    verifyPricing() {
        this.isValidated = this.isPricingChecked;
    }

    async connect() {
        if (this.connectionType.toString() === AzureConnection.Microsoft.toString()) {
            this.MS.DataStore.addToDataStore('AADTenant', this.azureDirectory, DataStoreType.Public);
        } else {
            this.MS.DataStore.addToDataStore('AADTenant', 'common', DataStoreType.Public);
        }

        let response: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureAuthUri', {});
        window.location.href = response.Body.value;
    }

    public async NavigatingNext(): Promise<boolean> {
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

        return await super.NavigatingNext();
    }
}

enum AzureConnection {
    Microsoft,
    Organizational
}