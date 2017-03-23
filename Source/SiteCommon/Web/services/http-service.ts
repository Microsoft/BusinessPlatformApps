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

    constructor(MainService, HttpClient) {
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

    Close() {
        this.command.close(!this.MS.DeploymentService.hasError && this.MS.DeploymentService.isFinished);
    }

    async getApp(name) {
        var response = null;
        let uniqueId = this.MS.UtilityService.GetUniqueId(20);
        this.MS.LoggerService.TrackStartRequest('GetApp-name', uniqueId);
        if (this.isOnPremise) {
            response = await this.command.gettemplate(this.MS.LoggerService.UserId, this.MS.LoggerService.UserGenId, '', this.MS.LoggerService.OperationId, uniqueId, name);
        } else {
            response = await this.getRequestObject('get', `/App/${name}`).send();
            response = response.response;
        }
        if (!response) {
            response = '{}';
        }

        this.MS.LoggerService.TrackEndRequest('GetTemplate-name', uniqueId, true);
        let responseParsed = JSON.parse(response);
        return responseParsed;
    }

    async executeAsync(method, content: any = {}): Promise<ActionResponse> {
        this.isServiceBusy = true;
        var actionResponse: ActionResponse = null;

        if (!content.isInvisible) {
            this.MS.ErrorService.Clear();
        }

        let uniqueId = this.MS.UtilityService.GetUniqueId(20);

        try {
            var actionRequest: ActionRequest = new ActionRequest(content, this.MS.DataStore);
            this.MS.LoggerService.TrackStartRequest(method, uniqueId);
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
            actionResponse.Status = ActionStatus[<string>responseParsed.Status];

            this.MS.LoggerService.TrackEndRequest(method, uniqueId, !actionResponse.IsSuccess);
            this.MS.DataStore.loadDataStoreFromJson(actionResponse.DataStore);

            // Handle any errors here
            if (actionResponse.Status === ActionStatus.Failure || actionResponse.Status === ActionStatus.FailureExpected) {
                this.MS.ErrorService.details = `${actionResponse.ExceptionDetail.AdditionalDetailsErrorMessage} --- Action Failed ${method} --- Error ID:(${this.MS.LoggerService.UserGenId})`;
                this.MS.ErrorService.logLocation = actionResponse.ExceptionDetail.LogLocation;
                this.MS.ErrorService.message = actionResponse.ExceptionDetail.FriendlyErrorMessage;
                this.MS.ErrorService.showContactUs = actionResponse.Status === ActionStatus.Failure;
            } else if (actionResponse.Status !== ActionStatus.Invisible) {
                this.MS.ErrorService.Clear();
            }
        } catch (e) {
            this.MS.ErrorService.message = this.MS.Translate.COMMON_UNKNOWN_ERROR;
            this.MS.ErrorService.showContactUs = true;
            throw e;
        } finally {
            this.isServiceBusy = false;
        }

        return actionResponse;
    }

    async executeAsyncWithImpersonation(method, content): Promise<ActionResponse> {
        let body: any = {};

        if (content) {
            body = content;
        }

        body.useImpersonation = true;
        return this.executeAsync(method, content);
    }

    private getRequestObject(method: string, relativeUrl: string, body: any = {}) {
        let uniqueId = this.MS.UtilityService.GetUniqueId(20);
        var request = this.HttpClient.createRequest(relativeUrl);
        request = request
            .withBaseUrl(this.baseUrl)
            .withHeader('Content-Type', 'application/json; charset=utf-8')
            .withHeader('UserGeneratedId', this.MS.LoggerService.UserGenId)
            .withHeader('OperationId', this.MS.LoggerService.OperationId)
            .withHeader('SessionId', this.MS.LoggerService.appInsights.context.session.id)
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