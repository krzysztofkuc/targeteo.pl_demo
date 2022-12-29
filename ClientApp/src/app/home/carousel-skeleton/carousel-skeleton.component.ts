import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-carousel-skeleton',
  templateUrl: './carousel-skeleton.component.html',
  styleUrls: ['./carousel-skeleton.component.css']
})
export class CarouselSkeletonComponent implements OnInit {
  responsiveOptions: any;

  constructor() { }

  ngOnInit(): void {
    this.setCarouselOptions();
  }

  setCarouselOptions() {
    this.responsiveOptions = [
      {
          breakpoint: '1024px',
          numVisible: 3,
          numScroll: 3
      },
      {
          breakpoint: '768px',
          numVisible: 2,
          numScroll: 2
      },
      {
          breakpoint: '560px',
          numVisible: 1,
          numScroll: 1
      }];
  }

}
