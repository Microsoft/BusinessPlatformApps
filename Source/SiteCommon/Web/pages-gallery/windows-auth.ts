import { DataStoreType } from '../enums/data-store-type';

import { ViewModelBase } from '../services/view-model-base';

export class WindowsAuth extends ViewModelBase {
    discoveredUsername: string = '';
    enteredUsername: string = '';
    logInAsCurrentUser: boolean = false;
    password: string = '';
    username: string = '';

    loginSelectionChanged(): void {
        this.onInvalidate();
        if (this.logInAsCurrentUser) {
            this.enteredUsername = this.username;
            this.username = this.discoveredUsername;
        } else {
            this.username = this.enteredUsername ? this.enteredUsername : this.discoveredUsername;
        }
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();

        if (!this.username) {
            this.discoveredUsername = await this.MS.HttpService.getExecuteResponseAsync('Microsoft-GetCurrentUserAndDomain', 'Value');
            this.loginSelectionChanged();
        }
    }

    async onValidate(): Promise<boolean> {
        this.onInvalidate();

        let usernameError: string = this.validateUsername(this.username);
        if (usernameError) {
            this.MS.ErrorService.message = usernameError;
        } else {
            this.MS.DataStore.addToDataStore('ImpersonationDomain', this.MS.UtilityService.extractDomain(this.username), DataStoreType.Public);
            this.MS.DataStore.addToDataStore('ImpersonationUsername', this.MS.UtilityService.extractUsername(this.username), DataStoreType.Public);
            this.MS.DataStore.addToDataStore('ImpersonationPassword', this.password, DataStoreType.Private);

            this.isValidated = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-ValidateNtCredential');
            this.showValidation = this.isValidated;
        }

        super.onValidate();

        return this.isValidated;
    }

    validateUsername(username: string): string {
        let error: string = this.MS.Translate.WINDOWS_AUTH_USERNAME_ERROR;
        if (username.includes('\\')) {
            error = '';
        } else if (username.length > 0) {
            error = this.MS.Translate.WINDOWS_AUTH_USERNAME_ERROR_2;
        }
        return error;
    }
}