import { Component, Inject, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
// import {  MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { OverlayPanel } from 'primeng/overlaypanel';
import { LoginOverlayService } from 'src/app/services/login-overlay.service';
import { ShoppingCartService, CartItem } from 'src/app/services/shopping-cart.service';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { OneUserOrderComponent } from '../one-user-order/one-user-order.component';

@Component({
  selector: 'app-shopping-cart-dialog',
  templateUrl: './shopping-cart-dialog.component.html',
  styleUrls: ['./shopping-cart-dialog.component.css']
})
export class ShoppingCartDialogComponent implements OnInit {
  @Input() parent: OverlayPanel;
  @Input() isBasketShoppingCart: boolean = true;
  @Input() isMobilePhone: boolean;


  get items(): CartItem[] {
    return this.shoppingCartService.cartItems;
  }

  get totalSum(): number {
    return this.shoppingCartService.totalSum;
  }

  constructor(private shoppingCartService: ShoppingCartService,
              private toastr: ToastrService,
              private userContextService: UserContextServiceService,
              private loginOverlayService: LoginOverlayService,
              private router: Router) {}
  
  ngOnInit(): void {}

  clearItem(item: CartItem){
    this.shoppingCartService.clearCartItem(item);

    if(this.totalSum === 0) {
      this.closePanel();
    }
  }

  completeOrder() {

    this.closePanel();

    if(!this.userContextService.Login){
      this.loginOverlayService.showOverlay('/checkoutsummary');
      return;
    }else{
      this.router.navigate(['/checkoutsummary']);
    }

    

    // this.router.navigate(['/checkoutsummary']);

    // this.dialog.open(CompleteOrderComponent, {
    //   // position: {
    //   //   // top: '500px',
    //   //   // left: '500px'
    //   //   // top: this.cartBtn.nativeElement.offsetHeight + 10 + 'px'  ,
    //   //   // left: this.cartBtn.nativeElement.offsetLeft - 20 + 'px'
    //   // }
    // });
  }

  closePanel(){
    this.parent.hide();
  }

  decrementQuantity(item){
    if(item.quantity > 1){
      this.shoppingCartService.removeCartItem(item); 
    }
  }

  incrementQuantity(item){

    if(item.quantity + 1 > item.product.QuantityInStock){
      this.toastr.error("Niestety w magazynie pozostało tylko " + item.product.QuantityInStock + " przedmiotów " + item.product.Title);
    }

    this.shoppingCartService.addCartItem(item);
  }

  

}
