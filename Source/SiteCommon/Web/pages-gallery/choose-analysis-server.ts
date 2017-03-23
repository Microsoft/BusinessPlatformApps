import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class Customize extends ViewModelBase {
    showDescription: boolean = false;
    ssasEnabled: string = 'false';
   
    async OnLoaded() {
        this.isValidated = true;
        this.showValidation = false;
    }

    async OnValidate(): Promise<boolean> {
        this.isValidated = true;
        this.showValidation = false;
        return true;
    }

    async NavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStoreWithCustomRoute('ssas', 'ssasDisabled', this.ssasEnabled === 'true' ? 'false' : 'true', DataStoreType.Public);
        return true;
    }
}