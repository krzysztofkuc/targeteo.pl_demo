import { CategoryVm } from "./categoryVm";
import { ProductVm } from "./productVm";
import { AttributeValueListVm } from "./attributeValueListVm";
import { ProductAttributeVm } from "./productAttributeVm";
import { AllAttributeTypes } from "./enums";
import { CategoryAttributeVm } from "./categoryAttributeVm";
import { TreeNode } from "primeng/api";

export class AddAttributeToProduct {

    public ProductAttributeId: number;

    public Name: string;

    public Value: string;

    public AttributeType: string

    public CategoryAttributeId: number;

    public Fk_CategoryAttributes: number;

    public ProductOfAttributeId: number;

    public AllCategories: CategoryVm[];

    public AllCategoriesTreeNode: TreeNode[];

    public AllAttributeTypes: AllAttributeTypes;

    public ComboboxValues: AttributeValueListVm[];

    public CurrentProduct: ProductVm;

    public CurrentProductAttribute: ProductAttributeVm;

    public AllAttributes: CategoryAttributeVm[];

    public ViewFilterType: string;

    public CategoryId: number;
}