import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class WindowsAuth extends ViewModelBase {
    username: string = '';
    password: string = '';

    discoveredUsername: string = '';
    enteredUsername: string = '';
    logInAsCurrentUser: boolean = false;

    constructor() {
        super();
    }

    loginSelectionChanged() {
        this.Invalidate();
        if (this.logInAsCurrentUser) {
            this.enteredUsername = this.username;
            this.username = this.discoveredUsername;
        } else {
            if (!this.enteredUsername) {
                this.username = this.discoveredUsername;
            } else {
                this.username = this.enteredUsername;
            }
        }
    }

    async OnValidate(): Promise<boolean> {
        this.isValidated = false;
        let usernameError: string = this.MS.UtilityService.validateUsername(this.username);
        if (usernameError) {
            this.MS.ErrorService.message = usernameError;
            return false;
        }

        let domain: string = this.MS.UtilityService.extractDomain(this.username);
        let usernameWithoutDomain: string = this.MS.UtilityService.extractUsername(this.username);

        this.MS.DataStore.addToDataStore('ImpersonationDomain', domain, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('ImpersonationUsername', usernameWithoutDomain, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('ImpersonationPassword', this.password, DataStoreType.Private);

        let response = await this.MS.HttpService.executeAsync('Microsoft-ValidateNtCredential', {});
        if (response.IsSuccess) {
            this.isValidated = true;
        }

        super.OnValidate();
        return this.isValidated;
    }

    async OnLoaded(): Promise<void> {
        this.isValidated = false;

        if (!this.username) {
            var response = await this.MS.HttpService.executeAsync('Microsoft-GetCurrentUserAndDomain', {});
            this.discoveredUsername = response.Body.Value;
            this.loginSelectionChanged();
        }
   }
}