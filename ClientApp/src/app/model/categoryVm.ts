import { ProductVm } from "./productVm";
import { CategoryAttributeVm } from "./categoryAttributeVm";

export class CategoryVm
{
        public CategoryId: number;

        public Name: string;

        public ParentId: number;

        public ParentName: string;

        public Parent: CategoryVm;

        public ProdId: number;

        public Categories: CategoryVm[];

        public Products: ProductVm[];

        public Attributes: CategoryAttributeVm[];

        public Icon: string;

        public Caption: string;

        public ThumbnailFileName: string;
}