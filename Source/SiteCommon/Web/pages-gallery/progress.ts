import { ViewModelBase } from '../services/view-model-base';

export class ProgressViewModel extends ViewModelBase {
    datastoreEntriesToValidate: string[] = [];
    downloadPbiText: string = this.MS.Translate.PROGRESS_DOWNLOAD_PBIX_INFO;
    filename: string = 'report.pbix';
    filenameSSAS: string = 'reportSSAS.pbix';
    asDatabase: string = 'Sccm';
    finishedActionName: string = '';
    hasPowerApp: boolean = false;
    isDataPullDone: boolean = false;
    isPbixReady: boolean = false;
    isPowerAppReady: boolean = false;
    isUninstall: boolean = false;
    pbixDownloadLink: string = '';
    powerAppDownloadLink: string = '';
    powerAppFileName: string = '';
    recordCounts: any[] = [];
    showCounts: boolean = false;
    sliceStatus: any[] = [];
    sqlServerIndex: number = 0;
    successMessage: string = this.MS.Translate.PROGRESS_ALL_DONE;
    targetSchema: string = '';

    constructor() {
        super();
    }

    async OnLoaded(): Promise<void> {
        if (this.MS.DataStore.getValue('HasNavigated') == null) {
            this.MS.NavigationService.NavigateHome();
            return;
        }

        for (let i = 0; i < this.datastoreEntriesToValidate.length; i++) {
            if (this.MS.DataStore.getValue(this.datastoreEntriesToValidate[i]) === null) {
                this.MS.NavigationService.NavigateHome();
                return;
            }
        }

        this.hasPowerApp = this.hasPowerApp && this.MS.DataStore.getValue('SkipPowerApp') == null;

        // Run all actions
        let success: boolean = await this.MS.DeploymentService.ExecuteActions();

        if (!success) {
            return;
        }

        if (!this.isUninstall) {
            let body: any = {};
            let ssas = this.MS.DataStore.getValue("ssasDisabled");
            let response = null;
            if (ssas && ssas === 'false') {
                body.FileNameSSAS = this.filenameSSAS;
                body.ASDatabase = this.asDatabase;
                response = await this.MS.HttpService.executeAsync('Microsoft-WranglePBISSAS', body);
            } else {
                body.FileName = this.filename;
                body.SqlServerIndex = this.sqlServerIndex;
                response = await this.MS.HttpService.executeAsync('Microsoft-WranglePBI', body);
            }

            if (response.IsSuccess) {
                this.pbixDownloadLink = response.Body.value;
                this.isPbixReady = true;
            }

            if (this.hasPowerApp) {
                let bodyPowerApp: any = {};
                bodyPowerApp.PowerAppFileName = this.powerAppFileName;
                let responsePowerApp = await this.MS.HttpService.executeAsync('Microsoft-WranglePowerApp', bodyPowerApp);

                if (responsePowerApp.IsSuccess && responsePowerApp.Body.value) {
                    this.isPowerAppReady = true;
                    this.powerAppDownloadLink = responsePowerApp.Body.value;
                } else {
                    this.hasPowerApp = false;
                }
            }

            this.QueryRecordCounts();
        }
    }

    async QueryRecordCounts(): Promise<void> {
        if (this.showCounts && !this.isDataPullDone && !this.MS.DeploymentService.hasError) {
            let body: any = {};
            body.FinishedActionName = this.finishedActionName;
            body.IsWaiting = false;
            body.SqlServerIndex = this.sqlServerIndex;
            body.TargetSchema = this.targetSchema;

            let response = await this.MS.HttpService.executeAsync('Microsoft-GetDataPullStatus', body);
            if (response.IsSuccess) {
                this.recordCounts = response.Body.status;
                this.sliceStatus = response.Body.slices;

                this.isDataPullDone = response.Body.isFinished;
                this.QueryRecordCounts();

            } else {
                this.MS.DeploymentService.hasError = true;
            }
        }
    }
}