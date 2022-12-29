import { PictureVm } from "./pictureVm";
import { CategoryVm } from "./categoryVm";
import { ProductAttributeVm } from "./productAttributeVm";
import { CityVm } from "./cityVm";
import { SupplyMethodVm } from "./supplyMethodVm";
import { UserSupplyMethodVm } from "./userSupplyMethodVm";

export class  ProductVm {

    public Uuid?: any;
    
    public ProductId: string;

    public Title: string;

    public Name: string;

    public Description: string;

    public Price: number;

    public ThumbPath: string;

    public Pictures: PictureVm[];

    //here is a problem with srialization on view Add new Product
    public Category: CategoryVm;

    public Attributes: ProductAttributeVm[];

    public Removed?: boolean;

    public Hidden: boolean;

    public QuantityInStock: number;

    public DiscountFromPrice: number;

    public UserId: string;

    public User: any;
    public City: CityVm;
    public CityId: number;

    public Date: Date;
    public DateTo: Date;
}