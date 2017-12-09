import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class facebookPages extends ViewModelBase {
    searchQuery: string = '';

    async onValidate(): Promise<boolean> {
        if (this.searchQuery.length > 0) {
            this.isValidated = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-ValidateFacebookPage', { FacebookPages: this.searchQuery });
            this.showValidation = this.isValidated;

            if (this.isValidated) {
                this.MS.DataStore.addToDataStore('FacebookPagesToFollow', this.searchQuery, DataStoreType.Public);
            }
        }

        return this.isValidated;
    }
}