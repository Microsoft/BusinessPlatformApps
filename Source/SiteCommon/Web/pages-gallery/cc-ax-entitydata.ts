import { ViewModelBase } from '../services/view-model-base';
import { DataStoreType } from '../enums/data-store-type';
import { D365Measurement } from '../models/d365-measurement';

export class AxEntityData extends ViewModelBase {
    measurements: D365Measurement[] = [];

    async onLoaded(): Promise<void> {
        super.onLoaded();
        if (this.MS.DataStore.getValue('SelectedMeasurements') === null) {
            this.measurements = [{
                name: 'FinancialAccounts',
                title: 'Financial accounts including Ledger balances, Revenue and Expenses',
                subtitle: 'By analyzing your aggregated Financial results with thousands of data sources from across the world, system will identify and forecast drivers that are impacting your bottom line.',
                value: 'CustCollectionsBIMeasurements',
                selected: true
            },
            {
                name: 'AccountsReceivable',
                title: 'Accounts receivable data including Invoices, Customers and Settlements',
                subtitle: 'By analyzing previous payment patterns and other data, system will predict which of your customers may pay on time and who will delay. System will also help you choose proactive steps and show the outcome.',
                value: 'LedgerActivityMeasure',
                selected: false
            },
            {
                name: 'RetailPOS',
                title: 'Retail POS transactions, items and store details',
                subtitle: 'We know cashflow is tight. By analyzing strategic relationships and previous payment patterns, let the system recommend ways in which you can optimize your bill payments.',
                value: 'RetailSales',
                selected: false
            },
            {
                name: 'Warehouse',
                title: 'Inventory, warehouses and product information',
                subtitle: 'By analyzing your aggregated Financial results with thousands of data sources from across the world, system will identify and forecast drivers that are impacting your bottom line.',
                value: 'WHSWarehouse',
                selected: false
            }];
        }
        if (this.measurements.filter(m => m.selected).length > 0) {
            this.isValidated = true;

            this.MS.DataStore.addToDataStore('SelectedMeasurements',
                this.measurements.filter(m => m.selected).map(m => m.value).join(","),
                DataStoreType.Public);
        }
    }

    async measurementSelected(): Promise<void> {
        this.MS.DataStore.addToDataStore('SelectedMeasurements',
            this.measurements.filter(m => m.selected).map(m => m.value).join(","),
            DataStoreType.Public);

        if (this.measurements.filter(m => m.selected).length > 0) {
            this.isValidated = true;            
        }
        else {
            this.isValidated = false;
        }
    }
}