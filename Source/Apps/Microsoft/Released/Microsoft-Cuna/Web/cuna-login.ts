import { ActionResponse } from "../../../../../SiteCommon/Web/models/action-response";

import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type';
import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base';

export class CunaLogin extends ViewModelBase {

    public async onClickLogin(): Promise<void> {
        await this.MS.UtilityService.connectToCuna();
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();
        
        if (await this.validateCunaToken()) {
            this.setValidated();
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        return await this.validateCunaToken();
    }

    async validateCunaToken(): Promise<boolean> {
        let token = this.getCunaToken();
        if (token && token !== '') {
            this.MS.DataStore.addToDataStore('CunaSamlAuthToken', token, DataStoreType.Private);
            let isValidToken: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-ValidateCunaToken');
            if (isValidToken.IsSuccess) {
                this.MS.UtilityService.removeItem('queryUrl');
                return true;
            }
        }
        return false;
    }

    getCunaToken(): string {
        let authCodeName = 'samlAuthCode';
        let authCode:any = this.MS.UtilityService.getItem(authCodeName);        
        return authCode;
    }
}
