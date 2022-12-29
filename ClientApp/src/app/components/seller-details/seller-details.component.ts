import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { orderDetailVm } from 'src/app/model/orderDetailVm';
import { OrderOpinionVm } from 'src/app/model/orderOpinionVm';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-seller-details',
  templateUrl: './seller-details.component.html',
  styleUrls: ['./seller-details.component.css']
})
export class SellerDetailsComponent implements OnInit {
  opinions: OrderOpinionVm[];

  constructor(private route: ActivatedRoute, private productSvc: ProductsService) { }

  ngOnInit(): void {

    this.route.queryParams.subscribe({
      next: params => {
        var userId = params['sellerId'];  

        this.productSvc.GetUserOpinions(userId).subscribe({
          next: res => {
            this.opinions = res;
          }
        });
      }, 
      error: error => {
        //handle error
      }});

  }


}
