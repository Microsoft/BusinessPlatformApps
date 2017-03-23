import { bindable } from 'aurelia-framework';
import { ViewModelBase } from '../services/view-model-base';

export class progresstemplate {
    @bindable viewmodel = null;
}

export class ProgressViewModel extends ViewModelBase {
    finishedActionName: string = '';
    isDataPullDone: boolean = false;
    isPbixReady: boolean = false;
    pbixDownloadLink: string = '';
    recordCounts: any[] = [];
    showCounts: boolean = false;
    sliceStatus: any[] = [];
    sqlServerIndex: number = 0;
    successMessage: string = 'All done! You can now download your Power BI report and start exploring your data.';
    targetSchema: string = '';

    constructor() {
        super();
        this.showNext = false;
    }

    async OnLoaded() {
        if (!this.MS.DeploymentService.isFinished) {
            await this.MS.DeploymentService.ExecuteActions();
            this.finishedActionName = this.MS.DataService.GetDataStore()['FinishedActionName'][0];
            this.targetSchema = this.MS.DataService.GetDataStore()['TargetSchema'][0];
            this.QueryRecordCounts();
        }
    }

    async QueryRecordCounts() {
        if (this.showCounts && !this.isDataPullDone && !this.MS.DeploymentService.hasError) {
            let body: any = {};
            body.FinishedActionName = this.finishedActionName;
            body.IsWaiting = false;
            body.SqlServerIndex = this.sqlServerIndex;
            body.TargetSchema = this.targetSchema;

            let response = await this.MS.HttpService.Execute('Microsoft-GetDataPullStatus', body);
            if (response.isSuccess) {
                this.isDataPullDone = response.response.isFinished;
                this.recordCounts = response.response.status;
                this.sliceStatus = response.response.slices;
                if (this.finishedActionName !== '') {
                    this.QueryRecordCounts();
                } else {
                    this.isDataPullDone = true;
                }
            } else {
                this.MS.DeploymentService.hasError = true;
            }
        }
    }
}