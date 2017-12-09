import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type';

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base';

export class CustomizeEdFiOrg extends ViewModelBase {

    localOrganizationID: string = '';
    stateOrganizationID: string = '';
    agencyCategoryType: string = '';
    institutionName: string = '';
    institutionAddress: string = '';
    institutionCity: string = '';
    institutionState: string = '';
    institutionZip: string = '';

    async onNavigatingNext(): Promise<boolean> {

        this.MS.DataStore.addToDataStore('localOrganizationID', this.localOrganizationID, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('stateOrganizationID', this.stateOrganizationID, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('agencyCategoryType', this.agencyCategoryType, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('institutionName', this.institutionName, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('institutionAddress', this.institutionAddress, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('institutionCity', this.institutionCity, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('institutionState', this.institutionState, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('institutionZip', this.institutionZip, DataStoreType.Public);

        return true;
    }


    async onValidate(): Promise<boolean> {
        this.isValidated = true;
        return this.isValidated;
    }
}