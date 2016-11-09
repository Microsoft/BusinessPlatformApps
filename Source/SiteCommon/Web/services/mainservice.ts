import { Aurelia } from 'aurelia-framework';
import { inject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { HttpClient } from 'aurelia-http-client';

import { DeploymentService } from './deploymentservice';
import { ErrorService } from './errorservice';
import { LoggerService } from './loggerservice';
import { NavigationService } from './navigationservice';
import { HttpService } from './httpservice';
import { DataStore } from './DataStore';
import { UtilityService } from './utilityservice';
import { ViewModelBase } from './viewmodelbase';

import {ExperienceType} from '../base/ExperienceType';

@inject(Router, HttpClient)
export default class MainService {
    Router: Router;
    MS: MainService;
    ErrorService: ErrorService;
    LoggerService: LoggerService;
    HttpService: HttpService;
    NavigationService: NavigationService;
    DataStore: DataStore;
    DeploymentService: DeploymentService;
    UtilityService: UtilityService;
    appName: string;
    templateData: any;
    experienceType: string;

    constructor(router, httpClient) {
        this.Router = router;
        (<any>window).MainService = this;


        this.UtilityService = new UtilityService(this);
        this.appName = this.UtilityService.GetQueryParameter('name');
        this.experienceType = this.UtilityService.GetQueryParameter('type');

        this.ErrorService = new ErrorService(this);
        this.HttpService = new HttpService(this, httpClient);
        this.NavigationService = new NavigationService(this);
        this.NavigationService.appName = this.appName;
        this.DataStore = new DataStore(this);

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
    async init() {
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
                    this.DeploymentService.isUninstall = true;
                    break;
                }
                default: {
                    pages = 'Pages';
                    actions = 'Actions';
                    break;
                }
            }
            this.templateData = await this.HttpService.getApp(this.appName);
            if (this.templateData && this.templateData[pages]) {
                this.NavigationService.init(this.templateData[pages]);
            }
            if (this.templateData && this.templateData[actions]) {
                this.DeploymentService.actions = this.templateData[actions];
            }
        }
    }
}