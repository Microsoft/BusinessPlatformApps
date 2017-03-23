import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class SapSource extends ViewModelBase {
    applicationServer: string = '';
    client: number = 800;
    instanceNumber: string = '00';
    language: string = 'EN';
    languages: string[] = [];
    password: string = '';
    rowBatchSize: number = 250000;
    sapRouterString: string = '';
    showAdvanced: boolean = false;
    systemId: string = 'ZZZ';
    user: string = '';

    constructor() {
        super();
    }

    async OnLoaded() {
        this.languages = ['AF', 'AR', 'BG', 'CA', 'CS', 'DA', 'DE', 'EL', 'EN', 'ES', 'ET', 'FI', 'FR', 'HE', 'HR', 'HU', 'ID', 'IS', 'IT', 'JA', 'KO', 'LT', 'LV', 'MS', 'NL', 'NO', 'PL', 'PT', 'RO', 'RU', 'SH', 'SK', 'SL', 'SR', 'SV', 'TH', 'TR', 'UK', 'Z1', 'ZF', 'ZH'];
        this.isValidated = false;
    }

    async OnValidate(): Promise<boolean> {
        super.OnValidate();

        this.isValidated = false;
        this.showValidation = false;

        this.storeCredentials();

        await this.MS.HttpService.executeAsync('Microsoft-CredentialManagerWrite');
        await this.MS.HttpService.executeAsync('Microsoft-WriteSAPJson');

        let responseValidate = await this.MS.HttpService.executeAsync('Microsoft-ValidateSAP');
        this.isValidated = responseValidate.IsSuccess;

        if (this.isValidated) {
            this.showValidation = true;
        }

        return this.isValidated;
    }

    private storeCredentials() {
        this.MS.DataStore.addToDataStore('CredentialTarget', 'Simplement.SolutionTemplate.AR.SAP', DataStoreType.Private);
        this.MS.DataStore.addToDataStore('CredentialUsername', this.user, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('CredentialPassword', this.password, DataStoreType.Private);

        this.MS.DataStore.addToDataStore('RowBatchSize', this.rowBatchSize, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SapClient', this.client, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SapHost', this.applicationServer, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SapLanguage', this.language, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SAPPassword', this.password, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('SapRouterString', this.sapRouterString, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SapSystemId', this.systemId, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SapSystemNumber', this.instanceNumber, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SapUser', this.user, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SqlConnectionString', '', DataStoreType.Public);
    }
}