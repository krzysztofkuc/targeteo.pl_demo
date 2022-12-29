import { ProductVm } from "./productVm";

export class CommentVm
{
        public Id: number;
        
        public Content: string;

        public OrderId: number;

        public UserIdFrom: string;

        public UserIdTo: string;

        public User: any;

        public ProductId: String;

        public Product: ProductVm;

        public Date: Date;

        public ConversationId: number;

        public Viewed: boolean;

        public UnreadMessages: number;
        
}