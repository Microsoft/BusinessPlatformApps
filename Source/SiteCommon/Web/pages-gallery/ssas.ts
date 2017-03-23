import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class Customize extends ViewModelBase {
    ssasType: string = 'New';
    server: string = '';
    email: string = '';
    password: string = '';
    sku: string = 'S0';

    Invalidate() {
        super.Invalidate();
    }

    async OnLoaded() {
        this.isValidated = false;

        let response = await this.MS.HttpService.executeAsync('Microsoft-GetEmail', {});
        this.email = response.Body.value;
    }

    async OnValidate(): Promise<boolean> {
        this.showValidation = true;
        if (this.ssasType == "New") {
            if (this.server.length < 3 || this.server.length > 63 || !/[a-z]/.test(this.server[0]) || !/^[a-z0-9]+$/.test(this.server)) {
                this.MS.ErrorService.message = this.MS.Translate.SSAS_INVALID_SERVER_NAME;
                return false;
            }

            let body: any = {};
            body.ASServerName = this.server;
            let response = await this.MS.HttpService.executeAsync('Microsoft-CheckASServerNameAvailability', body);
            if (response.IsSuccess) {
                this.isValidated = true;
                return true;
            }

            this.isValidated = false;
            return false;
        } else {
            let body: any = {};
            body.ASServerUrl = this.server;
            body.ASAdmin = this.email;
            body.ASAdminPassword = this.password;

            let response = await this.MS.HttpService.executeAsync('Microsoft-ValidateConnectionToAS', body);
            if (response.IsSuccess) {
                this.isValidated = true;
                this.MS.DataStore.addToDataStore("ASServerUrl", this.server, DataStoreType.Public);
                return true;
            }

            this.isValidated = false;
            return false;
        }
    }

    async NavigatingNext(): Promise<boolean> {
        if (this.ssasType == "New") {
            let body: any = {};
            body.ASServerName = this.server;
            body.ASAdmin = this.email;
            body.ASAdminPassword = this.password;
            body.ASSku = this.sku;

            let response = await this.MS.HttpService.executeAsync('Microsoft-DeployAzureAnalysisServices', body);
            if (!response.IsSuccess) {
                return false;
            }

            this.server = this.MS.DataStore.getValue("ASServerUrl");
            this.ssasType = "Existing";

            // validate creds
            let body2: any = {};
            body2.ASAdmin = this.email;
            body2.ASAdminPassword = this.password;

            let response2 = await this.MS.HttpService.executeAsync('Microsoft-ValidateConnectionToAS', body2);
            if (!response2.IsSuccess) {
                return false;
            }
        }

        return true;
    }
}