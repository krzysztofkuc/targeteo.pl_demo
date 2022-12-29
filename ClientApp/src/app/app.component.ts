import { Component, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MainModelService } from './services/main-model.service';
import { ActivatedRoute, ActivationEnd, Route, Router } from '@angular/router';
import { CategoryAttributeVm } from './model/categoryAttributeVm';
import {MegaMenuItem, MenuItem, TreeNode} from 'primeng/api';
import { HomeVm } from './model/homeVm';
import { AddCategoryComponent } from './components/add-category/add-category.component';
import { ProductsService } from './services/products.service';
import { MediaMatcher } from '@angular/cdk/layout';
import {  ToastrService } from 'ngx-toastr';
import { BreadcrumbService } from './services/breadcrumb.service';
import { UserContextServiceService } from './services/user-context-service.service';
import { DialogService } from 'primeng/dynamicdialog';
import { CategoryVm } from './model/categoryVm';
import { AutoComplete } from 'primeng/autocomplete';
import { ShoppingCartService } from './services/shopping-cart.service';
import { MapComponent } from './components/map/map.component';
import { CityVm } from './model/cityVm';
import { OverlayPanel } from 'primeng/overlaypanel';
import { SearchProductsComponent } from './components/search-products/search-products.component';
import { AddAttributeToProductComponent } from './components/add-attribute-to-product/add-attribute-to-product.component';
import WsMenuItem from './model/wsMenuItem';
import { Helpers } from 'src/app/helpers';
import { ProductVm } from './model/productVm';
import { Title } from '@angular/platform-browser';
import { SearchBarService } from './services/search-bar.service';
import { LoginDialogComponent } from './components/login-dialog/login-dialog.component';
import { LoginOverlayService } from './services/login-overlay.service';

export interface MenuItemShop extends MenuItem{
  parent: any;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  mobileSearchBarIsVisible: boolean = false;
  title = 'targeteo - platforma sprzedażowa';
  mode = new FormControl('push');
  routeLoded: boolean = false;

  items: MenuItem[];
  megaItems: MegaMenuItem[];
  categoriesTree: TreeNode[];
  itemsSteps: MenuItem[];

  parent: string;
  child: string;

  displayModalShoppingCart: boolean;

  @ViewChild('mapCmp', {static : false}) private openStreetMap:MapComponent;
  @ViewChild('op', {static : false}) private loginOverlay:OverlayPanel;
  @ViewChild('mobileSearchModalMap', {static : false}) private mobileSearchMap:SearchProductsComponent;
  timeout: any = null;

  // quickSearchResults: string[];
  quickSearchResults: ProductVm[];
  citySearchResults: string[];
  searchText: any;
  searchInCity: string;
  isLoginEnabled: boolean = false;
  crumbsTmp: MenuItem[] = [];
  showSidebar: boolean = false;
  currentCategory: CategoryVm;
  selectedMenuNodes: TreeNode[];
  isLoadingMainContent: boolean = true;
  // selectedCity: string;
  selectedCityVm: CityVm = new CityVm();
  numberOfUnreadMessages: number;
  isLoadingMenu: boolean = true;

  latitude: number;
  longitude: number;
  isMobilePhone:boolean = window.screen.width < 600;
  loginReturnUrl: string;

  // distances: string[] = ['+5 km','+10 km', '+15 km', '+30 km', '+50 km', '+100 km', '+200 km' ];
  selectedCityDistance: string;

  @ViewChild('search') private searchElement: AutoComplete;
  @ViewChild('menuTree') private menuSideBar: ElementRef;
  @ViewChild('op') private userMenu: OverlayPanel;

  constructor(private modelService: MainModelService,
              public productService: ProductsService,
              public shoppingCartService: ShoppingCartService,
              private router: Router,
              public loginOverlayService: LoginOverlayService,
              private route: ActivatedRoute,
              private toastr: ToastrService,
              private userContextService: UserContextServiceService,
              changeDetectorRef: ChangeDetectorRef, media: MediaMatcher,
              public dialogService: DialogService,
              public searchBarService: SearchBarService,
              private breadcrumb: BreadcrumbService){

    // this.mobileQuery = media.matchMedia('(max-width: 600px)');
    // this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    // this.mobileQuery.addListener(this._mobileQueryListener);

 }

