import { Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { timer } from 'rxjs';
import { OrderStatus } from 'src/app/model/enums';
import { OneUserOrderVm } from 'src/app/model/oneUserOrderVm';
import { OrderSummaryVm } from 'src/app/model/orderSummaryVm';
import { OrderVm } from 'src/app/model/orderVm';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-order-summary',
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.css']
})
export class OrderSummaryComponent implements OnInit{
  currentOrderId: string;
  // order: OrderVm;
  orderSummary: OrderSummaryVm;
  subscription: any;
  paymentCaption: string;
  orderSum: number;
  statusCaption: string;

  constructor(private activatedRoute: ActivatedRoute, private orderService: OrderService) {}

  ngOnInit(): void {
    
    this.activatedRoute.queryParams.subscribe(params => {

      this.currentOrderId = params.orderId;

      this.orderService.GetOrders(this.currentOrderId).subscribe( result => {
        // this.order = result;
        this.orderSummary = result;

        this.orderSum =  this.calculateSum(this.orderSummary); 
        this.setStatusForAllOrders(this.orderSummary);
        if(this.orderSummary.OneUserOrders.some(element => element.Status !== "paymentCompleted")){
          this.checkStatusSetInterval();
        }

      });
    });

  }
  setStatusForAllOrders(orderSummary: OrderSummaryVm) {
    orderSummary.OneUserOrders.forEach( element => {
      this.statusCaption = this.setStatusCaption(element.Status);
      var xxxx = 5;
    })
  }

  checkStatusSetInterval(){
      this.subscription = timer(500, 500).subscribe( t => {
        this.getOrder();
      }); 
  }

  getOrder(){

    this.orderService.GetOrders(this.currentOrderId).subscribe( result => {

      this.setStatusForAllOrders(result);

    });
  }

  checkStatus() {
    this.orderService.GetOrders(this.currentOrderId)
  }

  //extract to outsite file Utils or smlt
  setStatusCaption(status: string): string{
    var statusCaption = null;
    switch(status) {
      case "waitingForPayment":
        statusCaption = OrderStatus.waitingForPayment;
        break;
      case "preparing":
        statusCaption = OrderStatus.preparing;
        break;
      case "sent":
        statusCaption = OrderStatus.sent;
        break;
      case "returnShipment":
        statusCaption = OrderStatus.returnShipment;
        break;
      case "canceled":
        statusCaption = OrderStatus.canceled;
        break;
      case "paymentCompleted":
        statusCaption =  OrderStatus.paymentCompleted;
        break;
    }

    return statusCaption;
  }

  calculateSum(orderSummary: OrderSummaryVm): number {

    var sum = 0;
    orderSummary.OneUserOrders.forEach(oUsOr => {
      oUsOr.OrderDetails.forEach(element => {
        sum = sum + element.Product.Price * +element.Quantity;
      });
      // sum = sum + oUsOr.SelectedSupplyMethod.PricePerUnit;
    });

    return sum;
  }

  pay(){
    window.location.href = this.orderSummary.PaymentUrl;
  }

}