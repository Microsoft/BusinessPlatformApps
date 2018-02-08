import { DataStoreType } from '../enums/data-store-type';
import { ViewModelBase } from '../services/view-model-base';

export class LithiumLogin extends ViewModelBase {
    lithiumTenantId: string = '';
    lithiumClientId: string = '';
    lithiumClientSecret: string = '';
    lithiumRefreshToken: string = '';

    async onNavigatingNext(): Promise<boolean> {
        let tId: string = '';
        let clientID: string = '';
        let clientSecret: string = '';
        let refreshToken: string = '';

        if (this.lithiumTenantId != '' && this.lithiumClientId != '' && this.lithiumClientSecret != '' && this.lithiumRefreshToken != '') {
            tId = this.lithiumTenantId;
            clientID = this.lithiumClientId;
            clientSecret = this.lithiumClientSecret;
            refreshToken = this.lithiumRefreshToken;
        }

        this.MS.DataStore.addToDataStore("LithiumTenantId", tId, DataStoreType.Private);
        this.MS.DataStore.addToDataStore("LithiumClientId", clientID, DataStoreType.Private);
        this.MS.DataStore.addToDataStore("LithiumClientSecret", clientSecret, DataStoreType.Private);
        this.MS.DataStore.addToDataStore("LithiumRefreshToken", refreshToken, DataStoreType.Private);
        this.MS.DataStore.addToDataStore("Filename", "SocialAnalyticsforLithium.pbix", DataStoreType.Public);
        this.MS.DataStore.addToDataStore("Schema", "it", DataStoreType.Public);

        return true;
    }

    async onValidate(): Promise<boolean> {
        this.onInvalidate();

        if (this.lithiumTenantId.length > 0 && this.lithiumClientId.length > 0 && this.lithiumClientSecret.length > 0 && this.lithiumRefreshToken.length > 0) {
            this.isValidated = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-ValidateLithiumCredentials', {
                LithiumTenantId: this.lithiumTenantId,
                LithiumClientId: this.lithiumClientId,
                LithiumClientSecret: this.lithiumClientSecret,
                LithiumRefreshToken: this.lithiumRefreshToken
            });
        }

        this.showValidation = this.isValidated;
        return this.isValidated;
    }
}