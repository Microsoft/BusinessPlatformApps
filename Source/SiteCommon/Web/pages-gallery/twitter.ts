import { QueryParameter } from '../constants/query-parameter';

import { ActionStatus } from '../enums/action-status';
import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class Twitter extends ViewModelBase {
    authToken: any = {};
    isAuthenticated: boolean = false;
    selectedSubscriptionId: string;
    subscriptionsList: any[];

    constructor() {
        super();
    }

    async OnLoaded(){
        this.isAuthenticated = false;
        this.isValidated = false;
        this.showValidation = false;

        let queryParam = this.MS.UtilityService.GetItem('queryUrl');
        if (queryParam) {
            let code = this.MS.UtilityService.GetQueryParameterFromUrl(QueryParameter.CODE, queryParam);
            if (code) {
                this.MS.DataStore.addToDataStore('TwitterCode', code, DataStoreType.Private);

                let response = await this.MS.HttpService.executeAsync('Microsoft-ConsentTwitterConnectionToLogicApp', {});
                if (response.IsSuccess) {
                    this.isAuthenticated = true;
                    this.isValidated = true;
                    this.showValidation = true;
                }
            } else {
                // Do existing flow
                let response = await this.MS.HttpService.executeAsync('Microsoft-VerifyTwitterConnection', {});
                if (response.Status === ActionStatus.FailureExpected) {
                    this.MS.ErrorService.details = '';
                    this.MS.ErrorService.message = '';
                }

                if (response.IsSuccess) {
                    this.isAuthenticated = true;
                    this.isValidated = true;
                    this.showValidation = true;
                }
            }

            this.MS.UtilityService.RemoveItem('queryUrl');
        } else {
            // No redirect was present, dont bother checking
            // We still check for now
            let response = await this.MS.HttpService.executeAsync('Microsoft-VerifyTwitterConnection', {});
            this.MS.ErrorService.details = '';
            this.MS.ErrorService.message = '';
            if (response.IsSuccess) {
                this.isAuthenticated = true;
                this.isValidated = true;
                this.showValidation = true;
            }
        }
    }

    async connect() {
        if (!this.isAuthenticated) {
            let response = await this.MS.HttpService.executeAsync('Microsoft-CreateTwitterConnectionToLogicApp', {});
            if (response.IsSuccess) {
                window.location.href = response.Body['Consent']['value'][0]['link'];
            }
        }
    }
}