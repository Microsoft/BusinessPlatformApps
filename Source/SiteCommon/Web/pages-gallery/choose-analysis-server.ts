import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class Customize extends ViewModelBase {
    registerAnalysisServices: boolean = true;
    showDescription: boolean = false;
    ssasEnabled: string = 'false';
   
    async onLoaded(): Promise<void> {
        this.setValidated(false);
    }

    async onNavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStoreWithCustomRoute('ssas', 'ssasDisabled', this.ssasEnabled === 'true' ? 'false' : 'true', DataStoreType.Public);

        let isSuccess: boolean = true;

        if (this.registerAnalysisServices && this.ssasEnabled === 'true') {
            isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-RegisterProvider', { AzureProvider: 'Microsoft.AnalysisServices' });
        }

        return isSuccess;
    }
}