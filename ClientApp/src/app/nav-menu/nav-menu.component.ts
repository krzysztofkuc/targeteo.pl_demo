import { Component, ViewChild, ElementRef, EventEmitter, Output, Input } from '@angular/core';
import { ShoppingCartDialogComponent } from '../components/shopping-cart-dialog/shopping-cart-dialog.component';
import { LoginDialogComponent } from '../components/login-dialog/login-dialog.component';
import { ShoppingCartService, CartItem } from '../services/shopping-cart.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  @ViewChild('loginDialogButton',  {read: ElementRef, static: true}) loginDialogBtn: ElementRef;
  
  @Output() eventClicked = new EventEmitter<Event>();

  @Input() categories: MenuItem[];

  get SumOfItems(): number {
    if(!this.shoppingCartService.totalItemsCount){
      return 0;
    }
    return this.shoppingCartService.totalItemsCount;
  }

  constructor(private shoppingCartService: ShoppingCartService) {  
  }

  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  openLoginDialog() {
    // this.dialog.open(LoginDialogComponent, {
    //   position: {
    //     // top: '500px',
    //     // left: '500px'
    //     top: this.loginDialogBtn.nativeElement.offsetHeight + 10 + 'px'  ,
    //     left: this.loginDialogBtn.nativeElement.offsetLeft - 20 + 'px'
    //   }
    // });
}

  moveMenuClick(){
    this.eventClicked.emit();
  }
}