 public get isAdmin() {
  return this.userContextService.IsAdmin? true : false;
}

// public get displayModalShoppingCart() {
//   return this.shoppingCartService.displayModalShoppingCart? true : false;
// }

public get isMainPage() {
  var s = this.route.snapshot;
  return s.url.length == 0 && s.params.categoryParent == null && !s.queryParams.searchString;
}

  ngOnInit(){

    //this is calling two times -> in home component
    this.productService.GetMenu().subscribe( result => {

              this.items = result;
              //heere are loading only one level;p[858585858585858585858585858585858585858585858585858585858585858585585
              // this.categoriesTree = this.items as TreeNode[];
              this.isLoadingMenu = false;
              //this.addCommandToMenuItems(this.items);
              // this.megaItems = (result.MenuItems as MegaMenuItem[]);
              // this.categoriesTree = result.CategoriesTreeNode;
              // this.addCommandToMenuItems(this.items);
    });

    if(this.userContextService.Login){
      this.productService.GetCountedUnreadMessages().subscribe( res => {
        this.productService.numberOfUnreadMessaes = res;
        // this.numberOfUnreadMessages = res;
      });
    }
    

    this.router.events.subscribe(val => {

         this.productService.isLoading = true;
         if(val instanceof ActivationEnd) {
           
          this.parent = val.snapshot.params.categoryParent;
          this.child = val.snapshot.params.categoryChild;
          var queryParams = val.snapshot.queryParamMap;

          this.isLoginEnabled = true;
          // if(val.snapshot.url[0]?.path == "admin"){
          //   this.isLoginEnabled = true;
          // }

          //this get entire home model, should get only filters model
          this.modelService.filterModel(this.parent, this.child, queryParams).subscribe(res => {
            this.isLoadingMainContent = false;
            var result = (res as HomeVm);
            this.productService.setProducts(result.Products);
            // this.productService.filteredProducts = ;
            this.productService.filtersForProducts = result.CurrentAttributes;
            this.currentCategory = res.CurrentCategory;

            //fill search
            var paramText = queryParams.get("searchString");
            this.searchBarService.searchText =  {Title : paramText};
            this.searchBarService.searchInCity = queryParams.get("city");
            this.searchBarService.selectedCityDistance = "+" + queryParams.get("distance") + " km";

            //Shpould be transfered where is loading menu ?
            //load breaCrumb
            const flattedCrumbs: WsMenuItem[] = [];
            var flatten = this.flatCrumbs(res.CurrentCategoryBreadCrumb, flattedCrumbs);
            this.breadcrumb.setCrumbs(flatten.reverse());

            //load menu
            if(!this.routeLoded){
              // this.items = result.MenuItems;
              // this.megaItems = (result.MenuItems as MegaMenuItem[]);
              this.categoriesTree = result.CategoriesTreeNode;
              this.addCommandToMenuItems(this.items);
            }

            this.routeLoded = true;
            this.productService.isLoading = false;
          });
        }

      });

      this.setShoppingCartModalVisible();


      this.shoppingCartService.closeShoppingCartOverlay$.subscribe( res => {
          this.displayModalShoppingCart = false;
          this.userMenu.hide();
      });
      
  }

  flatCrumbs(crumb: WsMenuItem, result:WsMenuItem[]): WsMenuItem[] {

    if(!crumb){
      return [];
    }

    result.push(crumb);

    if(crumb.Parent){
      this.flatCrumbs(crumb.Parent, result);
    }

    return result;
  }

  // flatten(item: WsMenuItem) {
  //   const flat = [];
  
  //   if (Array.isArray(item)) {
  //     flat.push(...flatten(item));
  //   } else {
  //     flat.push(item);
  //   }
  
  //   return flat;
  // }

  private addCommandToMenuItems(items: MenuItem[]) {
    items.forEach( item => {
      item.command = () => this.menuItemClicked(null, item);

      if(item.items){
        this.addCommandToMenuItems(item.items);
      }

    });
  }
  
