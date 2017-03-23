import { inject } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router'

import { MainService } from "./SiteCommon/Web/services/main-service";

@inject(MainService)
export class App
{
    MS: MainService;

    constructor(MainService) {
        this.MS = MainService;
    }

    async configureRouter(config: RouterConfiguration, router: Router) {
        await this.MS.init();
    }
}