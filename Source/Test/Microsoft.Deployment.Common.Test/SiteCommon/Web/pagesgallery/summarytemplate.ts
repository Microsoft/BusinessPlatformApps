import { bindable } from 'aurelia-framework';
import { ViewModelBase } from '../services/view-model-base';

export class summarytemplate {
    @bindable viewmodel = null;
}

export class Entry {
    text: string;
    value: string;

    constructor(text: string, value: string) {
        this.text = text;
        this.value = value;
    }
}

export class EntryRow {
    entries: Entry[] = [];
}

export class SummaryViewModel extends ViewModelBase {
    summaryRows: EntryRow[] = [];

    constructor() {
        super();
    }

    init(summary: any) {
        this.textNext = 'Run';

        let entryRow: EntryRow = new EntryRow();
        for (let text in summary) {
            if (summary.hasOwnProperty(text) && summary[text]) {
                entryRow.entries.push(new Entry(text, summary[text]));
                if (entryRow.entries.length > 2) {
                    this.summaryRows.push(entryRow);
                    entryRow = new EntryRow();
                }
            }
        }
        if (entryRow.entries.length > 0) {
            this.summaryRows.push(entryRow);
        }
    }
}