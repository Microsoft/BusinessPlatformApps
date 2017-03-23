import { VariableType } from '../enums/variable-type';

export class Variable {
    secondArgument: string = '';
    saveToDataStore: boolean = false;
    type: VariableType;
    value: any = '';
}