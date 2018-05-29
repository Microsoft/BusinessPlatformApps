import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';

import { ViewModelBase } from '../services/view-model-base';

export class SelectStorage extends ViewModelBase {
    selectedSubscriptionId: string = '';
    selectedSubscriptionName: string = '';
    selectedStorageName: string = '';
    selectedContainerName: string = '';
    storagesList: any[] = [];
    containersList: any[] = [];
    prerequisiteDescription = '';
    prerequisiteLink = '';
    prerequisiteLinkText = '';

    async getStorages(): Promise<void> {
        let storages: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureStorages');
        if (storages.IsSuccess) {
            this.storagesList = storages.Body;
            if (!this.storagesList || (this.storagesList && this.storagesList.length === 0)) {
                this.MS.ErrorService.message = this.MS.Translate.AZURE_SUBSCRIPTION_STORAGE_ERROR + this.selectedSubscriptionName;
                this.isValidated = false;
            } else {
                this.selectedStorageName = this.storagesList[0].StorageAccountName;
                this.containersList = this.storagesList[0].Containers;

                this.selectedContainerName = this.containersList.length === 0 ? '' : this.containersList[0].Name;
            }
        }
    }

    async changeContainers(): Promise<void> {
        var storage = this.storagesList.find(x => x.StorageAccountName === this.selectedStorageName);
        this.containersList = storage.Containers;

        if (this.selectedContainerName && this.containersList.length > 0) {
            var container = this.containersList.find(x => x.Name === this.selectedContainerName);
            if (container) {
                this.isValidated = true;
                return;
            }
        }

        this.selectedContainerName = this.containersList.length === 0 ? '' : this.containersList[0].Name;

        if (this.selectedStorageName && this.selectedContainerName && this.selectedContainerName.length > 0) {
            this.isValidated = true;
        }
        else {
            this.isValidated = false;
        }
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();

        if (!this.selectedSubscriptionName) {
            var subscriptionObject = this.MS.DataStore.getJson('SelectedSubscription');
            this.selectedSubscriptionName = subscriptionObject.DisplayName;
        }

        if (this.storagesList.length === 0) {
            await this.getStorages();
        }

        if (this.selectedStorageName && this.selectedContainerName && this.selectedContainerName.length > 0) {
            this.isValidated = true;
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        this.MS.DataStore.addToDataStore('SelectedStorageAccount', this.storagesList.find(x => x.StorageAccountName === this.selectedStorageName), DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SelectedStorageAccountName', this.selectedStorageName, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SelectedContainer', this.selectedContainerName, DataStoreType.Public);

        return isSuccess;
    }
}