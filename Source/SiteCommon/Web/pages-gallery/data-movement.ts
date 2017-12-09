import { DataStoreType } from '../enums/data-store-type';

import { DataMovementType } from '../models/data-movement-type';
import { InformaticaAgent } from '../models/informatica-agent';
import { InformaticaRuntimeEnvironment } from '../models/informatica-runtime-environment';
import { ScribeAgent } from '../models/scribe-agent';
import { ScribeAgentInstall } from '../models/scribe-agent-install';
import { ScribeOrganization } from '../models/scribe-organization';

import { ViewModelBase } from '../services/view-model-base';

export class DataMovement extends ViewModelBase {
    company: string = '';
    dataMovement: string = '';
    dataMovementType: DataMovementType = new DataMovementType();
    informaticaAccount: string = 'Existing';
    informaticaAgentId: string = '';
    informaticaAgentLocation: string = '';
    informaticaAgents: InformaticaAgent[] = [];
    isRegistered: boolean = this.MS.HttpService.isOnPremise;
    nameFirst: string = '';
    nameLast: string = '';
    password: string = '';
    passwordConfirmation: string = '';
    scribeAgentId: string = '';
    scribeAgentInstall: ScribeAgentInstall = new ScribeAgentInstall();
    scribeAgents: ScribeAgent[] = [];
    scribeOrganizationId: string = '';
    scribeOrganizations: ScribeOrganization[] = [];
    showAdf: boolean = true;
    showD365: boolean = false;
    showInformatica: boolean = true;
    showScribe: boolean = true;
    subtitle: string = this.MS.Translate.DATA_MOVEMENT_SUBTITLE;
    username: string = '';

    OnDataMovementChanged(): void {
        this.onInvalidate();
        this.isValidated = this.dataMovement === this.dataMovementType.ADF || this.dataMovement === this.dataMovementType.D365;
    }

    async onLoaded(): Promise<void> {
        this.isValidated = this.dataMovement === this.dataMovementType.ADF || this.dataMovement === this.dataMovementType.D365;
    }

    async OnScribeOrganizationChanged(): Promise<void> {
        if (this.MS.HttpService.isOnPremise) {
            this.MS.DataStore.addToDataStore('ScribeOrganizationId', this.scribeOrganizationId, DataStoreType.Private);

            this.scribeAgents = [];

            let scribeAgents: ScribeAgent[] = await this.MS.HttpService.getResponseAsync('Microsoft-GetScribeAgents');

            if (scribeAgents && scribeAgents.length > 0) {
                for (let i = 0; i < scribeAgents.length; i++) {
                    let scribeAgent: ScribeAgent = scribeAgents[i];
                    if (!scribeAgent.isCloudAgent) {
                        this.scribeAgents.push(scribeAgent);
                    }
                }

                if (this.scribeAgents && this.scribeAgents.length > 0) {
                    this.scribeAgentId = this.scribeAgents[0].id;
                    this.setValidated();
                } else {
                    this.scribeAgentInstall = await this.MS.HttpService.getResponseAsync('Microsoft-GetScribeAgentInstall');
                }
            }
        }
    }

