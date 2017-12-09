import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type';

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base';

export class CustomizeSql extends ViewModelBase {
    sqlServerAdminLogin: string = '';
    sqlServerAdminPassword: string = '';
    sqlServerProductionApiLogin: string = '';
    sqlServerProductionApiPassword: string = '';

    async onNavigatingNext(): Promise<boolean> {

        this.MS.DataStore.addToDataStore('sqlServerAdminLogin', this.sqlServerAdminLogin, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('sqlServerAdminPassword', this.sqlServerAdminPassword, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('sqlServerProductionApiLogin', this.sqlServerProductionApiLogin, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('sqlServerProductionApiPassword', this.sqlServerProductionApiPassword, DataStoreType.Private);

        return true;
    }


    async onValidate(): Promise<boolean> {
        this.isValidated = this.sqlServerAdminPassword.length > 0 && this.sqlServerAdminLogin.length > 0 && this.sqlServerProductionApiLogin.length > 0 && this.sqlServerProductionApiPassword.length > 0;
        return this.isValidated;
    }
}