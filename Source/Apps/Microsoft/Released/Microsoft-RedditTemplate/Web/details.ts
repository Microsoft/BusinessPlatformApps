import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base'

export class Details extends ViewModelBase {
    async onLoaded(): Promise<void> {
        this.isValidated = true;
    }
}