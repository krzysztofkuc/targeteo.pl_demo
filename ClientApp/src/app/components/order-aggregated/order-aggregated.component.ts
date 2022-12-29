import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { AngularInpostGeowidgetService, GeowidgetTypeEnum } from 'angular-inpost-geowidget';
import { ToastrService } from 'ngx-toastr';
import { OneUserOrderVm } from 'src/app/model/oneUserOrderVm';
import { OrderService } from 'src/app/services/order.service';
import { CartItem, ShoppingCartService } from 'src/app/services/shopping-cart.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import * as _ from 'lodash'
import { orderDetailVm } from 'src/app/model/orderDetailVm';


@Component({
  selector: 'app-order-aggregated',
  templateUrl: './order-aggregated.component.html',
  styleUrls: ['./order-aggregated.component.css']
})
export class OrderAggregatedComponent implements OnInit {

  get items(): CartItem[] {
    return this.shoppingCartService.cartItems;
  }

  resultsX: OneUserOrderVm[];
  results: OneUserOrderVm[]= [];
  isOrdering: boolean = false;

  constructor(private http: HttpClient,
              private fb: FormBuilder,
              private orderService: OrderService,
              private toastr: ToastrService,
              public angularInpostGeowidgetService: AngularInpostGeowidgetService,
              private shoppingCartService: ShoppingCartService,
              private router: Router) { }

  ngOnInit(): void {

    var bb = new Array<OneUserOrderVm>();

      bb =  _.chain(this.items)
        // Group the elements of Array based on `product.User.Id` property
        .groupBy("product.User.Id")
        // `key` is group's name (userId), `value` is the array of objects
        .map((value, key) => ( { Items: value }))
        .value();

        bb.forEach(element => {
          const hxx: OneUserOrderVm = new OneUserOrderVm();

          hxx.Items = element.Items as Array<CartItem>;
          hxx.OrderDetails = this.mapCartItemToOrderDetails(element.Items);
          this.results.push(hxx as OneUserOrderVm);
        });
  }

  submitOrder(){

    this.isOrdering = true;
    this.toastr.success("Przekierowanie do płatności. Dziękujemy za złożenie zamówienia.")

     this.orderService.SaveOrder(this.results).subscribe( res => {
      window.location.href = res.Value;
      this.shoppingCartService.clearItems();
    },
    error => {
      this.toastr.error("Błąd.Nie złożono zamówienia.");
      this.isOrdering = false;
    },
     () => this.isOrdering = false);
  }

  mapCartItemToOrderDetails(Items: CartItem[]): orderDetailVm[] {
    let odResult: orderDetailVm[] = [];
    Items.forEach(e => {
      var od = new orderDetailVm();
      od.Product = e.product;
      od.Quantity = e.quantity;
      od.UnitPrice = e.product.Price

      odResult.push(od);
    });

    return odResult;
  }
}


