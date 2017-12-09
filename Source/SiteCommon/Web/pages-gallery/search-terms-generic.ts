import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class SearchTerms extends ViewModelBase {
    searchQuery: string = '';

    async onValidate(): Promise<boolean> {
        if (this.searchQuery.length > 0) {
            this.setValidated();
            this.MS.DataStore.addToDataStore('SearchQuery', this.searchQuery, DataStoreType.Public);
        }

        return true;
    }
}