import { bindable } from 'aurelia-framework';
import { ViewModelBase } from '../services/view-model-base';

export class sqlservertemplate {
    @bindable viewmodel = null;
}

export class SqlServerViewModel extends ViewModelBase {
    auth: string = 'Windows';
    azureSqlSuffix: string = '.database.windows.net';
    checkSqlVersion: boolean = false;
    database: string = null;
    databases: string[] = [];
    hideSqlAuth: boolean = false;
    isAzureSql: boolean = false;
    isWindowsAuth: boolean = true;
    logInAsCurrentUser: boolean = false;
    newSqlDatabase: string = null;
    password: string = '';
    passwordConfirmation: string = '';
    showAllWriteableDatabases: boolean = true;
    showAzureSql: boolean = true;
    showLogInAsCurrentUser: boolean = false;
    showCredsWhenWindowsAuth: boolean = false;
    showDatabases: boolean = false;
    showNewSqlOption: boolean = false;
    sqlInstance: string = 'ExistingSql';
    sqlServer: string = '';
    subtitle: string = '';
    title: string = '';
    username: string = '';
    validateWindowsCredentials: boolean = false;
    validationTextBox: string = '';

    constructor() {
        super();
        this.isValidated = false;
    }

    Invalidate() {
        super.Invalidate();
        this.database = null;
        this.databases = [];
        this.onAuthChange();
        this.showDatabases = false;
    }

    onAuthChange() {
        this.isWindowsAuth = this.auth.toLowerCase() === 'windows';
    }

    onDatabaseChange() {
    }

    async OnValidate() {
        super.OnValidate();

        this.sqlServer = this.sqlServer.toLowerCase();

        if (this.sqlInstance === 'ExistingSql') {
            let isValid: boolean = true;

            if (this.validateWindowsCredentials) {
                isValid = await this.MS.UtilityService.ValidateImpersonation(this.username, this.password, this.logInAsCurrentUser);
            }

            if (isValid) {
                let databasesResponse = await this.GetDatabases();
                if (databasesResponse.isSuccess) {
                    this.databases = databasesResponse.response.value;
                    this.isValidated = true;
                    this.showDatabases = true;
                    this.showValidation = true;
                } else {
                    this.isValidated = false;
                    this.showDatabases = false;
                    this.showValidation = false;
                }
            }
        } else if (this.sqlInstance === 'NewSql') {
            let invalidServerNames: string[] = [];
            let invalidUsernames: string[] = [
                'admin',
                'administrator',
                'dbmanager',
                'dbo',
                'guest',
                'loginmanager',
                'public',
                'root',
                'sa'
            ];
            let passwordError: string = this.MS.UtilityService.ValidatePassword(this.password, this.passwordConfirmation, 8);
            let servernameError: string = this.MS.UtilityService.ValidateUsername(this.sqlServer, invalidServerNames, 'Server name');
            let usernameError: string = this.MS.UtilityService.ValidateUsername(this.username, invalidUsernames, 'Username');
            let newSqlError: string = passwordError || servernameError || usernameError;
            if (newSqlError) {
                this.MS.ErrorService.message = newSqlError;
            } else {
                let databasesResponse = await this.ValidateAzureServerIsAvailable();
                if ((databasesResponse.isSuccess)) {
                    this.isValidated = true;
                    this.showValidation = true;
                } else {
                    this.isValidated = false;
                    this.showValidation = false;
                }
            }
        }
    }

    async NavigatingNext(): Promise<boolean> {
        let body = this.GetBody(true);
        let response = null;
        if (this.sqlInstance === 'ExistingSql') {
            response = await this.MS.HttpService.Execute('Microsoft-GetSqlConnectionString', body);
        } else if (this.sqlInstance === 'NewSql') {
            response = await this.CreateDatabaseServer();
        }

        if (response.isSuccess) {
            this.MS.DataService.AddToDataStore(this.MS.NavigationService.GetCurrentSelectedPage().PageName, 'SqlConnectionString', response.response.value);
            this.MS.DataService.AddToDataStore(this.MS.NavigationService.GetCurrentSelectedPage().PageName, 'Server', this.getSqlServer());
            this.MS.DataService.AddToDataStore(this.MS.NavigationService.GetCurrentSelectedPage().PageName, 'Database', this.database);
            this.MS.DataService.AddToDataStore(this.MS.NavigationService.GetCurrentSelectedPage().PageName, 'Username', this.username);

            let isProperVersion: boolean = true;

            if (this.checkSqlVersion) {
                let responseVersion = (this.logInAsCurrentUser || !this.showLogInAsCurrentUser) && !this.MS.UtilityService.UseImpersonation()
                    ? await this.MS.HttpService.Execute('Microsoft-CheckSQLVersion', {})
                    : await this.MS.HttpService.ExecuteWithImpersonation('Microsoft-CheckSQLVersion', {});
                isProperVersion = responseVersion.isSuccess;
            }

            return isProperVersion;
        }

        return false;
    }

    private async CreateDatabaseServer() {
        this.navigationMessage = 'Creating a new SQL database, this may take 2-3 minutes';
        let body = this.GetBody(true)
        body['SqlCredentials']['Database'] = this.newSqlDatabase;
        return await this.MS.HttpService.Execute('Microsoft-CreateAzureSql', body)
    }

    private async ValidateAzureServerIsAvailable() {
        let body = this.GetBody(false)
        return await this.MS.HttpService.Execute('Microsoft-ValidateAzureSqlExists', body)
    }

    private async GetDatabases() {
        let body = this.GetBody(true);

        let databasesResponse = (this.logInAsCurrentUser || !this.showLogInAsCurrentUser) && !this.MS.UtilityService.UseImpersonation()
            ? this.showAllWriteableDatabases
                ? await this.MS.HttpService.Execute('Microsoft-ValidateAndGetWritableDatabases', body)
                : await this.MS.HttpService.Execute('Microsoft-ValidateAndGetAllDatabases', body)
            : this.showAllWriteableDatabases
                ? await this.MS.HttpService.ExecuteWithImpersonation('Microsoft-ValidateAndGetWritableDatabases', body)
                : await this.MS.HttpService.ExecuteWithImpersonation('Microsoft-ValidateAndGetAllDatabases', body);

        return databasesResponse;
    }

    private GetDatabase() {
    }

    private GetBody(withDatabase: boolean) {
        super.OnValidate();
        let body = {};
        body['SqlCredentials'] = {};
        body['SqlCredentials']['Server'] = this.getSqlServer();
        body['SqlCredentials']['User'] = this.username;
        body['SqlCredentials']['Password'] = this.password;
        body['SqlCredentials']['AuthType'] = this.isWindowsAuth ? 'windows' : 'sql';

        if (this.isAzureSql) {
            body['SqlCredentials']['AuthType'] = 'sql';
        }

        if (withDatabase) {
            body['SqlCredentials']['Database'] = this.database
        }

        return body;
    }

    private getSqlServer(): string {
        let sqlServer: string = this.sqlServer;
        if (this.isAzureSql && !sqlServer.includes(this.azureSqlSuffix)) {
            sqlServer += this.azureSqlSuffix;
        }
        return sqlServer;
    }
}