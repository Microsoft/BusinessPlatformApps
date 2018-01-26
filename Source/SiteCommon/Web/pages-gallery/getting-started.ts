import { DataStoreType } from '../enums/data-store-type';

import { Registration } from '../models/registration';
import { Selection } from '../models/selection';

import { ViewModelBase } from '../services/view-model-base';

export class Gettingstarted extends ViewModelBase {
    architectureDiagram: string = '';
    contact: Registration = new Registration();
    downloadLink: string = '';
    isDownload: boolean = false;
    list1: string[] = [];
    list2: string[] = [];
    list1Title: string = this.MS.Translate.GETTING_STARTED_LIST_1;
    list2Title: string = this.MS.Translate.GETTING_STARTED_LIST_2;
    prerequisiteDescription: string = '';
    prerequisiteLink: string = '';
    prerequisiteLinkText: string = '';
    registration: Registration = new Registration();
    selection: Selection = new Selection();
    showPrivacy: boolean = true;
    showSelection: boolean = false;
    subtitle: string = '';
    templateName: string = '';
    terms: Registration = new Registration();
    upgrade: boolean = false;

    async clickRegister(): Promise<void> {
        this.MS.ErrorService.clear();

        this.registration.nameFirst = this.registration.nameFirst.trim();
        this.registration.nameLast = this.registration.nameLast.trim();
        this.registration.company = this.registration.company.trim();
        this.registration.email = this.registration.email.trim();
        this.registration.emailConfirmation = this.registration.emailConfirmation.trim();

        if (this.registration.nameFirst.length === 0 ||
            this.registration.nameLast.length === 0 ||
            this.registration.company.length === 0 ||
            this.registration.email.length === 0 ||
            this.registration.email !== this.registration.emailConfirmation ||
            this.registration.email.indexOf('@') === -1) {
            this.MS.ErrorService.message = this.MS.Translate.GETTING_STARTED_REGISTRATION_ERROR;
        }

        if (!this.MS.ErrorService.message) {
            let emailsToBlock: string[] = this.registration.emailsToBlock.split(',');
            for (let i = 0; i < emailsToBlock.length && !this.MS.ErrorService.message; i++) {
                let emailToBlock: string = emailsToBlock[i].replace('.', '\\.');
                let pattern: any = new RegExp(`.*${emailToBlock}`);
                if (pattern.test(this.registration.email)) {
                    this.MS.ErrorService.message = this.MS.Translate.GETTING_STARTED_REGISTRATION_ERROR_EMAIL;
                }
            }
        }

        if (!this.MS.ErrorService.message) {
            this.MS.DataStore.addToDataStore('FirstName', this.registration.nameFirst, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('LastName', this.registration.nameLast, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('CompanyName', this.registration.company, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('RowKey', this.registration.email, DataStoreType.Public);
            this.MS.HttpService.executeAsync(this.registration.action, { isInvisible: true });
            this.registration.text = '';
            this.downloadLink = this.registration.download;
            this.isValidated = true;
        }
    }

    async getDownloadLink(): Promise<void> {
        let response: any = await this.MS.HttpService.getExecuteResponseAsync('Microsoft-GetMsiDownloadLink');
        if (this.registration.text) {
            this.registration.download = response;
        } else {
            this.downloadLink = response;
        }
    }

    async onLoaded(): Promise<void> {
        this.isValidated = true;

        if (this.MS.HttpService.isOnPremise) {
            this.selection.label = '';
            this.selection.choice = this.selection.choiceDownload;
            this.selectionChanged();
            this.upgrade = !!(await this.MS.HttpService.executeAsync('Microsoft-CheckVersion')).Body;
        }

        if (this.isDownload) {
            this.getDownloadLink();
        } else {
            this.registration.text = '';
        }
    }

    openNewMSILink(): void {
        window.open("https://bpsolutiontemplates.com/?name=Microsoft-SCCMTemplate");
    }

    selectionChanged(): void {
        if (this.selection.choice === this.selection.choiceDownload) {
            this.isDownload = !this.MS.HttpService.isOnPremise;
            this.selection.list1Previous = this.list1;
            this.selection.list2Previous = this.list2;
            this.list1 = this.selection.list1;
            this.list2 = this.selection.list2;
            if (this.isDownload && !this.downloadLink) {
                this.getDownloadLink();
            }
        } else {
            this.isDownload = false;
            this.list1 = this.selection.list1Previous;
            this.list2 = this.selection.list2Previous;
        }
    }
}