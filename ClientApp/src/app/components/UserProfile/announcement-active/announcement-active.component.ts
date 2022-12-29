import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { orderDetailVm } from 'src/app/model/orderDetailVm';
import { ProductVm } from 'src/app/model/productVm';
import { ProductsService } from 'src/app/services/products.service';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';

@Component({
  selector: 'app-announcement-active',
  templateUrl: './announcement-active.component.html',
  styleUrls: ['./announcement-active.component.css']
})
export class AnnouncementActiveComponent implements OnInit {

  @Output() isSiteLoaded:EventEmitter<boolean> = new EventEmitter<boolean>(); 
  announcements: ProductVm[];

  constructor(private user: UserContextServiceService,
              private productsvc: ProductsService,
              private router: Router,
              private toastr: ToastrService) { }

  ngOnInit(): void {

    this.productsvc.GetActiveAnnouncements(this.user.UserId).subscribe(res => {
      this.announcements = res;
      this.isSiteLoaded.emit(true);
    },
    error => {
      this.toastr.error("error");
    });
  }

  editProduct(product: ProductVm){

    this.router.navigate(['/edit-product'], { queryParams: {id: product.ProductId }});
  }

  redirectToProduct(orderDetailVm: ProductVm){

    // <a [routerLink]="'/produkt/' + item.Product.ProductId">

    this.router.navigate(['produkt/' + orderDetailVm.ProductId] );
  }

}
