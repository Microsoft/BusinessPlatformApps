import { ActionResponse } from '../models/action-response';
import { ViewModelBase } from '../services/view-model-base';

export class ProgressViewModel extends ViewModelBase {
    datastoreEntriesToValidate: string[] = [];
    downloadPbiText: string = this.MS.Translate.PROGRESS_DOWNLOAD_PBIX_INFO;
    enablePublishReport: boolean = false;
    filename: string = 'report.pbix';
    finishedActionName: string = '';
    hasPowerApp: boolean = false;
    isDataPullDone: boolean = false;
    isPowerAppReady: boolean = false;
    isUninstall: boolean = false;
    oauthType: string = 'powerbi';
    pbixDownloadLinks: string[] = [];
    powerAppDownloadLink: string = '';
    powerAppFileName: string = '';
    publishReportLinks: any[] = [];
    publishReportSucceeded: boolean = false;
    recordCounts: any[] = [];
    redirectInSameTab: boolean = false;
    selectedPBIWorkspaceId: string = '';
    showCounts: boolean = false;
    showPublishReport: boolean = false;
    showDownloadButton: boolean = true;
    showBackButton: boolean = true;
    sliceStatus: any[] = [];
    successMessage: string = this.MS.Translate.PROGRESS_ALL_DONE;
    successMessage2: string = this.MS.Translate.PROGRESS_ALL_DONE2;
    targetSchema: string = '';

    async executeActions(): Promise<void> {
        if (await this.MS.DeploymentService.executeActions() && !this.isUninstall) {
            this.MS.DeploymentService.isFinished = true;
            await this.getPbixPath();

            if (this.pbixDownloadLinks.length > 1)
                this.redirectInSameTab = false;

            let index: number = 1;
            for (let pbixDownloadLink of this.pbixDownloadLinks) {
                let response: string = await this.MS.HttpService.getResponseAsync('Microsoft-PublishPBIReportCDSA', {
                    PBIWorkspaceId: this.selectedPBIWorkspaceId,
                    PBIXLocation: pbixDownloadLink
                });

                await this.MS.HttpService.getResponseAsync('Microsoft-UpdatePBIParameters');   
                this.publishReportLinks.push({ 'url': response, 'index': index });
                index++;
            }

            this.publishReportSucceeded = true;
            this.showPublishReport = true;

            if (!this.showBackButton)
                this.showBackButtonOnFinalPage = false;
        }
    }

    async onLoaded(): Promise<void> {
        this.MS.DeploymentService.isFinished = false;

        let isDataStoreValid: boolean = true;

        for (let i = 0; i < this.datastoreEntriesToValidate.length && isDataStoreValid; i++) {
            if (this.MS.DataStore.getValue(this.datastoreEntriesToValidate[i]) === null) {
                this.MS.NavigationService.navigateHome();
                isDataStoreValid = false;
            }
        }

        if (isDataStoreValid) {
            this.executeActions();
        }
    }
    
    async getPbixPath(): Promise<void> {
        let response: ActionResponse = null;

        response = await this.MS.HttpService.executeAsync('Microsoft-GetPbixPath', { FileName: this.filename });
        
        if (response.IsSuccess) {
            this.pbixDownloadLinks = JSON.parse(response.Body.Value);
        }
        else {
            this.MS.ErrorService.set(this.MS.Translate.CDSA_GET_PBIX_PATH_FAILED);
        }
    }
}