    async onValidate(): Promise<boolean> {
        this.onInvalidate();

        switch (this.dataMovement) {
            case this.dataMovementType.Informatica:
                this.MS.DataStore.addToDataStore('InformaticaUsername', this.username, DataStoreType.Private);
                this.MS.DataStore.addToDataStore('InformaticaPassword', this.password, DataStoreType.Private);

                if (this.informaticaAccount === 'New') {
                    if (this.password !== this.passwordConfirmation) {
                        this.MS.ErrorService.set(this.MS.Translate.DATA_MOVEMENT_INFORMATICA_PASSWORD_MATCH);
                        break;
                    }

                    this.MS.DataStore.addToDataStore('InformaticaCompany', this.company, DataStoreType.Private);
                    this.MS.DataStore.addToDataStore('InformaticaNameFirst', this.nameFirst, DataStoreType.Private);
                    this.MS.DataStore.addToDataStore('InformaticaNameLast', this.nameLast, DataStoreType.Private);

                    if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-RegisterInformaticaAccount')) {
                        this.informaticaAccount = 'Existing';
                        this.isRegistered = true;
                    } else {
                        break;
                    }
                }

                if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-VerifyInformaticaCredentials')) {
                    if (this.MS.HttpService.isOnPremise) {
                        let informaticaRuntimeEnvironments: InformaticaRuntimeEnvironment[] = await this.MS.HttpService.getResponseAsync('Microsoft-GetInformaticaAgents');

                        let informaticaAgents: InformaticaAgent[] = [];

                        if (informaticaRuntimeEnvironments && informaticaRuntimeEnvironments.length > 0) {
                            for (let i = 0; i < informaticaRuntimeEnvironments.length; i++) {
                                let informaticaRuntimeEnvironment: InformaticaRuntimeEnvironment = informaticaRuntimeEnvironments[i];
                                if (informaticaRuntimeEnvironment && informaticaRuntimeEnvironment.agents && informaticaRuntimeEnvironment.agents.length > 0) {
                                    for (let j = 0; j < informaticaRuntimeEnvironment.agents.length; j++) {
                                        let informaticaAgent: InformaticaAgent = informaticaRuntimeEnvironment.agents[j];
                                        if (informaticaAgent && informaticaAgent.name !== 'Informatica Cloud Hosted Agent') {
                                            informaticaAgents.push(informaticaAgent);
                                        }
                                    }
                                }
                            }
                        }

                        this.informaticaAgents = informaticaAgents;

                        if (this.informaticaAgents && this.informaticaAgents.length > 0) {
                            this.informaticaAgentId = this.informaticaAgents[0].id;
                            this.setValidated();
                        } else {
                            this.informaticaAgentLocation = await this.MS.HttpService.getResponseAsync('Microsoft-GetInformaticaAgentLocation');
                        }
                    } else {
                        this.MS.DataStore.addToDataStore('InformaticaAgentName', 'Informatica Cloud Hosted Agent', DataStoreType.Public);
                        this.setValidated();
                    }
                }

                break;
            case this.dataMovementType.Scribe:
                this.MS.DataStore.addToDataStore('ScribeUsername', this.username, DataStoreType.Private);
                this.MS.DataStore.addToDataStore('ScribePassword', this.password, DataStoreType.Private);

                this.scribeOrganizations = await this.MS.HttpService.getResponseAsync('Microsoft-GetScribeOrganizations');
                if (this.scribeOrganizations && this.scribeOrganizations.length > 0) {
                    this.scribeOrganizationId = this.scribeOrganizations[0].id;
                    if (this.MS.HttpService.isOnPremise) {
                        this.OnScribeOrganizationChanged();
                    } else {
                        this.MS.DataStore.addToDataStore('ScribeAgentName', 'Cloud Agent', DataStoreType.Public);
                        this.setValidated();
                    }
                }

                break;
        }

        return this.isValidated;
    }

    async onNavigatingNext(): Promise<boolean> {
        this.MS.DataStore.addToDataStore('DataMovement', this.dataMovement, DataStoreType.Public);

        switch (this.dataMovement) {
            case this.dataMovementType.Informatica:
                if (this.MS.HttpService.isOnPremise) {
                    let informaticaAgent: InformaticaAgent = this.informaticaAgents.find(x => x.id === this.informaticaAgentId);
                    this.MS.DataStore.addToDataStore('InformaticaAgentName', informaticaAgent.name, DataStoreType.Public);
                }
                break;
            case this.dataMovementType.Scribe:
                if (this.MS.HttpService.isOnPremise) {
                    let scribeAgent: ScribeAgent = this.scribeAgents.find(x => x.id === this.scribeAgentId);
                    this.MS.DataStore.addToDataStore('ScribeAgentName', scribeAgent.name, DataStoreType.Public);
                }
                let scribeOrganization: ScribeOrganization = this.scribeOrganizations.find(x => x.id === this.scribeOrganizationId);
                this.MS.DataStore.addToDataStore('ScribeApiToken', scribeOrganization.apiToken, DataStoreType.Private);
                this.MS.DataStore.addToDataStore('ScribeOrganizationId', scribeOrganization.id, DataStoreType.Private);
                break;
        }

        return true;
    }
}