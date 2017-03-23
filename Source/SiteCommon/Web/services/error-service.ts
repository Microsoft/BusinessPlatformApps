import { MainService } from './main-service';

export class ErrorService {
    MS: MainService;
    details: string = '';
    logLocation: string = '';
    message: string = '';
    showContactUs: boolean = false;

    constructor(MainService) {
        this.MS = MainService;
    }

    Clear() {
        this.details = '';
        this.logLocation = '';
        this.message = '';
        this.showContactUs = false;
    }
}