import { TreeNode } from "primeng/api";
import { CategoryVm } from "./categoryVm";
import { ProductVm } from "./productVm";
import { SupplyMethodVm } from "./supplyMethodVm";
import { UserLogin } from "./userLogin";

export class AddProductVm {
    
    public AllProducts:ProductVm[];

    public AllCategories: CategoryVm[];

    public AllCategoriesTreeNodeFlaten: TreeNode[];
    
    public AllCategoriesTreeNode: TreeNode[];

    public CurrentProduct: ProductVm;

    public DeletedPictures: string[];

    public User: UserLogin;

    public AllSupplyMethods: SupplyMethodVm[];

    //public CategoryVM CurrentCategory { get; set; }

    // public int iteration { get; set; }
}