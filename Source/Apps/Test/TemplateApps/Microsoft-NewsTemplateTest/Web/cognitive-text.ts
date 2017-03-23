import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type'

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base'

export class CognitiveText extends ViewModelBase {
    isBingChecked: boolean = false;

    constructor() {
        super();
        this.isValidated = false;
    }

    verifyBing() {
        this.isValidated = this.isBingChecked;
    }
}