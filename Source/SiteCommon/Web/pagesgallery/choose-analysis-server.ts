import { DataStoreType } from '../services/datastore';
import { ViewModelBase } from '../services/viewmodelbase';

export class Customize extends ViewModelBase {
    showDescription: boolean = false;
    ssasEnabled: boolean = true;

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
        this.MS.DataStore.addToDataStore('ssasEnabled', this.ssasEnabled ? 'false' : 'true', DataStoreType.Public);

        return true;
    }
}