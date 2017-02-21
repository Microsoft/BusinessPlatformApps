import { ViewModelBase } from '../../../../../SiteCommon/Web/services/viewmodelbase';
import { ActionResponse } from '../../../../../SiteCommon/Web/services/actionresponse';
import { DataStoreType } from '../../../../../SiteCommon/Web/services/datastore';

import { QueryParameter } from '../../../../../SiteCommon/Web/base/query-parameter';

export class Gettingstarted extends ViewModelBase {
    architectureDiagram: string = '';
    downloadLink: string = '';
    isDownload: boolean = false;
    list1: string[] = [];
    list2: string[] = [];
    list1Title: string = this.MS.Translate.GETTING_STARTED_LIST_1;
    list2Title: string = this.MS.Translate.GETTING_STARTED_LIST_2;
    oauthType: string = '';
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
        this.isAuthenticated = false;
        if (!this.isDownload) {
            this.isAuthenticated = true;
            this.isValidated = true;
        } else {
            let queryParam = this.MS.UtilityService.GetItem('queryUrl');
            if (queryParam) {

                let token = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.CODE, queryParam);

                if (token === '') {
                    this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_UNKNOWN_ERROR;
                    this.MS.ErrorService.details = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.ERRORDESCRIPTION, queryParam);
                    this.MS.ErrorService.showContactUs = true;
                    return;
                }

                var tokenObj = { code: token };
                let authToken = await this.MS.HttpService.executeAsync('Microsoft-GetAzureToken', tokenObj);
                if (authToken.IsSuccess) {
                    this.isAuthenticated = true;
                    if (!this.registration) {
                        this.isValidated = true;
                        this.isDownload = true;
                    }
                    await this.MS.HttpService.executeAsync('Microsoft-PowerBiLogin');
                }
            }
        }

        if (this.isAuthenticated && this.isDownload) {
            this.GetDownloadLink();
        } else {
            this.registration = '';
        }

    }

    async connect() {
        this.MS.DataStore.addToDataStore('oauthType', this.oauthType, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('AADTenant', 'common', DataStoreType.Public);
        let response: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureAuthUri', {});

        window.location.href = response.Body.value;
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
            this.MS.HttpService.executeAsync(this.registrationAction, { isInvisible: true });
            this.registration = '';
            this.downloadLink = this.registrationDownload;
            this.isValidated = true;
        }
    }
}