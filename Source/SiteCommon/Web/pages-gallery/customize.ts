import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class Customize extends ViewModelBase {
    actuals: string = 'Closed opportunities';
    baseUrl: string = '';
    fiscalMonth: number = 1;
    emails: string = '';
    emailRegex: RegExp;
    isEmailValidated: boolean = false;
    pipelineFrequency: string = 'Week';
    pipelineInterval: string = '1';
    recurrent: string = 'Never';
    showEmails: boolean = false;
    showCrmUrl: boolean = false;
    showRecurrenceOptions: boolean = false;
    sourceApplication: string = '';

    async OnLoaded() {
        this.emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        this.isValidated = true;
        if (this.showCrmUrl) {
            let orgUrl: string = this.MS.DataStore.getValue('OrganizationUrl');
            if (orgUrl && orgUrl[orgUrl.length - 1] === '/') {
                orgUrl = orgUrl.substr(0, orgUrl.length - 1);
            }
            this.baseUrl = orgUrl;
            this.isValidated = true;
        }

        if (this.sourceApplication === 'Salesforce' && this.emails != '') {
            this.isValidated = false;
            this.showValidation = false;
        }
    }

    async OnValidate(): Promise<boolean> {
        this.isValidated = false;
        this.showValidation = false;
        this.isEmailValidated = false;

        if (this.emails != null && this.emails != '') {
            let mails = this.emails.split(',');
            for (let mail in mails) {
                if (!this.emailRegex.test(mails[mail])) {
                    this.isValidated = false;
                    this.showValidation = false;
                    this.MS.ErrorService.message = 'Validation failed. The email address ' + mails[mail] + ' is not valid.';
                    return false;
                }
            }
        }

        this.isValidated = true;
        this.showValidation = true;
        this.isEmailValidated = true;
        return true;
    }

    async NavigatingNext(): Promise<boolean> {

        if (this.sourceApplication === 'Salesforce') {
            switch (this.recurrent) {
                case 'Every 15 minutes':
                    this.pipelineFrequency = 'Minute';
                    this.pipelineInterval = '15';
                    break;
                case 'Every 30 minutes':
                    this.pipelineFrequency = 'Minute';
                    this.pipelineInterval = '30';
                    break;
                case 'Hourly':
                    this.pipelineFrequency = 'Hour';
                    this.pipelineInterval = '1';
                    break;
                case 'Daily':
                    this.pipelineFrequency = 'Day';
                    this.pipelineInterval = '1';
                    break;
                case 'Never':
                    this.pipelineFrequency = 'Never';
                    this.pipelineInterval = 'Never';
                    break;
                default:
                    break;
            }

            this.MS.DataStore.addToDataStore('EmailAddresses', this.emails, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('pipelineStart', null, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('pipelineEnd', null, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('pipelineType', null, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('postDeploymentPipelineFrequency', this.pipelineFrequency === 'Never' ? 'Week' : this.pipelineFrequency, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('postDeploymentPipelineInterval', this.pipelineInterval === 'Never' ? '1' : this.pipelineInterval, DataStoreType.Public);

            if (this.recurrent == 'Never') {
                this.MS.DataStore.addToDataStore('historicalOnly', 'true', DataStoreType.Public);
            }
            else {
                this.MS.DataStore.addToDataStore('historicalOnly', 'false', DataStoreType.Public);
            }

            this.MS.DataStore.addToDataStore('pipelineFrequency', 'Month', DataStoreType.Public);
            this.MS.DataStore.addToDataStore('pipelineInterval', '1', DataStoreType.Public);
            this.MS.DataStore.addToDataStore('pipelineStart', '', DataStoreType.Public);
            this.MS.DataStore.addToDataStore('pipelineEnd', '', DataStoreType.Public);
            this.MS.DataStore.addToDataStore('pipelineType', 'PreDeployment', DataStoreType.Public);

            let url = this.MS.DataStore.getValue('SalesforceBaseUrl');

            if (url && url.split('/').length >= 3) {
                let urlParts = url.split('/');
                this.baseUrl = urlParts[0] + '//' + urlParts[2];
            }
        }

        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlSubGroup', 'SalesManagement', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlEntryName', 'BaseURL', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeBaseUrl', 'SqlEntryValue', this.baseUrl, DataStoreType.Public);

        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlSubGroup', 'SalesManagement', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlEntryName', 'FiscalMonthStart', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeFiscalMonth', 'SqlEntryValue', this.fiscalMonth, DataStoreType.Public);

        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeSourceApplication', 'SqlGroup', 'SolutionTemplate', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeSourceApplication', 'SqlSubGroup', 'SalesManagement', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeSourceApplication', 'SqlEntryName', 'SourceApplication', DataStoreType.Public);
        this.MS.DataStore.addToDataStoreWithCustomRoute('CustomizeSourceApplication', 'SqlEntryValue', this.sourceApplication, DataStoreType.Public);

        return true;
    }

    InvalidateEmails() {
        this.Invalidate();

        this.isValidated = this.emails.length === 0 || this.isEmailValidated;
    }
}