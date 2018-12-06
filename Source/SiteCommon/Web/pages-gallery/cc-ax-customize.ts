import { DataStoreType } from '../enums/data-store-type';
import { AzureConnection } from '../enums/azure-connection';
import { ViewModelBase } from '../services/view-model-base';
import { ActionResponse } from "../models/action-response";
import { QueryParameter } from '../constants/query-parameter';

export class Customize extends ViewModelBase {
    authToken: any = {};
    azureDirectory: string = '';
    connectionType: AzureConnection = AzureConnection.Organizational;
    oauthType: string = 'axerp';
    axBaseUrl: string = '';
    urlRegex: RegExp;

    async onLoaded(): Promise<void> {
        super.onLoaded();

        if (this.axBaseUrl === this.MS.DataStore.getValue('ValidatedAxInstanceName')) {    
            this.validationText = this.MS.Translate.NAVIGATION_SUCCESSFULLY_CONNECTED;
            this.setValidated();
        } 

        if (this.axBaseUrl == '' && this.MS.DataStore.getValue('AxInstanceName') != null) {
            this.axBaseUrl = this.MS.DataStore.getValue('AxInstanceName', DataStoreType.Public);
        }

        this.urlRegex = /^(ht|f)tps:\/\/[a-z0-9-\.]+\.[a-z]{2,4}\/?([^\s<>\#%\,\{\}\\|\\\^\[\]]+)?\/$/;
        
        await this.getToken(this.oauthType, async () => {
            if (await this.validateAxInstance()) {
                this.validationText = this.MS.Translate.NAVIGATION_SUCCESSFULLY_CONNECTED;
                this.setValidated();
                this.MS.DataStore.addToDataStore('ValidatedAxInstanceName', this.axBaseUrl, DataStoreType.Public);
            }
            else {
                this.MS.ErrorService.message = this.MS.Translate.AX_LOGIN_UNKNOWN_ERROR;
            }
        });        
    }

    async connect(): Promise<void> {        
        if (this.axBaseUrl) {
            if (!this.axBaseUrl.endsWith('/')) {
                this.axBaseUrl = this.axBaseUrl + '/';
            }
            if (this.urlRegex.test(this.axBaseUrl)) {
                this.MS.DataStore.addToDataStore('AxInstanceName', this.axBaseUrl, DataStoreType.Public);
                // TODO: Temporary workaround to make use of specific tenant                
                this.azureDirectory = 'contosoax7.onmicrosoft.com';
                this.MS.UtilityService.connectToAzure(this.oauthType, this.azureDirectory);
            } else {
                this.MS.ErrorService.message = 'Validation failed. The url address ' + this.axBaseUrl + ' is not valid.';
            }
        }     
    }

    async getToken(openAuthorizationType: string, callback: () => Promise<void>): Promise<void> {
        let queryParam: any = this.MS.UtilityService.getItem('queryUrl');
        if (queryParam) {
            let token = this.MS.UtilityService.getQueryParameterFromUrl(QueryParameter.CODE, queryParam);
            if (token === '') {
                if (this.axBaseUrl != this.MS.DataStore.getValue('ValidatedAxInstanceName')) {
                    this.MS.ErrorService.set(this.MS.Translate.AX_LOGIN_UNKNOWN_ERROR, this.MS.UtilityService.getQueryParameterFromUrl(QueryParameter.ERROR_DESCRIPTION, queryParam));
                }
            } else {
                if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-GetAzureToken', { code: token, oauthType: openAuthorizationType })) {
                    await callback();
                }
            }
            this.MS.UtilityService.removeItem('queryUrl');
        }
    }    

    async validateAxInstance(): Promise<boolean> {
        let validInstance: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-ValidateAxInstanceCDSA');
        if (validInstance.IsSuccess)
            return true;
        return false;
    }

    async onChange(): Promise<any> {
        if (this.axBaseUrl === this.MS.DataStore.getValue('ValidatedAxInstanceName')) {
            this.validationText = this.MS.Translate.NAVIGATION_SUCCESSFULLY_CONNECTED;
            this.setValidated();            
        } else {            
            let connecttodynamics365: any = this.MS.UtilityService.getItem('connecttodynamics365');
            connecttodynamics365.axBaseUrl = this.axBaseUrl;
            this.MS.UtilityService.saveItem('connecttodynamics365', connecttodynamics365);

            this.isValidated = false;
            this.onInvalidate();
        }
    }
}