import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class CognitiveService extends ViewModelBase {
    cognitiveServiceKey: string = '';
    cognitiveSelectedType: string = 'NewKey';
    cognitiveServiceName: string = 'SolutionTemplateCognitiveService';

    constructor() {
        super();
        this.isValidated = true;
    }

    onKeyTypeChange(): void {
        this.Invalidate();
        if (this.cognitiveSelectedType === 'ExistingKey') {
            this.isValidated = false;
        } else if (this.cognitiveSelectedType === 'NewKey') {
            this.cognitiveServiceKey = '';
            this.isValidated = true;
        }
    }

    Invalidate(): void {
        super.Invalidate();
    }

    async OnValidate(): Promise<boolean> {
        if (!super.OnValidate()) {
            return false;
        }

        if (this.cognitiveSelectedType === 'ExistingKey') {
            let body: any = {};
            body.CognitiveServiceKey = this.cognitiveServiceKey
            let response = await this.MS.HttpService.executeAsync('Microsoft-ValidateCognitiveKey', body);
            if (response.IsSuccess) {
                this.isValidated = true;
                this.showValidation = true;
            }
        } else if (this.cognitiveSelectedType === 'NewKey') {
            this.isValidated = true;
        }

        return this.isValidated;
    }

    async NavigatingNext(): Promise<boolean> {
        if (!super.NavigatingNext()) {
            return false;
        }

        this.MS.DataStore.addToDataStore('CognitiveServiceKey', this.cognitiveServiceKey, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('CognitiveServiceName', this.cognitiveServiceName, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('CognitiveSkuName', 'S1', DataStoreType.Public);

        return true;
    }
}