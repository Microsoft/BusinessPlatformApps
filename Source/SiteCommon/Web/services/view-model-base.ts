import { activationStrategy } from 'aurelia-router';

import { InitParser } from '../classes/init-parser';

import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';

import { MainService } from './main-service';

export class ViewModelBase {
    isActivated: boolean = false;
    isValidated: boolean = false;
    isAuthenticated: boolean = true;

    showValidation: boolean = false;
    showValidationDetails: boolean = false;
    validationText: string;

    MS: MainService;

    textNext: string = 'Next';

    onNext: any[] = [];
    onValidate: any[] = [];

    navigationMessage: string = '';
    useDefaultValidateButton: boolean = false;

    viewmodel: ViewModelBase;

    parametersLoaded: boolean = false;

    constructor() {
        this.MS = (<any>window).MainService;
        this.viewmodel = this;
    }

    loadParameters() {
        // Load the parameters from the additionalParamters section
        if (!this.parametersLoaded) {
            var parameters = this.MS.NavigationService.getCurrentSelectedPage().Parameters;
            InitParser.loadVariables(this, parameters, this.MS, this);
        }

        this.parametersLoaded = true;
    }

    async NavigateNext() {
        if (this.MS.NavigationService.isCurrentlyNavigating) {
            return;
        }

        try {
            this.isValidated = false;

            this.MS.NavigationService.isCurrentlyNavigating = true;

            let isNavigationSuccessful: boolean = await this.NavigatingNext();
            let isExtendedNavigationSuccessful: boolean = false;
            if (isNavigationSuccessful) {
                 isExtendedNavigationSuccessful = await InitParser.executeActions(this.onNext, this, this.MS, this);
            }

            this.navigationMessage = '';

            if (isNavigationSuccessful && isExtendedNavigationSuccessful) {
                let currentRoute = this.MS.NavigationService
                    .getCurrentSelectedPage()
                    .RoutePageName.toLowerCase();
                let viewmodelPreviousSave = window.sessionStorage.getItem(currentRoute);

                // Save view model state
                if (viewmodelPreviousSave) {
                    window.sessionStorage.removeItem(currentRoute);
                }

                this.viewmodel = null;
                this.MS = null;
                window.sessionStorage.setItem(currentRoute, JSON.stringify(this));
                this.viewmodel = this;
                this.MS = (<any>window).MainService;
                this.MS.NavigationService.NavigateNext();
                this.NavigatedNext();
                this.isValidated = true;
            }
        } catch (e) {
        } finally {
            this.MS.NavigationService.isCurrentlyNavigating = false;
            this.MS.DataStore.addToDataStore('HasNavigated', true, DataStoreType.Public);
        }

        setTimeout(() => {
            this.VerifyNavigation();
        }, 100);
    }

    NavigateBack() {
        if (this.MS.NavigationService.isCurrentlyNavigating) {
            return;
        }

        this.MS.NavigationService.isCurrentlyNavigating = true;
        let currentRoute = this.MS.NavigationService
            .getCurrentSelectedPage()
            .RoutePageName.toLowerCase();

        let viewmodelPreviousSave = window.sessionStorage.getItem(currentRoute);
        // Save view model state
        if (viewmodelPreviousSave) {
            window.sessionStorage.removeItem(currentRoute);
        }

        this.viewmodel = null;
        this.MS = null;
        window.sessionStorage.setItem(currentRoute, JSON.stringify(this));
        this.viewmodel = this;
        this.MS = (<any>window).MainService;

        // Persistence is lost here for maintaining pages the user has visited
        this.MS.NavigationService.NavigateBack();
        this.MS.DeploymentService.hasError = false;
        this.MS.ErrorService.Clear();

        this.MS.NavigationService.isCurrentlyNavigating = false;

        setTimeout(() => {
            this.VerifyNavigation();
        }, 100);
    }

    async activate(params, navigationInstruction) {
        this.isActivated = false;
        this.MS.UtilityService.SaveItem('Current Page', window.location.href);
        let currentRoute = this.MS.NavigationService.getCurrentSelectedPage().RoutePageName.toLowerCase();
        this.MS.UtilityService.SaveItem('Current Route', currentRoute);
        let viewmodelPreviousSave = window.sessionStorage.getItem(currentRoute);

        // Restore view model state or load new state
        if (viewmodelPreviousSave) {
            let jsonParsed = JSON.parse(viewmodelPreviousSave);
            for (let propertyName in jsonParsed) {
                this[propertyName] = jsonParsed[propertyName];
            }

            this.viewmodel = this;
            this.viewmodel.MS = (<any>window).MainService;
        } else {
            this.loadParameters();
        }

        this.MS.NavigationService.currentViewModel = this;
        this.isActivated = true;
    }

    // Called when object has navigated next -only simple cleanup logic should go here
    NavigatedNext(): void {
    }

    VerifyNavigation(): void {
        if (this.MS.UtilityService.isEdge()) {
            this.MS.NavigationService.NavigateToIndex();
        }
    }

    async attached() {
        await this.OnLoaded();
    }

    determineActivationStrategy() {
        return activationStrategy.replace; //replace the viewmodel with a new instance
    }

    ///////////////////////////////////////////////////////////////////////
    /////////////////// Methods to override ///////////////////////////////
    ///////////////////////////////////////////////////////////////////////

    // Called when object is no longer valid
    Invalidate() {
        this.isValidated = false;
        this.showValidation = false;
        this.validationText = null;
        this.MS.ErrorService.details = '';
        this.MS.ErrorService.message = '';
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
        this.isValidated = await InitParser.executeActions(this.onValidate, this, this.MS, this);
        this.showValidation = true;
        return this.isValidated;
    }

    // Called when object has initiated navigating next
    public async NavigatingNext(): Promise<boolean> {
        return true;
    }

    // Called when the view model is attached completely
    async OnLoaded() {
        this.isValidated = false;
    }
}