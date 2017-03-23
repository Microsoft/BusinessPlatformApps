import { QueryParameter } from '../constants/query-parameter';

import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';

import { AzureLogin } from './azure-login';

export class KeyVaultLogin extends AzureLogin {
    hasToken: boolean = false;

    constructor() {
        super();
    }

    async OnLoaded() {
        this.isValidated = false;
        this.showValidation = false;

        if (this.hasToken) {
            this.isValidated = true;
            this.showValidation = true;
        } else {
            let queryParam = this.MS.UtilityService.GetItem('queryUrl');
            if (queryParam) {
                let token = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.CODE, queryParam);
                if (token === '') {
                    this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_UNKNOWN_ERROR;
                    this.MS.ErrorService.details = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.ERRORDESCRIPTION, queryParam);
                    this.MS.ErrorService.showContactUs = true;
                    return;
                }
                var tokenObj = {
                    code: token
                };
                this.authToken = await this.MS.HttpService.executeAsync('Microsoft-GetAzureToken', tokenObj);
                if (this.authToken.IsSuccess) {
                    this.MS.DataStore.addToDataStore('AzureTokenKV',
                        this.authToken.Body.AzureToken,
                        DataStoreType.Private);

                    this.hasToken = true;
                    this.isValidated = true;
                    this.showValidation = true;
                }
                this.MS.UtilityService.RemoveItem('queryUrl');
            }
        }
    }

    async connect() {
        this.MS.DataStore.addToDataStoreWithCustomRoute('dynamics365login-', 'oauthType', this.oauthType, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('AADTenant', 'common', DataStoreType.Public);
        let response: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureAuthUri', {});
        window.location.href = response.Body.value;
    }

    public async NavigatingNext(): Promise<boolean> {
        return true;
    }
}