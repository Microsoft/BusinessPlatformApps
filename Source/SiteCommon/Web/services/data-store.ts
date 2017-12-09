import { DataStoreType } from '../enums/data-store-type';

import { DataStoreItem } from '../models/data-store-item';

import { Dictionary } from './dictionary'
import { MainService } from './main-service';

export class DataStore {
    CurrentRoutePage: string = '';
    DeploymentIndex: string = '';
    private MS: MainService;
    PrivateDataStore: Dictionary<Dictionary<any>>;
    PublicDataStore: Dictionary<Dictionary<any>>;

    constructor(MainService: MainService) {
        this.MS = MainService;
        this.PrivateDataStore = new Dictionary<Dictionary<any>>();
        this.PublicDataStore = new Dictionary<Dictionary<any>>();
        this.loadDataStores();
    }

    addObjectToDataStore(value: any, dataStoreType: DataStoreType): void {
        for (let propertyName in value) {
            this.updateValue(dataStoreType, this.currentRoute(), propertyName, value[propertyName]);
        }
        this.cacheDataStores();
    }

    addObjectToDataStoreWithCustomRoute(route: string, value: any, dataStoreType: DataStoreType): void {
        for (let propertyName in value) {
            this.updateValue(dataStoreType, route, propertyName, value[propertyName]);
        }
        this.cacheDataStores();
    }

    addTestToDataStore(key: string, value: boolean): void {
        if (value === null || value === undefined) {
            value = false;
        }
        this.updateValue(DataStoreType.Public, this.currentRoute(), key, value.toString());
        this.cacheDataStores();
    }

    addToDataStore(key: string, value: any, dataStoreType: DataStoreType): void {
        this.updateValue(dataStoreType, this.currentRoute(), key, value);
        this.cacheDataStores();
    }

    addToDataStoreWithCustomRoute(route: string, key: string, value: any, dataStoreType: DataStoreType): void {
        this.updateValue(dataStoreType, route, key, value);
        this.cacheDataStores();
    }

    currentRoute(): string {
        return this.CurrentRoutePage + '-' + this.DeploymentIndex;
    }

    getAllDataStoreItems(key: string, dataStoreType: DataStoreType = DataStoreType.Any): DataStoreItem[] {
        return this.getValueAndRoutesFromDataStore(dataStoreType, key);
    }

    getAllJson(key: string, dataStoreType: DataStoreType = DataStoreType.Any): any[] {
        return this.getAllValueFromDataStore(key, dataStoreType);
    }

    getAllValues(key: string, dataStoreType: DataStoreType = DataStoreType.Any): string[] {
        return this.getAllValueFromDataStore(key, dataStoreType).map(p => p.toString());
    }

    getJson(key: string, dataStoreType: DataStoreType = DataStoreType.Any): any {
        return this.getFirstValueFromDataStore(key, dataStoreType);
    }

    getJsonWithRoute(route: string, key: string, dataStoreType: DataStoreType = DataStoreType.Any): any {
        return this.getValueWithRouteAndKey(dataStoreType, route, key);
    }

    getValue(key: string, dataStoreType: DataStoreType = DataStoreType.Any): string {
        let val = this.getLastValueFromDataStore(key, dataStoreType);
        if (val || val === '') {
            return val.toString();
        }
        return val;
    }

    getValueWithRoute(route: string, key: string, dataStoreType: DataStoreType = DataStoreType.Any): string {
        return this.getValueWithRouteAndKey(dataStoreType, route, key).toString();
    }

    keyExists(key: string, dataStoreType: DataStoreType = DataStoreType.Any): boolean {
        return this.getValueAndRoutesFromDataStore(dataStoreType, key).length > 0;
    }

