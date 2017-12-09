import { ActionResponse } from './action-response';

export class DataPullStatus {
    isFinished: boolean;
    slices: any[];
    status: any[];

    constructor(dataPullStatusResponse: ActionResponse) {
        let dataPullStatusResponseBody: any = dataPullStatusResponse.Body.Value;
        let dataPullStatus: any = null;

        try {
            dataPullStatus = JSON.parse(dataPullStatusResponseBody);
        } catch (e) {
            dataPullStatus = dataPullStatusResponseBody;
        }

        this.isFinished = dataPullStatus.isFinished;

        try {
            if (dataPullStatus.status) {
                this.status = JSON.parse(dataPullStatus.status);
            }

            if (dataPullStatus.slices) {
                this.slices = JSON.parse(dataPullStatus.slices);
            }
        } catch (e) {
            // do nothing
        }
    }
}