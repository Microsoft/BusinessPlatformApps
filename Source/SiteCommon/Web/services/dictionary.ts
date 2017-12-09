export class Dictionary<T> {
    private internalKeys: string[];
    private internalValues: T[];

    constructor() {
        this.internalKeys = new Array<string>();
        this.internalValues = new Array<T>();
    }

    add(key: string, value: T): void {
        if (this.internalKeys.indexOf(key) > -1) {
            throw new Error("Key is already inside dictionary");
        }

        this.internalKeys.push(key);
        this.internalValues.push(value);
    }

    containsKey(key: string): boolean {
        if (this.internalKeys.indexOf(key) === -1) {
            return false;
        }

        return true;
    }

    get(key: string): T {
        var index = this.internalKeys.indexOf(key);
        return this.internalValues[index];
    }

    getItem(index: number): [string, T] {
        var key: string = this.internalKeys[index];
        var value: T = this.internalValues[index];
        return [key, value];
    }

    keys(): string[] {
        return this.internalKeys;
    }

    length(): number {
        return this.internalKeys.length;
    }

    modify(key: string, value: T) {
        var index: number = this.internalKeys.indexOf(key);
        if (index === -1) {
            throw new Error('Key is not found inside dictionary');
        }
        this.internalValues[index] = value;
    }

    remove(key: string): void {
        var index = this.internalKeys.indexOf(key, 0);
        if (index > -1) {
            this.internalKeys.splice(index, 1);
            this.internalValues.splice(index, 1);
        }
    }

    toJSON(): any {
        let toConvert: any = {};

        for (var i = 0; i < this.length(); i++) {
            toConvert[this.internalKeys[i]] = this.internalValues[i];
        }

        return toConvert;
    }

    values(): any[] {
        return this.internalValues;
    }
}