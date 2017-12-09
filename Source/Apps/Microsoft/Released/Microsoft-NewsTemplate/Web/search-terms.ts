import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type'

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base'

export class SearchTerms extends ViewModelBase {
    searchQuery: string = '';

    async onValidate(): Promise<boolean> {
        if (this.searchQuery.length > 0 && this.searchQuery.length <= 130) {
            this.setValidated();
            this.MS.DataStore.addToDataStore('SearchQuery', this.searchQuery, DataStoreType.Public);
        } else if (this.searchQuery.length > 130) {
            this.MS.ErrorService.set(this.MS.Translate.SEARCH_TERMS_QUERY_ERROR_LONG);
        } else {
            this.MS.ErrorService.set(this.MS.Translate.SEARCH_TERMS_QUERY_ERROR_EMPTY);
        }

        return this.isValidated;
    }
}