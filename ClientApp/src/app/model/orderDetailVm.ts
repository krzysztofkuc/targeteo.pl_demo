import { OneUserOrderVm } from "./oneUserOrderVm";
import { OrderVm } from "./orderVm";
import { ProductVm } from "./productVm";

export class orderDetailVm {
    
    public OrderDetailId: number;
    public ForeignOrderId: string;
    public ProductId: string;
    public Quantity: number;
    public UnitPrice: number;
    public Product: ProductVm;
    public OneUserOrder: OneUserOrderVm;
}