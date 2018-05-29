import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';
import { AzureLocation } from '../models/azure-location';

import { ViewModelBase } from '../services/view-model-base';

export class Sql extends ViewModelBase {
    azureGovtSuffix: string = '.database.usgovcloudapi.net';
    azureLocations: AzureLocation[] = [];
    azureSqlSuffix: string = '.database.windows.net';
    checkSqlVersion: boolean = false;
    credentialTarget: string = '';
    database: string = null;
    databases: string[] = [];
    hideSqlAuth: boolean = false;
    invalidUsernames: string[] = ['admin', 'administrator', 'dbmanager', 'dbo', 'guest', 'loginmanager', 'public', 'root', 'sa'];
    isAzureSql: boolean = false;
    isCreateAzureSqlSelected: boolean = false;
    isGovAzureSql: boolean = false;
    isWindowsAuth: boolean = true;
    newSqlDatabase: string = null;
    password: string = '';
    passwordConfirmation: string = '';
    showAllWriteableDatabases: boolean = true;
    showAzureSql: boolean = true;
    showCreateAzureSqlPrompt: boolean = false;
    showCredsWhenWindowsAuth: boolean = false;
    showDatabases: boolean = false;
    showGovAzure: boolean = false;
    showNewSqlOption: boolean = false;
    showSkuS1: boolean = true;
    showSqlRecoveryModeHint: boolean = false;
    sqlInstance: string = 'ExistingSql';
    sqlLocation: string = '';
    sqlServer: string = '';
    sqlSku: string = 'S1';
    subtitle: string = '';
    title: string = '';
    useImpersonation: boolean = false;
    username: string = '';
    validateWindowsCredentials: boolean = false;
    validationTextBox: string = '';

    onAuthChange(): void {
        this.onInvalidate();

        this.password = '';
        this.username = '';
    }

    onInvalidate(): void {
        super.onInvalidate();

        this.database = null;
        this.databases = [];
        this.showDatabases = false;
    }

