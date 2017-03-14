import { DataStoreType } from '../services/datastore';
import { ViewModelBase } from '../services/viewmodelbase';

export class Customize extends ViewModelBase {
    showDescription: boolean = false;
    ssasEnabled: string = 'true';
   
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