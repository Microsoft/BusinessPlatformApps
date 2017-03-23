import { QueryParameter } from '../constants/query-parameter';

import { AzureConnection } from '../enums/azure-connection';
import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';

import { ViewModelBase } from '../services/view-model-base';

export class AzureLogin extends ViewModelBase {
    authToken: any = {};
    azureConnection = AzureConnection;
    azureDirectory: string = '';
    azureProviders: string[] = [];
    bapiServices: string[] = [];
    connectionType: AzureConnection = AzureConnection.Organizational;
    oauthType: string = '';
    selectedResourceGroup: string = `SolutionTemplate-${this.MS.UtilityService.GetUniqueId(5)}`;
    selectedSubscriptionId: string = '';
    showAdvanced: boolean = false;
    showPricingConfirmation: boolean = false;
    subscriptionsList: any[] = [];
    defaultLocation: number = 5;

    //News Specific Variables for Azure
    bingUrl: string = '';
    bingtermsofuse: string = '';

    // Variables to override
    pricingUrl: string = '';
    pricingCost: string = '';
    pricingCalculator: string = '';
    pricingCalculatorUrl: string = '';

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
                let token = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.CODE, queryParam);
                if (token === '') {
                    this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_UNKNOWN_ERROR;
                    this.MS.ErrorService.details = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.ERRORDESCRIPTION, queryParam);
                    this.MS.ErrorService.showContactUs = true;
                    return;
                }

                var tokenObj = { code: token };
                this.authToken = await this.MS.HttpService.executeAsync('Microsoft-GetAzureToken', tokenObj);
                if (this.authToken.IsSuccess) {
                    this.MS.DataStore.addToDataStore('AzureToken',
                        this.authToken.Body.AzureToken,
                        DataStoreType.Private);
                    let subscriptions: ActionResponse = await this.MS.HttpService
                        .executeAsync('Microsoft-GetAzureSubscriptions', {});
                    if (subscriptions.IsSuccess) {
                        this.subscriptionsList = subscriptions.Body.value;
                        if (!this
                            .subscriptionsList ||
                            (this.subscriptionsList && this.subscriptionsList.length === 0)) {
                            this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_SUBSCRIPTION_ERROR;
                        } else {
                            this.selectedSubscriptionId = this.subscriptionsList[0].SubscriptionId;
                            this.showPricingConfirmation = true;
                            this.isValidated = true;
                            this.showValidation = true;
                            await this.MS.HttpService.executeAsync('Microsoft-PowerBiLogin');
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

    async connect() {
        this.MS.DataStore.addToDataStore('oauthType', this.oauthType, DataStoreType.Public);

        if (this.connectionType.toString() === AzureConnection.Microsoft.toString()) {
            this.MS.DataStore.addToDataStore('AADTenant', this.azureDirectory, DataStoreType.Public);
        } else {
            this.MS.DataStore.addToDataStore('AADTenant', 'common', DataStoreType.Public);
        }

        let response: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureAuthUri', {});
        window.location.href = response.Body.value;
    }

    async NavigatingNext(): Promise<boolean> {
        let subscriptionObject = this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId);
        this.MS.DataStore.addToDataStore('SelectedSubscription', subscriptionObject, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SelectedResourceGroup', this.selectedResourceGroup, DataStoreType.Public);

        let locationsResponse: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetLocations', {});
        if (locationsResponse.IsSuccess) {
            this.MS.DataStore.addToDataStore('SelectedLocation', locationsResponse.Body.value[this.defaultLocation], DataStoreType.Public);
        }

        let response = await this.MS.HttpService.executeAsync('Microsoft-CreateResourceGroup', {});

        if (!response.IsSuccess) {
            return false;
        }

        for (let i = 0; i < this.azureProviders.length; i++) {
            this.MS.DataStore.addToDataStore('AzureProvider', this.azureProviders[i], DataStoreType.Public);
            let responseRegister = await this.MS.HttpService.executeAsync('Microsoft-RegisterProvider', {});
            if (!responseRegister.IsSuccess) {
                return false;
            }
        }

        for (let i = 0; i < this.bapiServices.length; i++) {
            this.MS.DataStore.addToDataStore('BapiService', this.bapiServices[i], DataStoreType.Public);
            let responseRegister = await this.MS.HttpService.executeAsync('Microsoft-RegisterBapiService', {});
            if (!responseRegister.IsSuccess) {
                return false;
            }
        }

        return await super.NavigatingNext();
    }
}