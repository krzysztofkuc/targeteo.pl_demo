import { User } from "oidc-client";
import { OneUserOrderVm } from "./oneUserOrderVm";

export class OrderSummaryVm {
    
    public Id: string;
    public PaymentUrl: string;
    public Date: Date;
    public OneUserOrders: OneUserOrderVm[];
    
    // public checkbox: boolean;
}