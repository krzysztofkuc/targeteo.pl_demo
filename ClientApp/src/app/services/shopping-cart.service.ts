import { Injectable, Inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, Subject} from 'rxjs';
import { ProductVm } from '../model/productVm';

export class CartItem {

  constructor() {
    this.quantity = 0;    
  }
  public product: ProductVm;
  quantity: number;
  // price: number;
  // image: string;
  // name: string;
  // details: string;
  // heart: boolean;
  // uuid?: any;
  // remove?: boolean;
}

@Injectable()
export class ShoppingCartService {

  private _cartItems: CartItem[] = [];
  private _totalSum: number;
  private _totalItemsCount: number;
  private cartItemsChange:Subject<CartItem[]> = new Subject<CartItem[]>();
  public shoppingCartOverlayVisibilityChange$:Subject<boolean> = new Subject<boolean>();
  public closeShoppingCartOverlay$:Subject<boolean> = new Subject<boolean>();
  public shoppingCartModalVisibilityChange$:Subject<Event> = new Subject<Event>();


  get cartItems() : CartItem[] {
    return this._cartItems;
  }

  get totalSum() : number {

    this.countTotalSum();
    return this._totalSum;
  }

  get totalItemsCount() : number {
    this.countTotalQuantity();
    return this._totalItemsCount;
  }

  constructor(private toastr: ToastrService) {
    var obj =  JSON.parse(sessionStorage.getItem("shoppingCart"));

    if(obj){
      this._cartItems = obj;
    }
  }

  addCartItem(item: CartItem) {
    var foundItem = this.cartItems.find(x => x.product.ProductId == item.product.ProductId);

    if(foundItem?.quantity + 1> item.product.QuantityInStock){
      throw new Error("Niestety w magazynie pozostao tylko " + item.product.QuantityInStock + " produktów " + item.product.Title);
    }

    if(foundItem){
      foundItem.quantity = foundItem.quantity + 1;
    }else{
      item.quantity = item.quantity + 1;
      this.cartItems.push(item);
    }

    this.countTotalSum();
    this.countTotalQuantity();

    this.toastr.success("Dodano do koszyka","", {
      timeOut: 1500,
      positionClass: "toast-bottom-right"
    });

    sessionStorage.setItem("shoppingCart", JSON.stringify(this.cartItems));
  }

  clearCartItem(item: CartItem) {

    var foundItem = this.cartItems.find(x => x.product.ProductId == item.product.ProductId);
    if(foundItem){

      // foundItem.quantity = foundItem.quantity - 1;
      this.cartItems.splice(this.cartItems.indexOf(item), 1);
    }

    sessionStorage.setItem("shoppingCart", JSON.stringify(this.cartItems));
  }

  clearItems() {
    this._cartItems = [];
    sessionStorage.setItem("shoppingCart", JSON.stringify(this.cartItems));
  }

  removeCartItem(item: CartItem) {

    var foundItem = this.cartItems.find(x => x.product.ProductId == item.product.ProductId);
    if(foundItem && foundItem.quantity){
      foundItem.quantity = foundItem.quantity - 1;
    }

    this.countTotalSum();
    this.countTotalQuantity();

    // this.toastr.success("Dodano do koszyka","", {
    //   timeOut: 1500,
    //   positionClass: "toast-bottom-right"
    // });

    sessionStorage.setItem("shoppingCart", JSON.stringify(this.cartItems));
  }

  countTotalSum(){
    var sum = 0;
    this._cartItems.forEach( item => {
      sum += item.product.Price * item.quantity;
    });

    this._totalSum = sum;
  }

  countTotalQuantity(){
    var count = 0;

    this._cartItems.forEach( x => {
      count += x.quantity;
    });

    this._totalItemsCount = count;
  }

  openShoppingCartOverlay($event) {
    this.shoppingCartOverlayVisibilityChange$.next($event);
  }

  closeShoppingCartOverlay($event) {
    this.closeShoppingCartOverlay$.next($event);
  }

  openShoppingCartModal($event) {
    this.shoppingCartModalVisibilityChange$.next($event);
  }
  

}