import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { OverlayPanel } from 'primeng/overlaypanel';
import { ShoppingCartService } from 'src/app/services/shopping-cart.service';

@Component({
  selector: 'app-shopping-cart-btn',
  templateUrl: './shopping-cart-btn.component.html',
  styleUrls: ['./shopping-cart-btn.component.css']
})
export class ShoppingCartBtnComponent implements OnInit {
  
  @ViewChild('op') op: OverlayPanel;
  @Input() fontSize: string;

  get SumOfItems() : number {
    return this.shoppingCartService.totalItemsCount;
  }

  constructor(private shoppingCartService: ShoppingCartService) { }

  ngOnInit(): void {
  }

  shoppingCartClikced(event) {

    var isMobilePhone = window.screen.width < 600;
    if(isMobilePhone){
      this.shoppingCartService.openShoppingCartModal(event);
    }else{
      this.shoppingCartService.openShoppingCartOverlay(event);
      this.op.toggle(event);
    }
  }
}
