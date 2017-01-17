import { ProgressViewModel } from '../../common/Web/directives/progresstemplate';

export class ProgressVM extends ProgressViewModel {
    successMessage: string = 'All done! You can now download your Power BI report and start exploring your Twitter data.';

    constructor() {
        super();
    }

    async OnLoaded() {
        let body: any = {};
        body.FileName = 'TwitterSolutionTemplate.pbix';

        let response = await this.MS.HttpService.Execute('Microsoft-WranglePBI', body);
        if (response.isSuccess) {
            this.pbixDownloadLink = response.response.value;
            this.isPbixReady = true;
        }

        super.OnLoaded();
    }
}