import { QueryParameter } from '../constants/query-parameter';

import { DataStoreType } from '../enums/data-store-type';

import { MainService } from './main-service';

export class UtilityService {
    MS: MainService;

    constructor(mainservice: MainService) {
        this.MS = mainservice;
    }

    clearSessionStorage(): void {
        window.sessionStorage.clear();
    }

    clone(obj: any): any {
        return JSON.parse(JSON.stringify(obj));
    }

    async connectToAzure(openAuthorizationType: string, azureActiveDirectoryTenant: string = this.MS.Translate.DEFAULT_TENANT): Promise<void> {
        this.MS.DataStore.addToDataStore('AADTenant', azureActiveDirectoryTenant, DataStoreType.Public);
        window.location.href = await this.MS.HttpService.getExecuteResponseAsync('Microsoft-GetAzureAuthUri', 'value', {
            oauthType: openAuthorizationType,
            currentPage: this.MS.UtilityService.getItem('Current Page')
        });
    }

    async connectToAzureSPN(): Promise<void> {
        window.location.href = await this.MS.HttpService.getExecuteResponseAsync('Microsoft-GetAzureAuthUriForSPN');
    }

    async connectToFacebook(): Promise<void> {
        window.location.href = await this.MS.HttpService.getExecuteResponseAsync('Microsoft-GetFacebookAuthUri');
    }

    async connectToCuna(): Promise<void> {
        let returnValue = await this.MS.HttpService.getExecuteResponseAsync('Microsoft-GetCunaAuthUri');
        window.location.href = returnValue;
    }

    extractDomain(username: string): string {
        let usernameSplit: string[] = username.split('\\');
        return usernameSplit[0];
    }

    extractUsername(username: string): string {
        let usernameSplit: string[] = username.split('\\');
        return usernameSplit[1];
    }

    generateDailyTriggers(): string[] {
        let dailyTriggers: string[] = [];
        for (let i = 0; i < 24; i++) {
            dailyTriggers.push(`${i}:00`);
            dailyTriggers.push(`${i}:30`);
        }
        return dailyTriggers;
    }

    getItem(key: any): any {
        let item: any = JSON.parse(window.sessionStorage.getItem(key));
        return item;
    }

    getQueryParameter(id: any): string {
        var regex = new RegExp('[?&]' + id.replace(/[\[\]]/g, '\\$&') + '(=([^&#]*)|&|#|$)');
        var results = regex.exec(window.location.href);
        return (!results || !results[2])
            ? ''
            : decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    getQueryParameterFromUrl(name: any, url: any): string {
        var regex = new RegExp('[?&]' + name.replace(/[\[\]]/g, '\\$&') + '(=([^&#]*)|&|#|$)');
        var results = regex.exec(url);
        return (!results || !results[2])
            ? ''
            : decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    async getToken(openAuthorizationType: string, callback: () => Promise<void>): Promise<void> {
        let queryParam: any = this.getItem('queryUrl');
        if (queryParam) {
            let token = this.getQueryParameterFromUrl(QueryParameter.CODE, queryParam);
            if (token === '') {
                this.MS.ErrorService.set(this.MS.Translate.AZURE_LOGIN_UNKNOWN_ERROR, this.MS.UtilityService.getQueryParameterFromUrl(QueryParameter.ERROR_DESCRIPTION, queryParam));
            } else {
                if (openAuthorizationType === 'Facebook') {
                    await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-GetPermanentPageToken', { code: token, oauthType: openAuthorizationType })
                    await callback();
                }
                else if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-GetAzureToken', { code: token, oauthType: openAuthorizationType })) {
                    await callback();
                }
            }
            this.MS.UtilityService.removeItem('queryUrl');
        }
    }

    getUniqueId(characters: number): string {
        return Math.random().toString(36).substr(2, characters + 2);
    }

    isOnline(): boolean {
        return window && window.navigator && window.navigator.onLine;
    }

    parseCsv(content: string): string[][] {
        let data: string[][] = [];
        let rows: string[] = content.split('\r\n');
        for (let i = 0; i < rows.length; i++) {
            data.push(rows[i].split(','));
        }
        return data;
    }

    readFile(file: File, callback: (result: any) => void): void {
        if (file) {
            let fileReader: FileReader = new FileReader();
            fileReader.onload = (fileContent: any) => {
                callback(fileContent.target.result);
            };
            fileReader.readAsText(file);
        }
    }

    removeItem(key: any): void {
        window.sessionStorage.removeItem(key);
    }

    saveItem(key: any, value: any): void {
        let val = JSON.stringify(value);
        if (window.sessionStorage.getItem(key)) {
            window.sessionStorage.removeItem(key);
        }
        window.sessionStorage.setItem(key, val);
    }
}