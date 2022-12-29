import { orderDetailVm } from "./orderDetailVm";
import { PictureVm } from "./pictureVm";

export class OrderOpinionVm {
    
    public Id: number;
    public Text: string;
    public OrderDetailId: number;
    public PictureId: number;
    public Evaluation: number;
    public OrderDetails: orderDetailVm;
    public Picture: PictureVm;
}