import { ViewModelBase } from '../services/view-model-base';

export class ASLogin extends ViewModelBase {
    hasToken: boolean = false;

    async connect(): Promise<void> {
        await this.MS.UtilityService.connectToAzure(this.openAuthorizationType.AAS);
    }

    async onLoaded(): Promise<void> {
        this.onInvalidate();

        if (this.hasToken) {
            this.setValidated();
        } else {
            await this.MS.UtilityService.getToken(this.openAuthorizationType.AAS, async () => {
                this.hasToken = this.setValidated();
            });
        }
    }
}