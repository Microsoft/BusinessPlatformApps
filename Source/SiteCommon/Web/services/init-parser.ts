import { DataStoreType } from '../enums/data-store-type';
import { VariableType } from '../enums/variable-type';

import { ActionResponse } from '../models/action-response';
import { Variable } from '../models/variable';

import { MainService } from '../services/main-service';

export class InitParser {
    public static MS: MainService;

    public static async executeActions(actions: any[], self: any): Promise<boolean> {
        let isSuccess: boolean = true;

        for (let i = 0; i < actions.length && isSuccess; i++) {
            let action: any = actions[i];
            let name: string = action.name;
            if (name) {
                var body = {};
                this.loadVariables(body, action, this.MS, self);

                var response: ActionResponse = await this.MS.HttpService.executeAsync(name, body);
                if (response.IsSuccess) {
                    this.MS.DataStore.addObjectToDataStore(response, DataStoreType.Private);
                } else {
                    isSuccess = false;
                }
            }
        }

        return isSuccess;
    }

    public static extractVariable(value: any): string {
        let resultSplit = value.split(',');
        return resultSplit[0].trim();
    }

    public static isPermanentEntryIntoDataStore(value: any): boolean {
        let resultSplit = value.split(',');

        for (let index = 0; index < resultSplit.length; index++) {
            if (index < 1) {
                continue;
            }

            let param: string = resultSplit[index].trim().toLowerCase();
            let paramSplit = param.split('=');

            if (paramSplit[0] === 'issaved' && paramSplit[1] === 'true') {
                return true;
            }
        }

        return false;
    }

    public static loadVariables(objToChange: any, obj: any, MS: MainService, self: any): void {
        this.MS = MS;
        for (let propertyName in obj) {
            let val: any = obj[propertyName];
            this.parseVariable(propertyName, val, objToChange, this.MS, self);

            if (val && typeof (val) === 'object' && propertyName !== 'onNext' && propertyName !== 'onValidate') {
                this.loadVariables(objToChange[propertyName], this.MS.UtilityService.clone(val), this.MS, self);
            }
        }
    }

    public static parseVariable(key: string, value: any, obj: any, MS: MainService, self: any): void {
        let variable: Variable = this.getVariableType(value);
        let result: string = '';
        let command: string = '';

        if (!variable.secondArgument) {
            variable.secondArgument = '';
        }

        switch (variable.type) {
            case VariableType.DataStoreGetFirst:
                command = 'this.MS.DataStore.getJson("' + variable.value + '")' + variable.secondArgument;
                break;
            case VariableType.DataStoreGetAll:
                command = 'this.MS.DataStore.getAllJson("' + variable.value + '")' + variable.secondArgument;
                break;
            case VariableType.Run:
                command = variable.value;
                break;
            case VariableType.RunAndSave:
                command = variable.value;
                break;
            case VariableType.RunAndSaveOld:
                command = variable.value;
                break;
            case VariableType.RunAndTestFalse:
                command = variable.value
                break;
            case VariableType.RunAndTestTrue:
                command = variable.value
                break;
            case VariableType.RunAndTranslate:
                command = variable.value;
                break;
            case VariableType.Static:
                command = '';
                result = variable.value;
                break;
        }

        if (command && self) {
            result = eval(command);
        }

        obj[key] = result;
        if (variable.saveToDataStore) {
            MS.DataStore.addToDataStore(key, result, DataStoreType.Private);
        }
    }

    public static translateInitValue(value: string, MS: MainService = null): any {
        if (MS !== null) {
            this.MS = MS;
        }

        if (/^\$translate\(.*\)/.test(value)) {
            let variable: Variable = new Variable();
            this.processInitValue(value, variable, VariableType.RunAndTranslateList, '$translate(');
            value = eval(variable.value);
        }
        return value;
    }

    private static getVariableType(value: any): Variable {
        let variable: Variable = new Variable();

        if (!value) {
            variable.type = VariableType.Static;
            variable.value = value;
            return variable;
        } else if (Array.isArray(value)) {
            for (let i = 0; i < value.length; i++) {
                value[i] = this.translateInitValue(value[i]);
            }
        } else if (typeof value === 'object') {
            for (let key in value) {
                if (value.hasOwnProperty(key)) {
                    if (Array.isArray(value[key])) {
                        for (let i = 0; i < value[key].length; i++) {
                            value[key][i] = this.translateInitValue(value[key][i]);
                        }
                    } else {
                        value[key] = this.translateInitValue(value[key]);
                    }
                }
            }
        }

        let ciValue: string = value.toString().toLowerCase();

        variable.type = VariableType.NotValid;

        if (ciValue[0] !== '$') {
            variable.type = VariableType.Static;
            variable.value = value;
        } else if (/^\$ds\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.DataStoreGetFirst, '$ds(');
        } else if (/^\$dsall\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.DataStoreGetAll, '$dsall(');
        } else if (/^\$run\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.Run, '$run(');
        } else if (/^\$isfalse\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.RunAndTestFalse, '$isfalse(');
        } else if (/^\$istrue\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.RunAndTestTrue, '$istrue(');
        } else if (/^\$translate\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.RunAndTranslate, '$translate(');
        } else if (/^\$\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.RunAndSaveOld, '$(');
        } else if (/^\$save\(.*\)/.test(ciValue)) {
            this.processInitValue(value, variable, VariableType.RunAndSave, '$save(');
        }

        return variable;
    }

    private static processInitValue(value: any, variable: Variable, type: VariableType, wrapper: string): void {
        value = value.substring(wrapper.length).trim();
        let index = value.lastIndexOf(')');
        let dsValue = value.substring(0, index);

        if (value.length >= index + 1) {
            let dsValue2 = value.substring(index + 1);
            variable.secondArgument = dsValue2;
        }

        if (type === VariableType.RunAndSaveOld) {
            dsValue = this.extractVariable(dsValue);
        }

        switch (type) {
            case VariableType.Run:
                dsValue = dsValue.replace('this.', 'self.');
                break;
            case VariableType.RunAndSave:
                dsValue = dsValue.replace('this.', 'self.');
                variable.saveToDataStore = true;
                break;
            case VariableType.RunAndSaveOld:
                dsValue = dsValue.replace('this.', 'self.');
                variable.saveToDataStore = this.isPermanentEntryIntoDataStore(dsValue);
                break;
            case VariableType.RunAndTestFalse:
                dsValue = `self.MS.DataStore.getValue('${dsValue}') === 'false'`;
                break;
            case VariableType.RunAndTestTrue:
                dsValue = `self.MS.DataStore.getValue('${dsValue}') === 'true'`;
                break;
            case VariableType.RunAndTranslate:
                dsValue = 'self.MS.Translate.' + dsValue;
                break;
            case VariableType.RunAndTranslateList:
                dsValue = 'this.MS.Translate.' + dsValue;
                break;
        }

        variable.type = type;
        variable.value = dsValue;
    }
}