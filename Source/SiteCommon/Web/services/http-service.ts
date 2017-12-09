/// <reference path="../../../Site/Microsoft.Deployment.Site.Web/typings/index.d.ts" />

import { ActionStatus } from '../enums/action-status';

import { ActionRequest } from '../models/action-request';
import { ActionResponse } from '../models/action-response';

import { HttpClient } from 'aurelia-http-client';

import { MainService } from './main-service';

export class HttpService {
    baseUrl: string = 'http://localhost:42387/api/';
    command: any;
    HttpClient: HttpClient;
    isOnPremise: boolean = false;
    isServiceBusy: boolean = false;
    MS: MainService;

    constructor(MainService: MainService, HttpClient: HttpClient) {
        if (window.location.href.startsWith('http://localhost') || window.location.href.startsWith('https://localhost')) {
            this.baseUrl = 'http://localhost:2305/api/';
        } else {
            let url = window.location.href;
            if (url.includes('bpsolutiontemplates')) {
                this.baseUrl = 'https://bpstservice.azurewebsites.net/api/';
            } else {
                url = url.replace('bpst', 'bpstservice');
                let splitUrls = url.split('/');
                this.baseUrl = splitUrls[0] + '//' + splitUrls[2] + '/api/';
            }
        }

        this.MS = MainService;
        this.HttpClient = HttpClient;

        let $window: any = window;
        if ($window && $window.command) {
            this.command = $window.command;
            this.isOnPremise = true;
        }
    }

    close(): void {
        this.command.close(!this.MS.DeploymentService.hasError && this.MS.DeploymentService.isFinished);
    }

    async executeAsync(method: string, content: any = {}): Promise<ActionResponse> {
        var actionResponse: ActionResponse = null;

        if (!content.isInvisible) {
            this.isServiceBusy = true;
            this.MS.ErrorService.clear();
        }

        let uniqueId = this.MS.UtilityService.getUniqueId(20);

        try {
            var actionRequest: ActionRequest = new ActionRequest(content, this.MS.DataStore);
            this.MS.LoggerService.trackStartRequest(method, uniqueId);
            var response = null;

            if (this.isOnPremise) {
                response = await this.command.executeaction(this.MS.LoggerService.UserId, this.MS.LoggerService.UserGenId, '', this.MS.LoggerService.OperationId, uniqueId, this.MS.NavigationService.appName,
                    method,
                    JSON.stringify(actionRequest));
            } else {
                response = await this.getRequestObject('post', `/action/${method}`, actionRequest).send();
                response = response.response;
            }

            var responseParsed: any = JSON.parse(response);
            actionResponse = responseParsed;
            actionResponse.Status = (<any>ActionStatus)[responseParsed.Status];

            this.MS.LoggerService.trackEndRequest(method, uniqueId, !actionResponse.IsSuccess);

            if (actionResponse.Status !== ActionStatus.Invisible) {
                this.MS.DataStore.loadDataStoreFromJson(actionResponse.DataStore);
            }

            if (!content.isInvisible) {
                if (actionResponse.Status === ActionStatus.Failure || actionResponse.Status === ActionStatus.FailureExpected) {
                    let additionalDetails: string = actionResponse.ExceptionDetail.AdditionalDetailsErrorMessage
                        ? `${actionResponse.ExceptionDetail.AdditionalDetailsErrorMessage} --- ${this.MS.Translate.COMMON_ACTION_FAILED}`
                        : this.MS.Translate.COMMON_ACTION_FAILED;
                    this.MS.ErrorService.set(actionResponse.ExceptionDetail.FriendlyErrorMessage,
                        `${additionalDetails} ${method} --- ${this.MS.Translate.COMMON_ERROR_ID}:(${this.MS.LoggerService.UserGenId})`,
                        actionResponse.Status === ActionStatus.Failure,
                        actionResponse.ExceptionDetail.LogLocation);
                } else if (actionResponse.Status !== ActionStatus.Invisible) {
                    this.MS.ErrorService.clear();
                }
            }
        } catch (e) {
            if (this.MS.UtilityService.isOnline() || this.isOnPremise) {
                this.MS.ErrorService.set(this.MS.Translate.COMMON_UNKNOWN_ERROR);
            } else {
                this.MS.ErrorService.set(this.MS.Translate.COMMON_OFFLINE);
            }
            throw e;
        } finally {
            if (!content.isInvisible) {
                this.isServiceBusy = false;
            }
        }

        return actionResponse;
    }

    async getApp(name: string): Promise<any> {
        var response = null;
        let uniqueId = this.MS.UtilityService.getUniqueId(20);
        this.MS.LoggerService.trackStartRequest('GetApp-name', uniqueId);
        if (this.isOnPremise) {
            response = await this.command.gettemplate(this.MS.LoggerService.UserId, this.MS.LoggerService.UserGenId, '', this.MS.LoggerService.OperationId, uniqueId, name);
        } else {
            response = await this.getRequestObject('get', `/App/${name}`).send();
            response = response.response;
        }
        if (!response) {
            response = '{}';
        }

        this.MS.LoggerService.trackEndRequest('GetTemplate-name', uniqueId, true);
        let responseParsed = JSON.parse(response);
        return responseParsed;
    }

    async getExecuteResponseAsync(method: string, property: string = 'value', content: any = {}): Promise<any> {
        return (await this.executeAsync(method, content)).Body[property];
    }

    async getResponseAsync(method: string, content: any = {}): Promise<any> {
        let response: any = null;

        let body: any = (await this.executeAsync(method, content)).Body.Value;

        try {
            response = JSON.parse(body);
        } catch (e) {
            response = body;
        }

        return response;
    }

    async isExecuteSuccessAsync(method: string, content: any = {}): Promise<boolean> {
        return (await this.executeAsync(method, content)).IsSuccess;
    }

    private getRequestObject(method: string, relativeUrl: string, body: any = {}): any {
        let uniqueId = this.MS.UtilityService.getUniqueId(20);
        var request = this.HttpClient.createRequest(relativeUrl);
        request = request
            .withBaseUrl(this.baseUrl)
            .withHeader('Content-Type', 'application/json; charset=utf-8')
            .withHeader('UserGeneratedId', this.MS.LoggerService.UserGenId)
            .withHeader('OperationId', this.MS.LoggerService.OperationId)
            .withHeader('SessionId', this.MS.LoggerService.getSessionId())
            .withHeader('UserId', this.MS.LoggerService.UserId)
            .withHeader('TemplateName', this.MS.NavigationService.appName)
            .withHeader('UniqueId', uniqueId);

        if (method === 'get') {
            request = request.asGet();
        } else {
            request = request
                .asPost()
                .withContent(JSON.stringify(body));
        }

        return request;
    }
}