import { ViewModelBase } from '../services/view-model-base';

export class KeyVaultLogin extends ViewModelBase {
    hasToken: boolean = false;
    oauthType: string = '';

    async connect(): Promise<void> {
        this.MS.UtilityService.connectToAzure(this.oauthType, this.MS.Translate.DEFAULT_TENANT);
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();

        if (this.hasToken) {
            this.setValidated();
        } else {
            await this.MS.UtilityService.getToken(this.oauthType, async () => {
                this.hasToken = this.setValidated();
            });
        }
    }
}