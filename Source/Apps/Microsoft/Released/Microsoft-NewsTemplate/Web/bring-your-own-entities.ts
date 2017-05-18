﻿import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type'

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base'

export class BringYourOwnEntities extends ViewModelBase {
    bringYourOwnEntities: string = this.option.NO;

    async OnLoaded(): Promise<void> {
        this.isValidated = true;
    }

    async NavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStore('BringYourOwnEntities', this.bringYourOwnEntities, DataStoreType.Public);
        return true;
    }
}