    loadDataStoreFromJson(value: any): void {
        if (value) {
            let privateStore = value.PrivateDataStore;
            let publicStore = value.PublicDataStore;

            if (privateStore) {
                this.PrivateDataStore = new Dictionary<Dictionary<any>>();
                for (let route in privateStore) {
                    let valueToAdd: Dictionary<any> = new Dictionary<any>();
                    for (let key in privateStore[route]) {
                        valueToAdd.add(key, privateStore[route][key]);
                    }
                    this.PrivateDataStore.add(route, valueToAdd);
                }
            }

            if (publicStore) {
                this.PublicDataStore = new Dictionary<Dictionary<any>>();
                for (let route in publicStore) {
                    let valueToAdd: Dictionary<any> = new Dictionary<any>();
                    for (let key in publicStore[route]) {
                        valueToAdd.add(key, publicStore[route][key]);
                    }
                    this.PublicDataStore.add(route, valueToAdd);
                }
            }
        }
    }

    routeAndKeyExists(route: string, key: string, dataStoreType: DataStoreType = DataStoreType.Any): boolean {
        var found: boolean = false;

        if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {
            if (this.PrivateDataStore.containsKey(route)) {
                if (this.PrivateDataStore.get(route).containsKey(key)) {
                    found = true;
                }
            }
        }

        if (dataStoreType === DataStoreType.Public || dataStoreType === DataStoreType.Any) {
            if (this.PublicDataStore.containsKey(route)) {
                if (this.PublicDataStore.get(route).containsKey(key)) {
                    found = true;
                }
            }
        }
        return found;
    }

    routeExists(route: string, dataStoreType: DataStoreType = DataStoreType.Any): boolean {
        var found: boolean = false;

        if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {
            if (this.PrivateDataStore.containsKey(route)) {
                found = true;
            }
        }

        if (dataStoreType === DataStoreType.Public || dataStoreType === DataStoreType.Any) {
            if (this.PublicDataStore.containsKey(route)) {
                found = true;
            }
        }
        return found;
    }

    toJSON(): any {
        var toConvert: any = {};
        toConvert.PublicDataStore = this.PublicDataStore;
        toConvert.PrivateDataStore = this.PrivateDataStore;
        toConvert.CurrentRoutePage = this.CurrentRoutePage;
        toConvert.DeploymentIndex = this.DeploymentIndex;
        return toConvert;
    }

    private static addModifyItemToDataStore(store: Dictionary<Dictionary<any>>, route: string, key: string, value: any): void {
        if (!store.containsKey(route)) {
            store.add(route, new Dictionary<any>());
        }

        if (!store.get(route).containsKey(key)) {
            store.get(route).add(key, value);
        }

        store.get(route).modify(key, value);
    }

    private cacheDataStores(): void {
        this.MS.UtilityService.saveItem(this.MS.NavigationService.appName + ' datastore', this);
    }

    private getAllValueFromDataStore(key: string, dataStoreType: DataStoreType = DataStoreType.Any): any[] {
        var items: DataStoreItem[] = this.getValueAndRoutesFromDataStore(dataStoreType, key);

        return items.map((value) => { return value.value });
    }

    private getFirstValueFromDataStore(key: string, dataStoreType: DataStoreType = DataStoreType.Any): any {
        var values: DataStoreItem[];

        if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {
            values = DataStore.getValueAndRoutesFromDataStore(this.PrivateDataStore, key);
            if (values.length > 0) {
                return values[0].value;
            }
        }

        if (dataStoreType === DataStoreType.Public || dataStoreType === DataStoreType.Any) {
            values = DataStore.getValueAndRoutesFromDataStore(this.PublicDataStore, key);
            if (values.length > 0) {
                return values[0].value;
            }
        }

        return null;
    }

    private getLastValueFromDataStore(key: string, dataStoreType: DataStoreType = DataStoreType.Any): any {
        var values: DataStoreItem[];

        if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {
            values = DataStore.getValueAndRoutesFromDataStore(this.PrivateDataStore, key);
            if (values.length > 0) {
                return values[values.length - 1].value;
            }
        }

        if (dataStoreType === DataStoreType.Public || dataStoreType === DataStoreType.Any) {
            values = DataStore.getValueAndRoutesFromDataStore(this.PublicDataStore, key);
            if (values.length > 0) {
                return values[values.length - 1].value;
            }
        }

        return null;
    }

