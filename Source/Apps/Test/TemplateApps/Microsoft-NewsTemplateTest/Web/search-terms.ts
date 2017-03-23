import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type'

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base'

export class SearchTerms extends ViewModelBase {
    searchQuery: string = '';

    constructor() {
        super();
        this.isValidated = false;
    }

    async OnValidate(): Promise<boolean> {
        if (!super.OnValidate()) {
            return false;
        }

        if (this.searchQuery.length > 0) {
            this.isValidated = true;
            this.showValidation = true;
            this.MS.DataStore.addToDataStore('SearchQuery', this.searchQuery, DataStoreType.Public);
        }

        return true;
    }
}