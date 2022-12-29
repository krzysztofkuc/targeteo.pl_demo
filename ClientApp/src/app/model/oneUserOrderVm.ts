import { CartItem } from "../services/shopping-cart.service";
import { UserSupplyMethodVm } from "./userSupplyMethodVm";
import { SupplyMethodVm } from "./supplyMethodVm";
import { orderDetailVm } from "./orderDetailVm";
import { User } from "oidc-client";

export class OneUserOrderVm {

    public Id: number;
    public FirstName: string;
    public LastName: string;
    public Address: string;
    public City: string;
    public State: string;
    public PostalCode: string;
    public Email: string;
    public Phone: string;
    public SupplyMethods: SupplyMethodVm[];
    public SelectedSupplyMethod: UserSupplyMethodVm;
    public PaczkomatDestinationAddress: string;
    public PaymentUrl: string;
    public OrderSummaryId: string;
    public User: any;
    public Status: string;
    public StatusCaption: string;
    public OrderDetails: orderDetailVm[];

    public Items: CartItem[];
    // public checkbox: boolean;
}
