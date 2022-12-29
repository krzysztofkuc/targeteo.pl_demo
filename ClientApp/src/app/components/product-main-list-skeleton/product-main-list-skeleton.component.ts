import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-product-main-list-skeleton',
  templateUrl: './product-main-list-skeleton.component.html',
  styleUrls: ['./product-main-list-skeleton.component.css']
})
export class ProductMainListSkeletonComponent implements OnInit {

  @Input() isLoadingProducts: boolean;
  @Input() productsLayout: string;
  dummyProducts = [0,1,2,3,4,5];

  constructor() { }

  ngOnInit(): void {
  }

}
