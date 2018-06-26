import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type'

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base'

export class BringYourOwnEntities extends ViewModelBase {
    bringYourOwnEntities: string = this.MS.Option.NO;

    async onLoaded(): Promise<void> {
        this.isValidated = true;
    }

    async onNavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStore('BringYourOwnEntities', this.bringYourOwnEntities, DataStoreType.Public);
        return true;
    }
}