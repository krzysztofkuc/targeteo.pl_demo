import { IUserAccountVm } from "./userAccountVm";

export interface IUserAccountSummary {
    SumOverall: number;
    UserAccounts: IUserAccountVm[];
}