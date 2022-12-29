import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ProductVm } from 'src/app/model/productVm';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-user-announcments-sold',
  templateUrl: './user-announcments.component.html',
  styleUrls: ['./user-announcments.component.css']
})
export class UserAnnouncmentsComponent implements OnInit {

  announcements: ProductVm[];
  timeNow: Date;
  isSiteLoaded: number = 0;

  constructor(private activatedRoute: ActivatedRoute, 
              private router: Router, 
                private productsvc: ProductsService,
                private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  siteBoughtLoaded(isLoaded){
    this.isSiteLoaded += +isLoaded;
  }

  siteActiveLoaded(isLoaded){
    this.isSiteLoaded += +isLoaded;
  }
  siteSoldLoaded(isLoaded){
    this.isSiteLoaded += +isLoaded;
  }
}
