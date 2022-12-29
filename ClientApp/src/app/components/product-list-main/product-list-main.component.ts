import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ProductVm } from 'src/app/model/productVm';
import { ProductsService } from 'src/app/services/products.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ShoppingCartService, CartItem } from 'src/app/services/shopping-cart.service';
import { ToastrService } from 'ngx-toastr';
import { SelectItem } from 'primeng/api';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { CategoryAttributeVm } from 'src/app/model/categoryAttributeVm';
import { Dialog } from 'primeng/dialog';

@Component({
  selector: 'app-product-list-main',
  templateUrl: './product-list-main.component.html',
  styleUrls: ['./product-list-main.component.scss']
})
export class ProductListMainComponent implements OnInit{

  displayFiltersModal: boolean = false;
  sortOptions: SelectItem[];
  sortKey: string;
  productsLayout: string = "list";
  isMobilePhone:boolean = window.screen.width < 700;
  @ViewChild('filterModalDialog') filtersDialog: Dialog;

  // get products(): ProductVm[] {
  //   return this.productService.filteredProducts;
  // }

  get filters(): CategoryAttributeVm[] {
    return this.productService.filtersForProducts;
  }

  get isLoadingProducts(): boolean {
    return this.productService.isLoading;
  }

  get isAdmin(): boolean {
    return this.userContextService.IsAdmin;
  }

  constructor(private productService: ProductsService,
              private router: Router,
              private toastr: ToastrService,
              private userContextService: UserContextServiceService,
              private activatedRoute: ActivatedRoute,
              private shoppingCartService: ShoppingCartService) { }

  ngOnInit(){

    //fill order by component
    this.activatedRoute.queryParams.subscribe(params => {
        this.sortKey = params['orderBy'];
    });

    this.sortOptions = [
      {label: 'bez sortowania', value: 'none'},
      {label: 'Cena - od najmniejszej', value: 'priceAsc'},
      {label: 'Cena - od największej', value: 'priceDesc'}
    ];

    this.productsLayout = 'grid';
    localStorage.setItem('productListLayout', this.productsLayout);
  }

  onSortChange(event){
    let value = event.value;

    this.router.navigate(
      [],
      {
        relativeTo: this.activatedRoute,
        queryParams: {orderBy: value},
        queryParamsHandling: "merge", // remove to replace all query params by provided
      });
  }

  deleteProduct(product: ProductVm){
    this.productService.DeleteProduct(product).subscribe( res =>{
      this.toastr.success("Usunięto");
    } );
  }

  editProduct(product: ProductVm){

    this.router.navigate(['/edit-product'], { queryParams: {id: product.ProductId }});
  }

  addItemToCart(item: ProductVm, e: MouseEvent) {
    e.stopPropagation();
    var cartItem = new CartItem();
    cartItem.product = item;
    this.shoppingCartService.addCartItem(cartItem);
  }

  gotToProductDetails(product: ProductVm){
    this.router.navigate(['/produkt/' + product.ProductId]);
  }

  showFilters() {
    this.displayFiltersModal = true;
  }

  layoutOptionCLicked(event){
    var l = event.layout;
    localStorage.setItem('productListLayout', l);

  }

  onLazyLoad(event){
    this.productsLayout = localStorage.getItem('productListLayout');
  }

  onCloseMainFilters(value: boolean){
    this.displayFiltersModal = false;
  }
}
