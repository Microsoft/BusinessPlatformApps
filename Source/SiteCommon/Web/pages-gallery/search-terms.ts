import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class SearchTerms extends ViewModelBase {
    searchQuery: string = '';

    constructor() {
        super();
    }

    async OnLoaded() {
        this.isValidated = false;
    }

    async OnValidate(): Promise<boolean> {
        if (this.searchQuery.length > 0) {
            this.isValidated = true;
            this.showValidation = true;
            this.MS.DataStore.addToDataStore('SearchQuery', this.searchQuery, DataStoreType.Public);
        }

        return true;
    }
}