  addcategory(){
    const ref = this.dialogService.open(AddCategoryComponent, {
      header: 'Add category',
      width: '70%'
  });
  }
  

editCategory(){
  const ref = this.dialogService.open(AddCategoryComponent, {
    data: {
      category: this.currentCategory
    },
    header: 'Edit category',
    width: '70%'
});
}


addProduct(){

  this.router.navigate(['/add-product']);
}

onLogin(){
  this.loginOverlayService.show = true;
  // this.dialog.open(AddCategoryComponent, {
  //   data: {
  //     Category: node
  //   }
  //   // position: {
  //   //   // top: '500px',
  //   //   // left: '500px'
  //   //   // top: this.cartBtn.nativeElement.offsetHeight + 10 + 'px'  ,
  //   //   // left: this.cartBtn.nativeElement.offsetLeft - 20 + 'px'
  //   // }
  // });
}

deleteCategory(){

  var params = this.route.snapshot.children[this.route.children.length - 1].params;
  var categoryParent = params.categoryParent;
  var categoryChild = params.categoryChild ? params.categoryChild : "";
  this.productService.DeleteCategory(categoryParent, categoryChild).subscribe(res => {
    this.toastr.success("Usunięto kategorię.");
  }),
  (error: any) => {
    this.toastr.error("Nie usunięto kategorii.");
  };
}

searchCity(event){
  this.productService.getProductNamesContains(event.query).subscribe(data => {

    this.quickSearchResults = data;
    
    // this.quickSearchResults = data.map( m => {
    //   return m.Title;
    // });
});
}

menuItemClicked(event: any, item: any) {
  if(item.children?.length == 0){
    this.closeMenuSidebar();
  }

  this.crumbsTmp = [];
  // this.setBreadCrumbs(item);
}

// setBreadCrumbs(item: MenuItemShop){

//   this.crumbsTmp.unshift(item);

//   if(item.parent){
//     this.setBreadCrumbs(item.parent);
//   }else{
//     this.breadcrumb.setCrumbs(this.crumbsTmp);
//   }
// }

  mobileSearchBarVisibility() : void{
    this.mobileSearchBarIsVisible = !this.mobileSearchBarIsVisible;

    // if(this.mobileSearchBarIsVisible){
    //   setTimeout(()=>{ // this will make the execution after the above boolean has changed
    //     this.searchElement.focusInput();
    //     this.searchElement.focus = true;
    //     // this.searchElement.focusInput();
    //   },100); 
    // }
  }

  showMenuSidebar() : void{
    this.showSidebar = true;
  }

  
  closeMenuSidebar() : void{
    this.showSidebar = false;
  }

  openLoginDialog(e){
    this.loginOverlayService.show = true;
  }

nodeSelect(ev) {
  
  if(ev.node.children == null || ev.node.children?.length == 0)
  {
    this.showSidebar = false;
  }

  if(ev.node.routerLink?.length==3){
    this.router.navigate(['/kategoria/' + ev.node.routerLink[2]]);
  }
  else{
    this.router.navigate(['/kategoria/' + ev.node.routerLink[2] + '/' + ev.node.routerLink[3]]);
  }
}

setShoppingCartModalVisible(){
  this.shoppingCartService.shoppingCartModalVisibilityChange$.subscribe( res => {
    this.displayModalShoppingCart = true;
});
}

completeOrder(event){
  this.displayModalShoppingCart = false;
  
  if(!this.userContextService.Login){
    this.loginOverlayService.showOverlay('/checkoutsummary')
    return;
  }else{
    this.router.navigate(['/checkoutsummary']);
  }
}

addNewAnnouncement() {
  this.displayModalShoppingCart = false;

  if(!this.userContextService.Login){
    this.loginOverlayService.showOverlay('/add-product')
    return;
  }else{
    this.router.navigate(['/add-product']);
  }
}

userLoggedGotoAddProduct(){
  this.loginOverlayService.show = false;
  this.router.navigate(['/add-product']);
}

loginBtnClicked(event){
  if(this.isMobilePhone){
    this.loginOverlayService.show = true;
  }else{
    this.loginOverlay.toggle(event);
  }
}

mobileSearchBarShown(event){
  // this.mobileSearchMap.reloadMap();
  this.mobileSearchMap.initMap();
  // this.timeout = setTimeout(() => {
    
  // }, 1000);
}

addAttribute(){

  const ref = this.dialogService.open(AddAttributeToProductComponent, {
    data: {
      isEditMode: false,
      // product: this.model.CurrentProduct
      },
    header: 'Dodawanie atrybutu'
  });
}

}


