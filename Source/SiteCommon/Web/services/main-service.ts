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
import { Mscc } from './mscc';
import { NavigationService } from './navigation-service';
import { TranslateService } from './translate-service';
import { UtilityService } from './utility-service';

@inject(Router, HttpClient)
export class MainService {
    appName: string;
    etl: string;
    experienceType: ExperienceType;
    DataStore: DataStore;
    DeploymentService: DeploymentService;
    ErrorService: ErrorService;
    HttpService: HttpService;
    LoggerService: LoggerService;
    MS: MainService;
    mscc: Mscc;
    msccBanner: any;
    msccLearnMore: any;
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
        this.appName = this.UtilityService.getQueryParameter(QueryParameter.NAME);

        let experienceTypeString: string = this.UtilityService.getQueryParameter(QueryParameter.TYPE);
        this.experienceType = (<any>ExperienceType)[experienceTypeString];

        this.ErrorService = new ErrorService(this);
        this.HttpService = new HttpService(this, httpClient);
        this.NavigationService = new NavigationService(this);
        this.NavigationService.appName = this.appName;
        this.DataStore = new DataStore(this);

        let translate: TranslateService = new TranslateService(this, this.UtilityService.getQueryParameter(QueryParameter.LANG));
        this.Translate = translate.language;

        if (this.UtilityService.getItem('App Name') !== this.appName) {
            this.UtilityService.clearSessionStorage();
        }

        this.UtilityService.saveItem('App Name', this.appName);

        if (!this.UtilityService.getItem('UserGeneratedId')) {
            this.UtilityService.saveItem('UserGeneratedId', this.UtilityService.getUniqueId(15));
        }

        this.LoggerService = new LoggerService(this);
        this.DeploymentService = new DeploymentService(this);

        this.etl = this.UtilityService.getQueryParameter(QueryParameter.ETL);
    }

    // Uninstall or any other types go here
    async init(): Promise<void> {
        let pages: string = '';
        let actions: string = '';

        if (this.appName && this.appName !== '') {
            switch (this.experienceType) {
                case ExperienceType.Install: {
                    pages = 'Pages';
                    actions = 'Actions';
                    break;
                }
                case ExperienceType.Uninstall: {
                    pages = 'UninstallPages';
                    actions = 'UninstallActions';
                    this.DeploymentService.experienceType = this.experienceType;
                    break;
                }
                default: {
                    pages = 'Pages';
                    actions = 'Actions';
                    this.DeploymentService.experienceType = ExperienceType.Install;
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

            if (!this.HttpService.isOnPremise) {
                this.mscc = new Mscc(this.msccBanner, this.msccLearnMore);
            }
        }
    }
}