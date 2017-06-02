/// <reference path="../../../Site/Microsoft.Deployment.Site.Web/typings/index.d.ts" />

import { activationStrategy } from 'aurelia-router';

import { InitParser } from '../classes/init-parser';

import { DataStoreType } from '../enums/data-store-type';

import { MainService } from './main-service';

export class ViewModelBase {
    isActivated: boolean = false;
    isAuthenticated: boolean = true;
    isValidated: boolean = false;
    MS: MainService;
    navigationMessage: string = '';
    onNext: any[] = [];
    onValidate: any[] = [];
    showValidation: boolean = false;
    showValidationDetails: boolean = false;
    textNext: string = 'Next';
    useDefaultValidateButton: boolean = false;
    validationText: string;
    viewmodel: ViewModelBase;

    constructor() {
        this.MS = (<any>window).MainService;
        this.viewmodel = this;
    }

    loadParameters(): void {
        var parameters = this.MS.NavigationService.getCurrentSelectedPage().Parameters;
        InitParser.loadVariables(this, this.MS.UtilityService.Clone(parameters), this.MS, this);
    }

    async NavigateNext(): Promise<void> {
        if (this.MS.NavigationService.isCurrentlyNavigating) {
            return;
        }

        try {
            this.isValidated = false;
            this.MS.NavigationService.isCurrentlyNavigating = true;
            let isNavigationSuccessful: boolean = await this.NavigatingNext();
            let isExtendedNavigationSuccessful: boolean = false;
            if (isNavigationSuccessful) {
                isExtendedNavigationSuccessful = await InitParser.executeActions(this.onNext, this);
            }
            this.navigationMessage = '';
            if (isNavigationSuccessful && isExtendedNavigationSuccessful) {
                let currentRoute = this.MS.NavigationService.getCurrentSelectedPage().RoutePageName.toLowerCase();
                let viewmodelPreviousSave = window.sessionStorage.getItem(currentRoute);

                // Save view model state
                if (viewmodelPreviousSave) {
                    window.sessionStorage.removeItem(currentRoute);
                }

                window.sessionStorage.setItem(currentRoute, JSON.stringify(this.saveViewModel()));
                this.MS.NavigationService.NavigateNext();
                this.NavigatedNext();
                this.isValidated = true;
            }
        } catch (e) {
        } finally {
            this.MS.NavigationService.isCurrentlyNavigating = false;
            this.MS.DataStore.addToDataStore('HasNavigated', true, DataStoreType.Public);
            if (this.isValidated) {
                this.VerifyNavigation();
            }
        }
    }

    NavigateBack(): void {
        if (this.MS.NavigationService.isCurrentlyNavigating) {
            return;
        }

        this.MS.NavigationService.isCurrentlyNavigating = true;
        let currentRoute = this.MS.NavigationService.getCurrentSelectedPage().RoutePageName.toLowerCase();

        let viewmodelPreviousSave = window.sessionStorage.getItem(currentRoute);
        if (viewmodelPreviousSave) {
            window.sessionStorage.removeItem(currentRoute);
        }

        window.sessionStorage.setItem(currentRoute, JSON.stringify(this.saveViewModel()));

        this.MS.NavigationService.NavigateBack();
        this.MS.ErrorService.Clear();
        this.MS.NavigationService.isCurrentlyNavigating = false;
        this.VerifyNavigation();
    }

    async activate(): Promise<void> {
        this.MS.NavigationService.Activate();

        this.isActivated = false;
        this.MS.UtilityService.SaveItem('Current Page', window.location.href);
        let currentRoute = this.MS.NavigationService.getCurrentSelectedPage().RoutePageName.toLowerCase();
        this.MS.UtilityService.SaveItem('Current Route', currentRoute);
        let viewmodelPreviousSave = window.sessionStorage.getItem(currentRoute);

        // Restore view model state
        if (viewmodelPreviousSave) {
            let jsonParsed = JSON.parse(viewmodelPreviousSave);
            for (let propertyName in jsonParsed) {
                (<any>this)[propertyName] = jsonParsed[propertyName];
            }

            this.viewmodel = this;
            this.viewmodel.MS = (<any>window).MainService;
        }

        this.loadParameters();

        this.MS.NavigationService.currentViewModel = this;
        this.isActivated = true;
    }

    // Called when object has navigated next -only simple cleanup logic should go here
    NavigatedNext(): void {
    }

    VerifyNavigation(): void {
        if (this.MS.UtilityService.isEdge()) {
            this.MS.UtilityService.Reload();
        }
    }

    async attached(): Promise<void> {
        await this.OnLoaded();
    }

    determineActivationStrategy(): any {
        return activationStrategy.replace; //replace the viewmodel with a new instance
    }

    saveViewModel(): any {
        let cleanedViewModel: any = {};
        let vmKeys: string[] = Object.keys(this);
        for (let i = 0; i < vmKeys.length; i++) {
            let vmKey: string = vmKeys[i];
            if (this.hasOwnProperty(vmKey) && vmKey !== 'MS' && vmKey !== 'viewmodel') {
                cleanedViewModel[vmKey] = (<any>this)[vmKey];
            }
        }
        return cleanedViewModel;
    }

    ///////////////////////////////////////////////////////////////////////
    /////////////////// Methods to override ///////////////////////////////
    ///////////////////////////////////////////////////////////////////////

    // Called when object is no longer valid
    Invalidate(): void {
        this.isValidated = false;
        this.showValidation = false;
        this.validationText = null;
        this.MS.ErrorService.Clear();
    }

    // Called when object is validating user input
    async OnValidate(): Promise<boolean> {
        if (!this.isValidated) {
            this.showValidation = true;
            return false;
        }

        this.isValidated = false;
        this.showValidation = false;
        this.MS.ErrorService.Clear();
        this.isValidated = await InitParser.executeActions(this.onValidate, this);
        this.showValidation = true;
        return this.isValidated;
    }

    // Called when object has initiated navigating next
    public async NavigatingNext(): Promise<boolean> {
        return true;
    }

    // Called when the view model is attached completely
    async OnLoaded(): Promise<void> {
        this.isValidated = false;
    }
}