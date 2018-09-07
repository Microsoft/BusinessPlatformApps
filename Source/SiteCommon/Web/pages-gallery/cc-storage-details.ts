import { AzureConnection } from '../enums/azure-connection';
import { DataStoreType } from '../enums/data-store-type';

import { AzureLocation } from '../models/azure-location';
import { ActionResponse } from '../models/action-response';

import { ViewModelBase } from '../services/view-model-base';

export class StorageDetails extends ViewModelBase {

    authToken: any = {};
    azureConnection = AzureConnection;
    azureDirectory: string = '';
    azureProviders: string[] = [];
    azureLocations: AzureLocation[] = [];
    bapiServices: string[] = [];
    bingUrl: string = '';
    bingtermsofuse: string = '';
    connectionType: AzureConnection = AzureConnection.Organizational;
    defaultLocation: number = 5;
    defaultLocationName: string = '';
    selectedLocationName: string = '';
    oauthType: string = '';
    pricingCalculator: string = '';
    pricingCalculatorUrl: string = '';
    pricingUrl: string = '';
    pricingCost: string = '';
    selectedResourceGroup: string = `SolutionTemplate-${this.MS.UtilityService.getUniqueId(5)}`;
    selectedSubscriptionId: string = '';
    showAdvanced: boolean = false;
    showPricingConfirmation: boolean = false;
    subscriptionsList: any[] = [];
    showLocations: boolean = false;
    allowedLocations: string = ''; // Some services only existing in certain regions so added region/location filter

    async connect(): Promise<void> {
        this.MS.UtilityService.connectToAzure(this.oauthType, this.isConnectionMicrosoft() ? this.azureDirectory : this.MS.Translate.DEFAULT_TENANT);
    }

    async getSubscriptions(): Promise<void> {
        let subscriptions: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureSubscriptions', { showLocations: this.showLocations, allowedLocations: this.allowedLocations });
        if (subscriptions.IsSuccess) {
            this.subscriptionsList = subscriptions.Body.value;
            if (!this.subscriptionsList || (this.subscriptionsList && this.subscriptionsList.length === 0)) {
                this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_SUBSCRIPTION_ERROR;
            } else {
                this.selectedSubscriptionId = this.subscriptionsList[0].SubscriptionId;

                if (this.showLocations) {
                    this.azureLocations = this.subscriptionsList[0].Locations;
                    if (this.defaultLocationName) {
                        var location = this.azureLocations.find(x => x.Name === this.defaultLocationName);
                        if (location) {
                            this.selectedLocationName = this.defaultLocationName;
                        }
                        else {
                            this.selectedLocationName = this.azureLocations.length === 0 || this.azureLocations.length <= this.defaultLocation ? '' : this.azureLocations[this.defaultLocation].Name;
                        }
                    }
                    else {
                        this.selectedLocationName = this.azureLocations.length === 0 || this.azureLocations.length <= this.defaultLocation ? '' : this.azureLocations[this.defaultLocation].Name;
                    }
                }

                this.validate();
                this.showPricingConfirmation = true;
                await this.MS.HttpService.executeAsync('Microsoft-PowerBiLogin');
            }
        }
    }

    isConnectionMicrosoft(): boolean {
        return this.connectionType.toString() === AzureConnection.Microsoft.toString();
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();

        if (this.subscriptionsList.length > 0) {
            this.validate();
        } else {
            await this.MS.UtilityService.getToken(this.oauthType, async () => {
                await this.getSubscriptions();
            });
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        this.MS.DataStore.addToDataStore('SelectedSubscription', this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId), DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SelectedResourceGroup', this.selectedResourceGroup, DataStoreType.Public);

        if (!this.showLocations) {
            let locationsResponse: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetLocations');
            if (locationsResponse.IsSuccess) {
                if (this.defaultLocationName) {
                    let locationList: any[] = locationsResponse.Body.value;
                    this.MS.DataStore.addToDataStore('SelectedLocation', locationList.find(x => x.Name === this.defaultLocationName), DataStoreType.Public);
                }
                else {
                    this.MS.DataStore.addToDataStore('SelectedLocation', locationsResponse.Body.value[this.defaultLocation], DataStoreType.Public);
                }
            }
        }
        else {
            this.MS.DataStore.addToDataStore('SelectedLocation', this.azureLocations.find(x => x.Name === this.selectedLocationName), DataStoreType.Public);
        }

        isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CreateResourceGroup');

        for (let i = 0; i < this.azureProviders.length && isSuccess; i++) {
            isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-RegisterProvider', { AzureProvider: this.azureProviders[i] });
        }

        for (let i = 0; i < this.bapiServices.length && isSuccess; i++) {
            isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-RegisterBapiService', { BapiService: this.bapiServices[i] });
        }

        return isSuccess;
    }

    changeSubscription(): void {
        if (!this.showLocations) {
            this.validate();
            return;
        }

        this.azureLocations = this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId).Locations;

        // If selected one location originally and the location also exists in the new subscription, then keep the selected location
        if (this.selectedLocationName) {
            var location = this.azureLocations.find(x => x.Name === this.selectedLocationName);
            if (location) {
                return;
            }
        }

        // If nothing was selected, use the default location name to find the location
        // If nothing matches the default location name, use the default location then
        if (this.defaultLocationName) {
            var location = this.azureLocations.find(x => x.Name === this.defaultLocationName);
            if (location) {
                this.selectedLocationName = this.defaultLocationName;
            }
            else {
                this.selectedLocationName = this.azureLocations.length === 0 || this.azureLocations.length <= this.defaultLocation ? '' : this.azureLocations[this.defaultLocation].Name;
            }
        }
        else {
            this.selectedLocationName = this.azureLocations.length === 0 || this.azureLocations.length <= this.defaultLocation ? '' : this.azureLocations[this.defaultLocation].Name;
        }

        this.validate();
    }

    changeLocation(): void {
        this.validate();
    }

    async validateResourceGroup(): Promise<boolean> {
        this.onInvalidate();

        let subscriptionObject = this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId);
        this.MS.DataStore.addToDataStore('SelectedSubscription', subscriptionObject, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SelectedResourceGroup', this.selectedResourceGroup, DataStoreType.Public);

        if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-ExistsResourceGroup')) {
            this.validate();
        }

        return this.isValidated;
    }

    validate(): boolean {
        if (!this.selectedSubscriptionId) {
            this.onInvalidate();
            return false;
        }

        if (!this.showLocations) {
            return this.setValidated();
        }

        if (this.selectedLocationName) {
            return this.setValidated();
        }
        else {
            this.onInvalidate();
            return false;
        }
    }
}