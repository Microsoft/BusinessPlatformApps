import { ViewModelBase } from '../../../../../SiteCommon/Web/services/viewmodelbase'
import { DataStoreType } from '../../../../../SiteCommon/Web/services/datastore'

export class CognitiveText extends ViewModelBase {
    bingUrl: string = '';
    bingtermsofuse: string = '';
    cognitiveUrl: string = '';
    cognitivetermsofuse: string = '';

    isBingChecked: boolean = false;
    isCognitiveChecked: boolean = false;

    constructor() {
        super();
        this.isValidated = false;
    }

    verifyBing() {
        this.isValidated = this.isBingChecked && this.isCognitiveChecked;
    }

    verifyCognitive() {
        this.isValidated = this.isBingChecked && this.isCognitiveChecked;
    }

}