    async onLoaded(): Promise<void> {
        this.onInvalidate();

        if (this.showNewSqlOption) {
            if (!this.azureLocations || this.azureLocations.length === 0) {
                let locationsResponse: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetLocations');
                if (locationsResponse.IsSuccess) {
                    this.azureLocations = locationsResponse.Body.value;
                    if (!this.sqlLocation && this.azureLocations && this.azureLocations.length > 23) {
                        this.sqlLocation = this.azureLocations[23].Name;
                    }
                }
            }

            this.sqlLocation = this.sqlLocation || 'westus2';
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        let body = this.getBody(true);
        let response: ActionResponse = null;

        if (this.sqlInstance === 'ExistingSql') {
            response = await this.MS.HttpService.executeAsync('Microsoft-GetSqlConnectionString', body);
            this.MS.DataStore.addToDataStore('Database', this.database, DataStoreType.Public);
        } else if (this.sqlInstance === 'NewSql') {
            response = await this.createDatabaseServer();
            this.MS.DataStore.addToDataStore('Database', this.newSqlDatabase, DataStoreType.Public);
        }

        if (response && response.IsSuccess) {
            this.MS.DataStore.addToDataStore('SqlConnectionString', response.Body.value, DataStoreType.Private);
            this.MS.DataStore.addToDataStore('Server', this.getSqlServer(), DataStoreType.Public);
            this.MS.DataStore.addToDataStore('Username', this.username, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('Password', this.password, DataStoreType.Private);

            if (this.sqlInstance === 'ExistingSql') {
                this.MS.HttpService.executeAsync('Microsoft-ExistingSqlServer', { isInvisible: true });
            }

            if (this.checkSqlVersion) {
                isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CheckSQLVersion', body);
            }

            if (isSuccess) {
                if (this.useImpersonation) {
                    this.MS.DataStore.addToDataStore('CredentialTarget', this.credentialTarget, DataStoreType.Private);
                    this.MS.DataStore.addToDataStore('CredentialUsername', this.username, DataStoreType.Private);
                    this.MS.DataStore.addToDataStore('CredentialPassword', this.password, DataStoreType.Private);
                    isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CredentialManagerWrite', body);
                }

                this.MS.DataStore.addToDataStore('azureSqlDisabled', this.isAzureSql || this.isGovAzureSql ? 'false' : 'true', DataStoreType.Public);

                if (this.showCreateAzureSqlPrompt) {
                    this.MS.DataStore.addTestToDataStore('CreateAzureSql', this.isCreateAzureSqlSelected);
                }
            }
        } else {
            isSuccess = false;
        }

        return isSuccess;
    }

    async onValidate(): Promise<boolean> {
        let oldDatabase: string = this.database;

        this.onInvalidate();

        this.sqlServer = this.sqlServer.toLowerCase();
        if (this.sqlInstance === 'ExistingSql') {
            let databasesResponse: ActionResponse = await this.getDatabases();
            if (databasesResponse.IsSuccess) {
                this.databases = databasesResponse.Body.value;
                this.database = this.databases.indexOf(oldDatabase) >= 0 ? oldDatabase : this.databases[0];
                this.showDatabases = this.setValidated();
            } else {
                this.onInvalidate();
                this.MS.ErrorService.set(databasesResponse.ExceptionDetail.FriendlyErrorMessage, databasesResponse.ExceptionDetail.AdditionalDetailsErrorMessage);
            }
        } else if (this.sqlInstance === 'NewSql') {
            let newSqlError: string = this.validateAzureSQLCreate(this.username, this.password, this.passwordConfirmation);
            if (newSqlError) {
                this.MS.ErrorService.set(newSqlError);
            } else {
                this.isValidated = await this.isAzureServerIsAvailable();
            }
        }

        this.isValidated = this.isValidated && await super.onValidate();

        return this.isValidated;
    }

    private async createDatabaseServer(): Promise<ActionResponse> {
        this.navigationMessage = this.MS.Translate.SQL_SERVER_CREATING_NEW;
        let body = this.getBody(true);
        body['SqlCredentials']['Database'] = this.newSqlDatabase;

        this.MS.DataStore.addToDataStore('SqlLocation', this.sqlLocation, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SqlSku', this.sqlSku, DataStoreType.Public);

        return await this.MS.HttpService.executeAsync('Microsoft-CreateAzureSql', body);
    }

    private getBody(withDatabase: boolean): any {
        let body: any = {};

        body.useImpersonation = this.useImpersonation;
        body['SqlCredentials'] = {};
        body['SqlCredentials']['Server'] = this.getSqlServer();
        body['SqlCredentials']['User'] = this.username;
        body['SqlCredentials']['Password'] = this.password;
        body['SqlCredentials']['AuthType'] = this.isWindowsAuth && !this.isAzureSql ? 'windows' : 'sql';

        if (this.isAzureSql) {
            body['SqlCredentials']['AuthType'] = 'sql';
        }

        if (withDatabase) {
            body['SqlCredentials']['Database'] = this.database;
        }

        return body;
    }

    private async getDatabases(): Promise<ActionResponse> {
        let body: any = this.getBody(true);
        return this.showAllWriteableDatabases
            ? await this.MS.HttpService.executeAsync('Microsoft-ValidateAndGetWritableDatabases', body)
            : await this.MS.HttpService.executeAsync('Microsoft-ValidateAndGetAllDatabases', body);
    }

    private getSqlServer(): string {
        let sqlServer: string = this.sqlServer;
        if (this.isAzureSql && this.isGovAzureSql && !sqlServer.includes(this.azureSqlSuffix)) {
            sqlServer += this.azureGovtSuffix;
        }
        if (this.isAzureSql && !this.isGovAzureSql && !sqlServer.includes(this.azureSqlSuffix)) {
            sqlServer += this.azureSqlSuffix;
        }
        return sqlServer;
    }

    private async isAzureServerIsAvailable(): Promise<boolean> {
        return await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-ValidateAzureSqlExists', this.getBody(false));
    }

    private validateAzureSQLCreate(username: string, password: string, password2: string): string {
        return this.validatePassword(password, password2, 8) || this.validateUsername(username, this.invalidUsernames, 'Username') || '';
    }

    private validatePassword(pwd: string, pwd2: string, length: number): string {
        let passwordError: string = '';
        let specialChar: RegExp = /[\!\@\#\$\%\^\&\*\(\)\_\\\-\+\=\`\~\{\}\|\\\:\;\"\'\<\,\>\.\?/]/;

        if (pwd !== pwd2) {
            passwordError = this.MS.Translate.SQL_ERROR_PASSWORD_MATCH;
        } else if (length && pwd.length < length) {
            passwordError = this.MS.Translate.SQL_ERROR_PASSWORD_LENGTH;
        } else if ((/\s/g).test(pwd)) {
            passwordError = this.MS.Translate.SQL_ERROR_PASSWORD_SPACES;
        } else if (!((/[A-Z]/).test(pwd) && (/[a-z]/).test(pwd) && (/[0-9]/).test(pwd) && specialChar.test(pwd))) {
            passwordError = this.MS.Translate.SQL_ERROR_PASSWORD_SPECIAL_CHARACTERS;
        }
        return passwordError;
    }

    private validateUsername(username: string, invalidUsernames: string[], usernameText: string): string {
        let usernameError: string = '';
        if ((/\s/g).test(username)) {
            usernameError = `${usernameText} ${this.MS.Translate.SQL_ERROR_USERNAME_SPACES}`;
        } else if (username.length > 63) {
            usernameError = `${usernameText} ${this.MS.Translate.SQL_ERROR_USERNAME_LENGTH}`;
        } else if (invalidUsernames.indexOf(username.toLowerCase()) > -1) {
            usernameError = `${usernameText} ${this.MS.Translate.SQL_ERROR_USERNAME_RESERVED}`;
        } else if (!(/^[a-zA-Z0-9\-]+$/).test(username)) {
            usernameError = `${usernameText} ${this.MS.Translate.SQL_ERROR_USERNAME_ALPHANUMERIC}`;
        } else if (username[0] === '-' || username[username.length - 1] === '-') {
            usernameError = `${usernameText} ${this.MS.Translate.SQL_ERROR_USERNAME_HYPHEN}`;
        }
        return usernameError;
    }
}