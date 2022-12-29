import { ProductVm } from "./productVm";
import { CategoryVm } from "./categoryVm";
import { CategoryAttributeVm } from "./categoryAttributeVm";
import { MegaMenuItem, MenuItem, TreeNode } from "primeng/api";
import WsMenuItem from "./wsMenuItem";

export class HomeVm {

    Products: ProductVm[];
    CurrentCategory: CategoryVm;
    Categories: CategoryVm[];
    CategoriesTreeNode: TreeNode[];
    CurrentAttributes: CategoryAttributeVm[];
    MenuItems: MenuItem[];
    MegaItems: MegaMenuItem[];
    CurrentCategoryBreadCrumb: WsMenuItem;
}