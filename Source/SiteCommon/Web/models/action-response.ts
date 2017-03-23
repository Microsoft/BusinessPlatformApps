import { ActionStatus } from '../enums/action-status';

import { ActionResponseExceptionDetail } from '../models/action-response-exception-detail';

export class ActionResponse {
    Body: any;
    Status: ActionStatus;
    DataStore: any;
    DoesResponseContainsCredentials: boolean;
    ExceptionDetail: ActionResponseExceptionDetail;
    IsSuccess: boolean;
}