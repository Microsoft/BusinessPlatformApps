import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base'

export class CognitiveText extends ViewModelBase {
    async onLoaded(): Promise<void> {
        this.isValidated = false;
    }
}