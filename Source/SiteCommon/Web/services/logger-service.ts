/// <reference path="../scripts/ai.1.0.0-build00159.d.ts" />

import { MainService } from './main-service';

export class LoggerService {
    MS: MainService;
    appInsights: Microsoft.ApplicationInsights.AppInsights;
    UserGenId: string;
    OperationId: string;
    UserId: string;
    SessionId: string;

    constructor(MainService: MainService) {
        this.MS = MainService;
        var snippet: any = {
            config: {
                instrumentationKey: 'app_insights_key'
            }
        };

        var init = new Microsoft.ApplicationInsights.Initialization(snippet);
        var applicationInsights = init.loadAppInsights();
        this.appInsights = applicationInsights;

        this.UserGenId = this.MS.UtilityService.getItem('UserGeneratedId');
        this.SessionId = applicationInsights.context.session.id;
        this.UserId = applicationInsights.context.user.id;
        this.OperationId = applicationInsights.context.operation.id;
    }

    getSessionId(): string {
        let sessionId: string = '';

        if (this.appInsights != null &&
            this.appInsights.context != null &&
            this.appInsights.context.session != null) {
            sessionId = this.appInsights.context.session.id;
        }

        return sessionId;
    }

    trackDeploymentEnd(isSuccess: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;
        properties.TemplateName = this.MS.NavigationService.appName;
        properties.Sucess = isSuccess;
        this.appInsights.trackEvent('UI-DeploymentEnd', properties);
    }

    trackDeploymentStart(): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;

        properties.TemplateName = this.MS.NavigationService.appName;
        this.appInsights.trackEvent('UI-DeploymentStart', properties);
    }

    trackDeploymentStepStartEvent(deploymentIndex: any, deploymentName: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;
        properties.DeploymentIndex = deploymentIndex;
        properties.DeploymentName = deploymentName;
        properties.TemplateName = this.MS.NavigationService.appName;
        this.appInsights.trackEvent('UI-' + deploymentName + '-Start-' + deploymentIndex, properties);
    }

    trackDeploymentStepStopEvent(deploymentIndex: any, deploymentName: any, isSuccess: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;
        properties.DeploymentIndex = deploymentIndex;
        properties.DeploymentName = deploymentName;
        properties.TemplateName = this.MS.NavigationService.appName;
        properties.Sucess = isSuccess;
        this.appInsights.trackEvent('UI-' + deploymentName + '-End-' + deploymentIndex, properties);
    }

    trackEndRequest(request: any, uniqueId: any, isSucess: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;
        properties.Request = request;
        properties.UniqueId = uniqueId;
        properties.Sucess = isSucess;
        properties.TemplateName = this.MS.NavigationService.appName;
        this.appInsights.trackEvent('UI-EndRequest-' + request, properties);
    }

    trackEvent(requestName: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;
        properties.TemplateName = this.MS.NavigationService.appName;
        this.appInsights.trackEvent('UI-' + requestName, properties);
    }

    trackPageView(page: any, url: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        this.appInsights.trackPageView(page, url, properties);
    }

    trackStartRequest(request: any, uniqueId: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;
        properties.Request = request;
        properties.UniqueId = uniqueId;
        properties.TemplateName = this.MS.NavigationService.appName;
        this.appInsights.trackEvent('UI-StartRequest-' + request, properties);
    }

    trackTrace(trace: string): void {
        this.appInsights.trackTrace('UI-' + trace);
    }

    trackUninstallEnd(isSuccess: any): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;
        properties.TemplateName = this.MS.NavigationService.appName;
        properties.Sucess = isSuccess;
        this.appInsights.trackEvent('UI-UninstallEnd', properties);
    }

    trackUninstallStart(): void {
        let properties: any = this.getPropertiesForTelemetry();
        properties.UserGenId = this.UserGenId;
        properties.SessionId = this.SessionId;
        properties.UserId = this.UserId;
        properties.OperationId = this.OperationId;

        properties.TemplateName = this.MS.NavigationService.appName;
        this.appInsights.trackEvent('UI-UninstallStart', properties);
    }

    private getPropertiesForTelemetry(): any {
        this.appInsights.config.disableTelemetry = !!this.MS.UtilityService.getItem('AITR_NONUSRACT_OnError');

        let obj: any = {};
        obj.AppName = this.MS.NavigationService.appName;
        obj.FullUrl = window.location.href;
        obj.Origin = window.location.origin;
        obj.Host = window.location.host;
        obj.HostName = window.location.hostname;
        obj.PageNumber = this.MS.NavigationService.index;

        if (this.MS.NavigationService.getCurrentSelectedPage()) {
            obj.Route = this.MS.NavigationService.getCurrentSelectedPage().RoutePageName;
            obj.PageName = this.MS.NavigationService.getCurrentSelectedPage().PageName;
            obj.PageModuleId = this.MS.NavigationService.getCurrentSelectedPage().Path.replace(/\\/g, "/");
            obj.PageDisplayName = this.MS.NavigationService.getCurrentSelectedPage().DisplayName;
        }

        obj.RootSource = this.MS.HttpService.isOnPremise ? 'MSI' : 'WEB';

        return obj;
    }
}