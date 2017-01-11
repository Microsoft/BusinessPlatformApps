import { DataStoreType } from '../services/datastore';
import { ViewModelBase } from '../services/viewmodelbase';

export class TwitterHandles extends ViewModelBase {
    accounts: string = '';
    twitterHandleId: string = '';
    twitterHandleName: string = '';

    constructor() {
        super();
        this.isValidated = true;
    }

    async OnValidate(): Promise<boolean> {
        if (!super.OnValidate()) {
            return false;
        }

        let body: any = {};
        body.Accounts = this.accounts;
        let response = await this.MS.HttpService.executeAsync('Microsoft-ValidateTwitterAccount', body);
        if (response.IsSuccess) {
            this.isValidated = true;
            this.showValidation = true;
            this.twitterHandleName = response.Body.twitterHandle;
            this.twitterHandleId = response.Body.twitterHandleId;
        }

        this.MS.DataStore.addToDataStore('TwitterHandles', this.accounts, DataStoreType.Public);
    }

    async Invalidate() {
        super.Invalidate();
        if (!this.accounts) {
            this.isValidated = true;
        }
    }

    async NavigatingNext(): Promise<boolean> {

        this.MS.DataStore.addToDataStoreWithCustomRoute('c1','SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('c1', 'SqlSubGroup', 'Twitter', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('c1','SqlEntryName', 'twitterHandle', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('c1', 'SqlEntryValue', this.twitterHandleName, DataStoreType.Public);

        this.MS.DataStore.addToDataStoreWithCustomRoute('c2', 'SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('c2','SqlSubGroup', 'Twitter', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('c2','SqlEntryName', 'twitterHandleId', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('c2','SqlEntryValue', this.twitterHandleId, DataStoreType.Public);
        return super.NavigatingNext();
    }
}