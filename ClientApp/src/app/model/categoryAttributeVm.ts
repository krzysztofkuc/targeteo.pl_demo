import { CategoryVm } from "./categoryVm";
import { ProductAttributeVm } from "./productAttributeVm";
import { AttributeValueListVm } from "./attributeValueListVm";

export class CategoryAttributeVm{

    public PkAttributeId: number;

    public Name: string;

    public Value: string;

    public NumberFrom: number;

    public NumberTo: number;

    public Bit: boolean;

    public dateRange: Date[];
    
    public dateFrom: string;
    public dateFromDate: Date;

    public dateTo: string;

    public CategoryAttributeId: number;

    public AttributeType: string;

    public CategoryAttribute: CategoryVm;

    public ProductAttributes: ProductAttributeVm[];

    public ComboboxValues: AttributeValueListVm[];

    public ViewFilterType: string;

    public Hide: boolean;

    public SelectedValues: any[];

    public SelectedValue: any;
}