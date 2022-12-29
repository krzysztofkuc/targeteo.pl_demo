import { ProductVm } from "./productVm";

export class MsgToUserVm {
    
    public Id: string;
    public Content: string;
    //public OrderId: string;
    public ProductId: string;
    // public Product: ProductVm;
    public UserIdFrom: string;
    public UserIdTo: string;
    public Date: Date;
    public ConversationId: number;
}
