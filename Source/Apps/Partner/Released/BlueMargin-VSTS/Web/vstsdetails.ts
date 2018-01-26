import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type';

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base';

export class vstsdetails extends ViewModelBase {
    vstsinstance: string = '';
    vststoken: string = '';

    async onValidate(): Promise<boolean> {
        if (this.vstsinstance.length > 0 && this.vststoken.length > 0) {
            //this.isValidated = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-ValidateFacebookPage', { FacebookPages: this.searchQuery });
            this.showValidation = this.isValidated;
            this.isValidated = true;

            if (this.isValidated) {
                this.MS.DataStore.addToDataStore('vstsinstance', this.vstsinstance, DataStoreType.Public);
                this.MS.DataStore.addToDataStore('vststoken', this.vststoken, DataStoreType.Private);
            }
        }

        return this.isValidated;
    }
}