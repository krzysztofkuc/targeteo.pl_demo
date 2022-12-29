import { IAccountOperationStatus } from "./AccountOperationStatusVm";
import { OrderSummaryVm } from "./orderSummaryVm";

export interface IUserAccountVm {
    Id: number;
    OperationDate: Date;
    OperationAmount: number;
    OrderSummary: OrderSummaryVm;
    Status: IAccountOperationStatus;
    StatusId: number;
}
