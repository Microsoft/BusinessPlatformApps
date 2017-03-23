import Viewmodelbase = require("../../../../SiteCommon/Web/services/view-model-base");

export class DummyPage extends Viewmodelbase.ViewModelBase {
    architectureDiagram: string = '';
    downloadLink: string = '';
    isDownload: boolean = false;
    list1: string[] = [];
    list2: string[] = [];
    list1Title: string = this.MS.Translate.GETTING_STARTED_LIST_1;
    list2Title: string = this.MS.Translate.GETTING_STARTED_LIST_2;
    registration: string = '';
    registrationAccepted: boolean = false;
    registrationAction: string = '';
    registrationCompany: string = '';
    registrationDownload: string = '';
    registrationEmail: string = '';
    registrationEmailConfirmation: string = '';
    registrationEmailsToBlock: string = '';
    registrationError: string = '';
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
       
    }

    async OnLoaded() {
        this.isValidated = true;
        
    }

    async Register() {
    }
}