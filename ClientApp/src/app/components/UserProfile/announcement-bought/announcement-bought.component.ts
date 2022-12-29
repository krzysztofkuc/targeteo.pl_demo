import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { orderDetailVm } from 'src/app/model/orderDetailVm';
import { ProductsService } from 'src/app/services/products.service';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { OrderStatus, OrderStatusEng } from 'src/app/model/enums';

@Component({
  selector: 'app-announcement-bought',
  templateUrl: './announcement-bought.component.html',
  styleUrls: ['./announcement-bought.component.css']
})
export class AnnouncementBoughtComponent implements OnInit {

  @Output() isSiteLoaded:EventEmitter<boolean> = new EventEmitter<boolean>(); 
  announcements: orderDetailVm[];
  orderStatus: typeof OrderStatusEng = OrderStatusEng;

  constructor(private user: UserContextServiceService,
              private productsvc: ProductsService,
              private router: Router,
              private toastr: ToastrService) { }

  ngOnInit(): void {

    // var xx = OrderStatus.canceled;
    this.productsvc.GetBoughtAnnouncements(this.user.UserId).subscribe(res => {
      this.announcements = res;
      this.isSiteLoaded.emit(true);
    },
    error => {
      this.toastr.error("error");
    });
  }

  addOrderOpinion(orderDetailVm: orderDetailVm, event){

    this.router.navigate(['addorderopinion/' + orderDetailVm.OrderDetailId]);
    event.stopPropagation(); 
  }

  redirectToOrderSummary(orderDetailVm: orderDetailVm, event: Event){
    this.router.navigate(['ordersummary'], { queryParams: { orderId: orderDetailVm.OneUserOrder.OrderSummaryId } });
  }

}
