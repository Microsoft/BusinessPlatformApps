import { ViewModelBase } from '../services/view-model-base';
import { PBIWorkspace } from '../models/pbi-workspace';
import { DataStoreType } from '../enums/data-store-type';
import { AzureConnection } from '../enums/azure-connection';
import { QueryParameter } from '../constants/query-parameter';

export class Customize extends ViewModelBase {
    pbiWorkspaces: PBIWorkspace[] = [];
    selectedPBIWorkspaceId: string = '';
    authToken: any = {};
    azureDirectory: string = '';
    createWorkspaceIfEmpty: boolean = false;
    creatingWorkspace: boolean = false;
    connectionType: AzureConnection = AzureConnection.Organizational;
    oauthType: string = 'powerbi';

    async connect(): Promise<void> {
        this.MS.UtilityService.connectToAzure(this.oauthType, this.isConnectionMicrosoft() ? this.azureDirectory : this.MS.Translate.DEFAULT_TENANT);        
    }

    isConnectionMicrosoft(): boolean {
        return this.connectionType.toString() === AzureConnection.Microsoft.toString();
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();

        this.textNext = 'Deploy';

        await this.getToken(this.oauthType, async () => {
            await this.getInstances();
            if (this.pbiWorkspaces.length > 0) {
                this.selectedPBIWorkspaceId = this.pbiWorkspaces[0].id;
            }
        });

        if (this.pbiWorkspaces && this.pbiWorkspaces.length > 0) {
            this.isValidated = true;
        }
    }

    async getInstances(): Promise<void> {
        if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-GetPBIClusterUri')) {
            this.pbiWorkspaces = await this.MS.HttpService.getResponseAsync('Microsoft-GetPBIWorkspacesCDSA');

            if (this.pbiWorkspaces.length == 0) {
                if (this.createWorkspaceIfEmpty) {
                    this.creatingWorkspace = true;
                    this.MS.DataStore.addToDataStore('pbiWorkspaceName', this.getWorkspaceName(), DataStoreType.Private);
                    let response = await this.MS.HttpService.getResponseAsync('Microsoft-CreatePBIWorkspace');
                    this.pbiWorkspaces.push(response);
                    this.creatingWorkspace = false;

                    if (this.pbiWorkspaces.length == 0) {
                        this.MS.ErrorService.set(this.MS.Translate.POWER_BI_CREATING_WORKSPACE_FAILED);
                    }
                }
                else {
                    this.MS.ErrorService.set(this.MS.Translate.POWER_BI_APP_WORKSPACE);
                }
            }
        }
    }

    async getToken(openAuthorizationType: string, callback: () => Promise<void>): Promise<void> {
        let queryParam: any = this.MS.UtilityService.getItem('queryUrl');
        if (queryParam) {
            let token = this.MS.UtilityService.getQueryParameterFromUrl(QueryParameter.CODE, queryParam);
            if (token == '') {
                this.pbiWorkspaces = [];
                this.MS.ErrorService.set(this.MS.Translate.POWER_BI_UNKNOWN_ERROR, this.MS.UtilityService.getQueryParameterFromUrl(QueryParameter.ERROR_DESCRIPTION, queryParam));                
            } else {
                if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-GetAzureToken', { code: token, oauthType: openAuthorizationType })) {
                    await callback();
                }
            }
            this.MS.UtilityService.removeItem('queryUrl');
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStore('PBIWorkspaceId', this.selectedPBIWorkspaceId, DataStoreType.Public);        
        return true;
    }

    getWorkspaceName(): string {
        return 'Cuna Insights ' + (Math.floor(Math.random() * 1000000)).toString();
    }
}