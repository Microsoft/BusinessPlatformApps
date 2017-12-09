import { AzureConnection } from '../enums/azure-connection';
import { ViewModelBase } from '../services/view-model-base';

export class AxLogin extends ViewModelBase {
    authToken: any = {};
    azureDirectory: string = '';
    connectionType: AzureConnection = AzureConnection.Organizational;
    orgList: any[] = [];
    oauthType: string = 'axerp';
    
    async connect(): Promise<void> {
        this.MS.UtilityService.connectToAzure(this.oauthType, this.isConnectionMicrosoft() ? this.azureDirectory : this.MS.Translate.DEFAULT_TENANT);
    }

    isConnectionMicrosoft(): boolean {
        return this.connectionType.toString() === AzureConnection.Microsoft.toString();
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();
        
        if (this.orgList.length > 0) {
            this.setValidated();
        } else {
            await this.MS.UtilityService.getToken(this.oauthType, async () => { this.setValidated(); });
        }
    }
}