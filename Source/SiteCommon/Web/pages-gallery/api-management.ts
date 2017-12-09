import { DataStoreType } from '../enums/data-store-type';

import { ApiManagementService } from '../models/api-management-service';

import { ViewModelBase } from '../services/view-model-base';

export class ApiManagement extends ViewModelBase {
    idApimService: string = '';
    listApimServices: ApiManagementService[] = [];

    async onLoaded(): Promise<void> {
        this.listApimServices = await this.MS.HttpService.getResponseAsync('Microsoft-GetApiManagementServices');

        if (this.listApimServices && this.listApimServices.length > 0) {
            this.idApimService = this.listApimServices[0].id;
            this.isValidated = true;
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStore('IdApimService', this.idApimService, DataStoreType.Public);
        return true;
    }
}