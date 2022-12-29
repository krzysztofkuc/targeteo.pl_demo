import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { orderDetailVm } from 'src/app/model/orderDetailVm';
import { ProductVm } from 'src/app/model/productVm';
import { ProductsService } from 'src/app/services/products.service';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';

@Component({
  selector: 'app-announcement-sold',
  templateUrl: './announcement-sold.component.html',
  styleUrls: ['./announcement-sold.component.css']
})
export class AnnouncementSoldComponent implements OnInit {

  @Output() isSiteLoaded:EventEmitter<boolean> = new EventEmitter<boolean>(); 
  announcements: orderDetailVm[];

  constructor(private activatedRoute: ActivatedRoute, 
              private router: Router, 
              private user: UserContextServiceService,
                private productsvc: ProductsService,
                private toastr: ToastrService) { 
                  //  
                }

  ngOnInit(): void {

    this.productsvc.GetSoldAnnouncements(this.user.UserId).subscribe(res => {
      this.announcements = res;
      this.isSiteLoaded.emit(true);
    },
    error => {
      this.toastr.error("error");
    });
  }

  gotToProductDetails(product: ProductVm){
    this.router.navigate(['/produkt/' + product.ProductId]);
  }

  // endAnnouncementClick(event, product: ProductVm){
  //   this.productsvc.EndAnnouncementClick(product.ProductId).subscribe(res => {
  //     this.updateProductDate(res, "Zakończono");
  //   },(error) => {
  //     this.toastr.error(error.error);
  //    });
  // }

  // activateAnnouncementClick(event, product: ProductVm){
  //   this.productsvc.activateAnnouncementClick(product.ProductId).subscribe(res => {
  //     this.updateProductDate(res, "Aktywowano");
  //   },(error) => {
  //     this.toastr.error(error.error);
  //    });
  // }

  // addAnnouncementActivityDaysClick(event, product: ProductVm){
  //   this.productsvc.addAnnouncementActivityDaysClick(product.ProductId).subscribe(res => {
  //     this.updateProductDate(res, "Dodano 30 dni");
  //   },(error) => {
  //          this.toastr.error(error.error);
  //         }
  //   );
  // }

  // private startTimer(){
  //   setInterval(() => {
  //     this.timeNow = new Date()
  //   }, 300)
  // }

  editProduct(product: ProductVm){

    this.router.navigate(['/edit-product'], { queryParams: {id: product.ProductId }});
  }

  redirectToOrderSummary(orderDetailVm: orderDetailVm){

    this.router.navigate(['ordersummary'], { queryParams: { orderId: orderDetailVm.OneUserOrder.OrderSummaryId } });
  }


  // updateProductDate(product: ProductVm, msg: string){
  //   this.announcements.filter(x => x.ProductId == product.ProductId)[0].DateTo = new Date(product.DateTo);
  //     this.toastr.success(msg);
  // }
}