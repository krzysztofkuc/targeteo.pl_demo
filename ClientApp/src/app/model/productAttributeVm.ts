import { ProductVm } from "./productVm";
import { CategoryAttributeVm } from "./categoryAttributeVm";
import { AttributeValueListVm } from "./attributeValueListVm";

export class ProductAttributeVm {

        public ProductAttributeId: number;

        public ProductOfAttributeId: number;

        public Fk_CategoryAttributes: number;

        public Value: string;

        public Product: ProductVm;

        public CategoryAttribute: CategoryAttributeVm;

        public ComboboxValues: AttributeValueListVm[];

        public ViewFilterType: string;

}