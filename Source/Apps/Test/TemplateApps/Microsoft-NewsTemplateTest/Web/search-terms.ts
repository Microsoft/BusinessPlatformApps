import { ViewModelBase } from '../../../../../SiteCommon/Web/services/viewmodelbase'
import { DataStoreType } from '../../../../../SiteCommon/Web/services/datastore'

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