    private getValueAndRoutesFromDataStore(dataStoreType: DataStoreType, key: string): DataStoreItem[] {
        var valuesToReturn: DataStoreItem[] = new Array<DataStoreItem>();

        if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {

            valuesToReturn = valuesToReturn.concat(DataStore.getValueAndRoutesFromDataStore(this.PrivateDataStore, key));
        }

        if (dataStoreType === DataStoreType.Public || dataStoreType === DataStoreType.Any) {
            valuesToReturn = valuesToReturn.concat(DataStore.getValueAndRoutesFromDataStore(this.PublicDataStore, key));
        }

        return valuesToReturn;
    }

    private static getValueAndRoutesFromDataStore(store: Dictionary<Dictionary<any>>, key: string): DataStoreItem[] {
        var itemsMatching = new Array<DataStoreItem>();

        for (var i = 0; i < store.length(); i++) {
            var item: [string, Dictionary<any>] = store.getItem(i);

            if (item['1'].containsKey(key)) {
                var itemToAdd: DataStoreItem = new DataStoreItem();
                itemToAdd.route = item['0'];
                itemToAdd.key = key;
                itemToAdd.value = item['1'].get(key);
                itemsMatching.push(itemToAdd);
            }
        }

        return itemsMatching;
    }

    private getValueWithRouteAndKey(dataStoreType: DataStoreType, route: string, key: string): any {
        if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {
            if (this.PrivateDataStore.containsKey(route) && this.PrivateDataStore.get(route).containsKey(key)) {
                return this.PrivateDataStore.get(route).get(key);
            }
        }

        if (dataStoreType === DataStoreType.Public || dataStoreType === DataStoreType.Any) {
            if (this.PublicDataStore.containsKey(route) && this.PublicDataStore.get(route).containsKey(key)) {
                return this.PublicDataStore.get(route).get(key);
            }
        }

        return null;
    }

    private loadDataStores(): void {
        let datastore: any = this.MS.UtilityService.getItem(this.MS.NavigationService.appName + ' datastore');
        if (!datastore) {
            this.PublicDataStore = new Dictionary<Dictionary<any>>();
            this.PrivateDataStore = new Dictionary<Dictionary<any>>();
        } else {
            this.loadDataStoreFromJson(datastore);
        }
    }

    private static updateItemIntoDataStore(store: Dictionary<Dictionary<any>>, route: string, key: string, value: any): boolean {
        var found: boolean = false;

        if (store.containsKey(route) && store.get(route).containsKey(key)) {
            found = true;
            if (value === null) {
                store.get(route).remove(key);
            } else {
                store.get(route).modify(key, value);
            }
        }

        return found;
    }

    private updateValue(dataStoreType: DataStoreType, route: string, key: string, value: any): void {
        var foundInPrivate: boolean = false;
        var foundInPublic: boolean = false;

        if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {
            foundInPrivate = DataStore.updateItemIntoDataStore(this.PrivateDataStore, route, key, value);
        }

        if (dataStoreType === DataStoreType.Public || dataStoreType === DataStoreType.Any) {
            foundInPublic = DataStore.updateItemIntoDataStore(this.PublicDataStore, route, key, value);
        }

        if (!foundInPublic && !foundInPrivate) {
            if (dataStoreType === DataStoreType.Private || dataStoreType === DataStoreType.Any) {
                DataStore.addModifyItemToDataStore(this.PrivateDataStore, route, key, value);
            }

            if (dataStoreType === DataStoreType.Public) {
                DataStore.addModifyItemToDataStore(this.PublicDataStore, route, key, value);
            }
        }
    }
}