import { bindable } from 'aurelia-framework';
import { ViewModelBase } from '../services/view-model-base';

export class gettingstartedtemplate {
    @bindable viewmodel = null;
}

export class GettingStartedViewModel extends ViewModelBase {
    architectureDiagram: string = '';
    downloadLink: string;
    features: string[] = [];
    isDownload: boolean = false;
    isEvaluation: boolean = false;
    pricing: string[] = [];
    requirements: string[] = [];
    subTitle: string;
    templateName: string;

    constructor() {
        super();
        this.showBack = false;
        this.showPrivacy = true;
        this.subTitle = '';
        this.templateName = '';
    }

    async OnLoaded() {
        if (this.isDownload && !this.isEvaluation) {
            let response = await this.MS.HttpService.Execute('Microsoft-GetMsiDownloadLink', {});
            this.downloadLink = response.response.value;
        }
    }
}