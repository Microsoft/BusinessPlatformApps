import { DataStoreType } from '../../../../../SiteCommon/Web/enums/data-store-type';

import { SearchEntity } from '../../../../../SiteCommon/Web/models/search-entity';

import { ViewModelBase } from '../../../../../SiteCommon/Web/services/view-model-base';

export class Customize extends ViewModelBase {
    csv: FileList = null;
    csvPath: string = null;
    delimiter: string = '\r\n';
    entities: SearchEntity[] = [];
    selectedEntity: SearchEntity = null;
    maximumTotalAliases: number = 32;

    entityAdd(): void {
        super.onInvalidate();
        this.entities.push(new SearchEntity());
        this.selectedEntity = this.entities[this.entities.length - 1];
    }

    entityRemove(): void {
        super.onInvalidate();
        let entityIndex: number = this.entities.indexOf(this.selectedEntity);
        this.entities.splice(entityIndex, 1);
        this.selectedEntity = this.entities[entityIndex > this.entities.length - 1 ? entityIndex - 1 : entityIndex];
    }

    entityUpload(): void {
        super.onInvalidate();
        this.MS.UtilityService.readFile(this.csv[0], (content: any) => {
            const data: string[][] = this.MS.UtilityService.parseCsv(content);
            const entities: SearchEntity[] = [];

            const headers: string[] = data[0];

            for (let i = 0; i < headers.length; i++) {
                const entity: SearchEntity = new SearchEntity();
                entity.name = headers[i];
                entity.values = '';
                entities.push(entity);
            }

            for (let i = 1; i < data.length; i++) {
                const row: string[] = data[i];
                for (let j = 0; j < row.length; j++) {
                    if (row[j]) {
                        entities[j].values += row[j] + this.delimiter;
                    }
                }
            }

            this.entities = this.entities.concat(entities);
            this.selectedEntity = this.entities[this.entities.length - 1];

            this.csvPath = null;
        });
    }

    async onLoaded(): Promise<void> {
        const entities: SearchEntity[] = JSON.parse(this.MS.DataStore.getValue('UserDefinedEntities'));
        this.entities = entities || [];
    }

    async onValidate(): Promise<boolean> {
        super.onInvalidate();
        const validator: any = {};

        for (let i = 0; i < this.entities.length; i++) {
            const entity: SearchEntity = this.entities[i];

            entity.name = entity.name.trim() || this.MS.Translate.COMMON_BLANK;
            entity.values = entity.values.trim();

            if (validator[entity.name]) {
                validator[entity.name].values += this.delimiter + entity.values;
            } else {
                validator[entity.name] = entity;
            }
        }

        const validEntities: SearchEntity[] = [];
        for (let key in validator) {
            if (validator.hasOwnProperty(key)) {
                validEntities.push(validator[key]);
            }
        }

        let allValidAliases: string[] = [];
        for (let entity of validEntities) {
            const entityAliases = entity.values.split('\n');
            allValidAliases = allValidAliases.concat(entityAliases);
        }
        if (allValidAliases.length > 0 && allValidAliases.length <= this.maximumTotalAliases) {
            this.setValidated();

            for (let i = 0; i < allValidAliases.length; i++) {
                allValidAliases[i] = `"${allValidAliases[i]}"`;
            }
            const searchString: string = allValidAliases.join('|');
            this.MS.DataStore.addToDataStore("SearchCriteria", searchString, DataStoreType.Public);

            this.MS.DataStore.addToDataStore('UserDefinedEntities',
                JSON.stringify(validEntities),
                DataStoreType.Public);
            this.showValidation = true;
        } else {
            this.MS.ErrorService.set(`Must have between 0 and 32 entity aliases in total.  You had ${allValidAliases.length} aliases across ${validEntities.length} entities.`);
        }
        return this.isValidated;
    }
}