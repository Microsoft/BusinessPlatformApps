import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';
import { D365Organization } from '../models/d365-organization';

import { ViewModelBase } from '../services/view-model-base';

export class D365Login extends ViewModelBase {
    d365OrganizationId: string = '';
    d365Organizations: D365Organization[] = [];
    d365Password: string = '';
    d365Username: string = '';

    async OnLoaded(): Promise<void> {
        this.isValidated = false;
    }

    async OnValidate(): Promise<boolean> {
        this.Invalidate();

        this.MS.DataStore.addToDataStore('D365Username', this.d365Username, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('D365Password', this.d365Password, DataStoreType.Private);

        let response: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetD365Organizations');

        if (response.IsSuccess) {
            this.d365Organizations = JSON.parse(response.Body.value);

            if (this.d365Organizations && this.d365Organizations.length > 0) {
                this.d365OrganizationId = this.d365Organizations[0].Id;

                this.isValidated = true;
                this.showValidation = true;
            }
        }

        return this.isValidated;
    }

    async NavigatingNext(): Promise<boolean> {
        let d365Organization: D365Organization = this.d365Organizations.find(x => x.Id === this.d365OrganizationId);
        this.MS.DataStore.addToDataStore('ConnectorUrl', d365Organization.ConnectorUrl, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('OrganizationName', d365Organization.Name, DataStoreType.Private);
        return true;
    }
}