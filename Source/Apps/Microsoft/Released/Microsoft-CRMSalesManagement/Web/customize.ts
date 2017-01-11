import { DataStoreType } from '../../../../../SiteCommon/Web/services/datastore';
import { ViewModelBase } from '../../../../../SiteCommon/Web/services/viewmodelbase';

export class Customize extends ViewModelBase {
    actuals: string = 'Closed opportunities';
    baseUrl: string = '';
    fiscalMonth: string = 'January';

    async NavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeActuals', 'SqlGroup', 'data', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeActuals', 'SqlSubGroup', 'actual_sales', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeActuals', 'SqlEntryName', 'enabled', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeActuals', 'SqlEntryValue', this.actuals ? 1 : 0, DataStoreType.Public);

        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlSubGroup', 'SalesManagement', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlEntryName', 'BaseURL', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlEntryValue', this.baseUrl, DataStoreType.Public);

        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlSubGroup', 'SalesManagement', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlEntryName', 'FiscalMonthStart', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlEntryValue', this.fiscalMonth, DataStoreType.Public);

        return true;
    }
}