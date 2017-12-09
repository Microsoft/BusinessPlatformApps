import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';
import { ActionResponse } from "../models/action-response";

export class Customize extends ViewModelBase {

    axBaseUrl: string = '';
    instances: string[] = [];
    selectedInstance: string = '';
    urlRegex: RegExp;

    async onLoaded(): Promise<void> {
        super.onLoaded();
        this.urlRegex = /^(ht|f)tps:\/\/[a-z0-9-\.]+\.[a-z]{2,4}\/?([^\s<>\#%\,\{\}\\|\\\^\[\]]+)?\/$/;
        await this.getInstances();
    }

    async onValidate(): Promise<boolean> {
        if (this.axBaseUrl) {
            this.selectedInstance = '';
            if (!this.axBaseUrl.endsWith('/')) {
                this.axBaseUrl = this.axBaseUrl + '/';
            }
            if (this.urlRegex.test(this.axBaseUrl)) {              
                this.MS.DataStore.addToDataStore('AxInstanceName', this.axBaseUrl, DataStoreType.Public);
                this.isValidated = await this.validateAxInstance();
            } else {
                this.MS.ErrorService.message = 'Validation failed. The url address ' + this.axBaseUrl + ' is not valid.';
            }
        }
        if (this.selectedInstance) {
            this.MS.DataStore.addToDataStore('AxInstanceName', this.selectedInstance, DataStoreType.Public);
            this.isValidated = await this.validateAxInstance();
        }
        return true;
    }

    async validateAxInstance(): Promise<boolean> {
        let validInstance: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-ValidateAxInstance');
        if (validInstance.IsSuccess)
            return true;
        return false;
    }

    async onInvalidate(): Promise<void> {
        super.onInvalidate();
        if (this.selectedInstance) {
            this.selectedInstance = '';
        }
        else {
            await this.getInstances();
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlSubGroup', 'D365', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlEntryName', 'Operations', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlEntryValue', 'CreditAndCollections', DataStoreType.Public);

        return true;
    }

    async getInstances(): Promise<void> {
        this.instances = await this.MS.HttpService.getResponseAsync('Microsoft-GetAxInstances');
        if (this.instances && this.instances.length > 0) {
            this.selectedInstance = this.instances[0];
        }
    }
}