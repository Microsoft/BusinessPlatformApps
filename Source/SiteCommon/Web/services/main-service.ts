/// <reference path="../../../Site/Microsoft.Deployment.Site.Web/typings/index.d.ts" />

import { inject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { HttpClient } from 'aurelia-http-client';

import { QueryParameter } from '../constants/query-parameter';

import { ExperienceType } from '../enums/experience-type';

import { Option } from '../models/option';

import { DataStore } from './data-store';
import { DeploymentService } from './deployment-service';
import { ErrorService } from './error-service';
import { HttpService } from './http-service';
import { LoggerService } from './logger-service';
import { NavigationService } from './navigation-service';
import { TranslateService } from './translate-service';
import { UtilityService } from './utility-service';

@inject(Router, HttpClient)
export class MainService {
    appName: string;
    experienceType: ExperienceType;
    DataStore: DataStore;
    DeploymentService: DeploymentService;
    ErrorService: ErrorService;
    HttpService: HttpService;
    LoggerService: LoggerService;
    MS: MainService;
    NavigationService: NavigationService;
    Option: Option = new Option();
    Router: Router;
    Translate: any;
    UtilityService: UtilityService;
    templateData: any;

    constructor(router: any, httpClient: HttpClient) {
        this.Router = router;
        (<any>window).MainService = this;

        this.UtilityService = new UtilityService(this);
        this.appName = this.UtilityService.GetQueryParameter(QueryParameter.NAME); 

        let experienceTypeString: string = this.UtilityService.GetQueryParameter(QueryParameter.TYPE);
        this.experienceType = (<any>ExperienceType)[experienceTypeString];

        this.ErrorService = new ErrorService(this);
        this.HttpService = new HttpService(this, httpClient);
        this.NavigationService = new NavigationService(this);
        this.NavigationService.appName = this.appName;
        this.DataStore = new DataStore(this);

        let translate: TranslateService = new TranslateService(this, this.UtilityService.GetQueryParameter(QueryParameter.LANG));
        this.Translate = translate.language;

        if (this.UtilityService.GetItem('App Name') !== this.appName) {
            this.UtilityService.ClearSessionStorage();
        }

        this.UtilityService.SaveItem('App Name', this.appName);

        if (!this.UtilityService.GetItem('UserGeneratedId')) {
            this.UtilityService.SaveItem('UserGeneratedId', this.UtilityService.GetUniqueId(15));
        }

        this.LoggerService = new LoggerService(this);
        this.DeploymentService = new DeploymentService(this);
    }

    // Uninstall or any other types go here
    async init(): Promise<void> {
        let pages: string = '';
        let actions: string = '';

        if (this.appName && this.appName !== '') {
            switch (this.experienceType) {
                case ExperienceType.install: {
                    pages = 'Pages';
                    actions = 'Actions';
                    break;
                }
                case ExperienceType.uninstall: {
                    pages = 'UninstallPages';
                    actions = 'UninstallActions';
                    this.DeploymentService.experienceType = this.experienceType;
                    break;
                }
                default: {
                    pages = 'Pages';
                    actions = 'Actions';
                    this.DeploymentService.experienceType = ExperienceType.install;
                    break;
                }
            }
            this.templateData = await this.HttpService.getApp(this.appName);
            if (this.templateData && this.templateData[pages]) {
                this.NavigationService.init(this.templateData[pages]);
            }
            if (this.templateData && this.templateData[actions]) {
                this.DeploymentService.init(this.templateData[actions]);
            }
        }
    }
}