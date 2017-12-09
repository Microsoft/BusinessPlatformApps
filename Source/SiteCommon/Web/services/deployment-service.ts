import { ActionStatus } from '../enums/action-status';
import { DataStoreType } from '../enums/data-store-type';
import { ExperienceType } from '../enums/experience-type';

import { InitParser } from './init-parser';
import { MainService } from './main-service';

export class DeploymentService {
    MS: MainService;
    actions: any[] = [];
    executingIndex: number = -1;
    executingAction: any = {};
    experienceType: ExperienceType;
    hasError: boolean = false;
    isFinished: boolean = false;
    message: string = '';
    progressPercentage: number = 0;

    constructor(MainService: MainService) {
        this.MS = MainService;
    }

    //async executeActions(showCounts: boolean = false): Promise<boolean> {
    async executeActions(): Promise<boolean> {
        if (this.isFinished && !this.hasError) {
            return false;
        }

        this.hasError = false;
        this.isFinished = false;

        if (this.experienceType === ExperienceType.Uninstall) {
            this.MS.LoggerService.trackUninstallStart();
        }
        if (this.experienceType === ExperienceType.Install) {
            this.MS.LoggerService.trackDeploymentStart();
        }

        let lastActionStatus: ActionStatus = ActionStatus.Success;
        this.MS.DataStore.DeploymentIndex = '';
        this.progressPercentage = 0;

        for (let i = 0; i < this.actions.length && !this.hasError; i++) {
            this.MS.DataStore.DeploymentIndex = i.toString();
            this.executingIndex = i;
            this.executingAction = this.actions[i];
            this.progressPercentage = i / this.actions.length * 100;

            let param: any = {};
            if (lastActionStatus !== ActionStatus.BatchWithState) {
                param = this.MS.UtilityService.clone(this.actions[i].AdditionalParameters);
            }

            InitParser.loadVariables(param, param, this.MS, this);

            // Skip action if requested to do so by variable
            if (param && param.skip && param.skip.toString().toLowerCase() === 'true') {
                continue;
            }

            this.MS.LoggerService.trackDeploymentStepStartEvent(i, this.actions[i].OperationName);
            let response = await this.MS.HttpService.executeAsync(this.actions[i].OperationName, param);
            this.message = '';

            this.MS.LoggerService.trackDeploymentStepStopEvent(i, this.actions[i].OperationName, response.IsSuccess);

            if (!(response.IsSuccess)) {
                this.hasError = true;
                break;
            }

            this.MS.DataStore.addObjectToDataStore(response.Body, DataStoreType.Private);
            if (response.Status === ActionStatus.BatchWithState || response.Status === ActionStatus.InProgress) {
                i = i - 1; // Loop again but dont add parameter back
            }

            lastActionStatus = response.Status;
        }

        this.MS.DataStore.DeploymentIndex = '';
        if (this.hasError) {
            this.message = 'Error';
        } else {
            this.executingAction = {};
            this.executingIndex++;
            this.message = 'Success';
            this.progressPercentage = 100;
        }

        if (this.experienceType === ExperienceType.Uninstall) {
            this.MS.LoggerService.trackUninstallEnd(!this.hasError);
        }
        //if (this.experienceType === ExperienceType.Install && !showCounts) {
        if (this.experienceType === ExperienceType.Install) {
            this.MS.LoggerService.trackDeploymentEnd(!this.hasError);
        }
        this.isFinished = true;

        if (this.experienceType === ExperienceType.Uninstall && !this.hasError) {
            this.MS.HttpService.close();
        }

        return !this.hasError;
    }

    init(actionsJson: any): void {
        for (let i = 0; i < actionsJson.length; i++) {
            actionsJson[i].DisplayName = InitParser.translateInitValue(actionsJson[i].DisplayName, this.MS);
        }
        this.actions = actionsJson;
    }
}