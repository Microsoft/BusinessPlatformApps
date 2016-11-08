import { ViewModelBase } from '../services/viewmodelbase';

export class Gettingstarted extends ViewModelBase {
    architectureDiagram: string = '';
    downloadLink: string;
    features: string[] = [];
    isDownload: boolean = false;
    pricing: string[] = [];
    requirements: string[] = [];
    showPrivacy: boolean = true;
    subtitle: string = '';
    templateName: string = '';

    constructor() {
        super();
    }

    async OnLoaded() {
        if (this.isDownload) {
            let response = await this.MS.HttpService.executeAsync('Microsoft-GetMsiDownloadLink', {});
            this.downloadLink = response.Body.value;
        }
    }
}