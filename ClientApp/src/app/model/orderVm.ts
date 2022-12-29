import { orderDetailVm } from "./orderDetailVm";
import { UserSupplyMethodVm } from "./userSupplyMethodVm";
import { SupplyMethodVm } from "./supplyMethodVm";


export class OrderVm {
    
    public OrderId: string;
    public Username: string;
    public FirstName: string;
    public LastName: string;
    public Address: string;
    public City: string;
    public State: string;
    public PostalCode: string;
    public Phone: string;
    public Email: string;
    public Status: string;
    public Total: string;
    public OrderDate: Date;
    public OrderDetails: orderDetailVm[];
    public TrackingNo: string;
    public PaymentUrl: string;
    public SupplyMethod: UserSupplyMethodVm;

    //need to dropdown
    public OldStatus: string;
}