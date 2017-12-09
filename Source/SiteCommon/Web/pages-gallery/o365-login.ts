import { AzureLogin } from './azure-login';
import { DataStoreType } from "../enums/data-store-type";
import { ActionResponse } from "../models/action-response";
import { QueryParameter } from "../constants/query-parameter";

export class O365Login extends AzureLogin {
    permissions: any = {};

    async connect(): Promise<void> {
        this.MS.DataStore.addToDataStore('Permissions', this.permissions, DataStoreType.Public);
        let createADresponseBody: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-CreateADApplication');
        if (createADresponseBody.IsSuccess) {
            this.MS.DataStore.addObjectToDataStore(createADresponseBody.Body, DataStoreType.Private);
            await this.MS.UtilityService.connectToAzureSPN();
        }
    }

    async onLoaded(): Promise<void> {
        this.onInvalidate();

        let queryParam: any = this.getItem('queryUrl');
        if (queryParam) {
            let token = this.getQueryParameterFromUrl(QueryParameter.CODE, queryParam);
            if (token === '') {
                this.MS.ErrorService.set(this.MS.UtilityService.getQueryParameterFromUrl('error', queryParam),'',false,'');
            }
            else {
                this.setValidated();
            }

            this.MS.UtilityService.removeItem('queryUrl');
        }
    }

    getItem(key: any): any {
        let item: any = JSON.parse(window.sessionStorage.getItem(key));
        return item;
    }

    getQueryParameterFromUrl(name: any, url: any): string {
        var regex = new RegExp('[?&]' + name.replace(/[\[\]]/g, '\\$&') + '(=([^&#]*)|&|#|$)');
        var results = regex.exec(url);
        return (!results || !results[2])
            ? ''
            : decodeURIComponent(results[2].replace(/\+/g, ' '));
    }
}
