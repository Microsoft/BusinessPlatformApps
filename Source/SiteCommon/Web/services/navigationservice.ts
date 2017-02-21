import { Aurelia } from 'aurelia-framework';
import { inject } from 'aurelia-framework';
import MainService from './mainservice';
import {JsonCustomParser} from "../base/JsonCustomParser";

export class NavigationService {
    currentViewModel: any = null;
    index: number = -1;
    isOnline: boolean = true;
    MS: MainService;
    pages: any[] = [];
    appName: string = '';
    isCurrentlyNavigating: boolean = false;

    constructor(MainService) {
        this.MS = MainService;
    }

    init(pagesJson) {
        this.pages = pagesJson;

        if (this.pages && this.pages.length && this.pages.length > 0) {
            this.index = 0;

            for (let i = 1; i < this.pages.length; i++) {
                this.MS.Router.addRoute({
                    route: this.pages[i].RoutePageName.toLowerCase(),
                    name: this.pages[i].PageName,
                    moduleId: '.' + this.pages[i].Path.replace(/\\/g,"/"),
                    title: this.pages[i].DisplayName,
                    nav: true
                });
            }

            this.MS.Router.addRoute({
                route: '',
                name: this.pages[0].PageName,
                moduleId: '.' + this.pages[0].Path.replace(/\\/g, "/"),
                title: this.pages[0].DisplayName,
                nav: true
            });

            this.pages[0].isActive = true;
            this.pages[0].RoutePageName = '';
            this.MS.Router.refreshNavigation();
        }

        this.UpdateIndex();
        this.MS.DataStore.CurrentRoutePage = this.pages[this.index].RoutePageName.toLowerCase();
        this.MS.LoggerService.TrackPageView(this.GetCurrentRoutePath(), window.location.href);
    }

    GetCurrentRoutePath(): string {
        let history: any = this.MS.Router.history;
        let route: string = history.location.hash;
        let routePage = this.MS.NavigationService.appName + route.replace('#', '');
        if (routePage.endsWith('/')) {
            routePage += '//';
            routePage.replace('///', '');
        }

        return routePage;
    }

    GetRoute(): string {
        let history: any = this.MS.Router.history;
        let route: string = history.location.hash;
        return route.replace('#', '').replace('/','');
    }

    UpdateIndex() {
        let routePageName = this.GetRoute();
        for (let i = 0; i < this.pages.length; i++) {
            if (this.pages[i].RoutePageName.toLowerCase() === routePageName.toLowerCase()) {
                this.index = i;
            }
        }

        for (let i = 0; i < this.pages.length; i++) {
            this.pages[i].isActive = i === this.index;
            this.pages[i].isComplete = i < this.index;
        }
        return this.index;
    }

    NavigateNext() {
        this.UpdateIndex();
        if (this.index >= this.pages.length - 1 && this.index < this.pages.length-1) {
            return;
        }
        this.index = this.index + 1;

        // If you skip the last page then we should throw an error - no check in place - to be added
        while (this.pages[this.index].Parameters.skip) {
            let body: any = {};
            JsonCustomParser.loadVariables(body, this.pages[this.index].Parameters, this.MS, this);
            if (body.skip && body.skip.toLowerCase() === "true") {
                this.index = this.index + 1;
            }
        }

        this.NavigateToIndex();
    }

    NavigateBack() {
        this.UpdateIndex();
        if (this.index == 0) {
            return;
        }
        this.index = this.index - 1;


        // If you skip the last page then we should throw an error - no check in place - to be added
        while (this.pages[this.index].Parameters.skip && this.index > 0 ) {
            let body: any = {};
            JsonCustomParser.loadVariables(body, this.pages[this.index].Parameters, this.MS, this);
            if (body.skip && body.skip.toLowerCase() === "true") {
                this.index = this.index - 1;
            }
        }
        this.NavigateToIndex();
    }

    JumpTo(index) {
        this.index = index;
        this.NavigateToIndex();
    }

    NavigateToIndex() {
         // do not update index here

        // Initialise the page
        this.MS.DataStore.CurrentRoutePage = this.pages[this.index].RoutePageName.toLowerCase();


        // The index is set to the next step
        this.MS.Router.navigate('#/' + this.pages[this.index].RoutePageName.toLowerCase());

        
        this.MS.Router.refreshNavigation();
        this.UpdateIndex();
        this.MS.LoggerService.TrackPageView(this.appName + '/' + this.pages[this.index].RoutePageName.toLowerCase(),
            window.location.href);
    }

    getCurrentSelectedPage() {
        return this.pages[this.index];
    }

    getIndex(): number {
        return this.index;
    }

    isLastPage(): boolean {
        if (this.pages.length - 1 === this.getIndex()) {
            return true;
        }

        return false;
    }

    isFirstPage(): boolean {
        return this.getIndex() === 0;
    }
}