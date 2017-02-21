import { DataStoreType } from '../services/datastore';
import { ViewModelBase } from '../services/viewmodelbase';

export class Gettingstarted extends ViewModelBase {
    architectureDiagram: string = '';
    downloadLink: string = '';
    isDownload: boolean = false;
    list1: string[] = [];
    list2: string[] = [];
    list1Title: string = this.MS.Translate.GETTING_STARTED_LIST_1;
    list2Title: string = this.MS.Translate.GETTING_STARTED_LIST_2;
    prerequisiteDescription: string = '';
    prerequisiteLink: string = '';
    prerequisiteLinkText: string = '';
    registration: string = '';
    registrationAccepted: boolean = false;
    registrationAction: string = '';
    registrationCompany: string = '';
    registrationContactLink: string = '';
    registrationContactLinkText: string = '';
    registrationDownload: string = '';
    registrationEmail: string = '';
    registrationEmailConfirmation: string = '';
    registrationEmailsToBlock: string = '';
    registrationEulaLink: string = '';
    registrationEulaLinkText: string = '';
    registrationLink: string = '';
    registrationNameFirst: string = '';
    registrationNameLast: string = '';
    registrationPrivacy: string = '';
    registrationPrivacyTitle: string = '';
    registrationValidation: string = '';
    showPrivacy: boolean = true;
    subtitle: string = '';
    templateName: string = '';

    constructor() {
        super();
    }

    async GetDownloadLink() {
        let response = await this.MS.HttpService.executeAsync('Microsoft-GetMsiDownloadLink');
        if (this.registration) {
            this.registrationDownload = response.Body.value;
        } else {
            this.downloadLink = response.Body.value;
        }
    }

    async OnLoaded() {
        this.isValidated = true;
        if (this.isDownload) {
            this.GetDownloadLink();
        } else {
            this.registration = '';
        }
    }

    async Register() {
        this.MS.ErrorService.Clear();

        this.registrationNameFirst = this.registrationNameFirst.trim();
        this.registrationNameLast = this.registrationNameLast.trim();
        this.registrationCompany = this.registrationCompany.trim();
        this.registrationEmail = this.registrationEmail.trim();
        this.registrationEmailConfirmation = this.registrationEmailConfirmation.trim();

        if (this.registrationNameFirst.length === 0 ||
            this.registrationNameLast.length === 0 ||
            this.registrationCompany.length === 0 ||
            this.registrationEmail.length === 0 ||
            this.registrationEmail !== this.registrationEmailConfirmation ||
            this.registrationEmail.indexOf('@') === -1) {
            this.MS.ErrorService.message = this.MS.Translate.GETTING_STARTED_REGISTRATION_ERROR;
        }

        if (!this.MS.ErrorService.message) {
            let emailsToBlock: string[] = this.registrationEmailsToBlock.split(',');
            for (let i = 0; i < emailsToBlock.length && !this.MS.ErrorService.message; i++) {
                let emailToBlock: string = emailsToBlock[i].replace('.', '\\.');
                let pattern: any = new RegExp(`.*${emailToBlock}`);
                if (pattern.test(this.registrationEmail)) {
                    this.MS.ErrorService.message = this.MS.Translate.GETTING_STARTED_REGISTRATION_ERROR_EMAIL;
                }
            }
        }

        if (!this.MS.ErrorService.message) {
            this.MS.DataStore.addToDataStore('FirstName', this.registrationNameFirst, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('LastName', this.registrationNameLast, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('CompanyName', this.registrationCompany, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('RowKey', this.registrationEmail, DataStoreType.Public);
            this.MS.HttpService.executeAsync(this.registrationAction, { isInvisible: true });
            this.registration = '';
            this.downloadLink = this.registrationDownload;
            this.isValidated = true;
        }
    }
}