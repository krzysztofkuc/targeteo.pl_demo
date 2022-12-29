import { Component, Input, OnInit } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';
import { ProductVm } from 'src/app/model/productVm';
import { ActivatedRoute, Router } from '@angular/router';
import { CartItem, ShoppingCartService } from 'src/app/services/shopping-cart.service';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery-9';
import { ToastrService } from 'ngx-toastr';
import { OrderOpinionVm } from 'src/app/model/orderOpinionVm';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit {

  @Input() product: ProductVm;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[] = [];
  isLoading: boolean = true;
  opinions: OrderOpinionVm[];
  avgUserEval: number;
  numberOfUserEvaluation: number;
  isMobilePhone:boolean = window.screen.width < 600;

  constructor(private route: ActivatedRoute, 
              private productService: ProductsService, 
              private router: Router,
              private shoppingCartService: ShoppingCartService,
              private toastr: ToastrService) { }
              
  ngOnInit(): void {

    var id = this.route.snapshot.paramMap.get('id');

    this.galleryOptions = [
      {
          // width: '600px',
          // height: '400px'
          // width: '300px',
          //breakpoint: 400,
          // height: '600px',
          // imagePercent: 80,
          previewCloseOnClick: true,
          previewCloseOnEsc: true,
          thumbnailsPercent: 20,
          thumbnailsMargin: 20,
          thumbnailMargin: 20,
          thumbnailsColumns: 4,
          preview: true,
          imageAnimation: NgxGalleryAnimation.Slide
      }
  ];

      this.productService.GetProduct(id).subscribe(res => {
        this.avgUserEval = res.AvgUserEvaluation;
        this.numberOfUserEvaluation = res.NumberOfUserEvaluation;
        
        this.product = res.CurrentProduct;
        this.product.Pictures.forEach( item => {
          this.galleryImages.push({
                                  small: "assets/upload/" + item.Path,
                                  medium: "assets/upload/" + item.Path,
                                  big: "assets/upload/" + item.Path
                                });
        });

        this.isLoading = false;

        this.productService.GetProductOpinions(this.product.ProductId).subscribe(res => {
          this.opinions = res;
          this.isLoading = false;
        });
        
      });
  }

  addItemToCart(event, item: ProductVm) {

    if(!(item.QuantityInStock > 0)){
      this.toastr.error("Produkt tymczasowo niedostępny.");
      return;
    }

    var cartItem = new CartItem();
    cartItem.product = item;

    try{
      this.shoppingCartService.addCartItem(cartItem);
      this.shoppingCartService.openShoppingCartOverlay(event);
      this.shoppingCartService.openShoppingCartModal(event);
    }
    catch(e)
    {
      this.toastr.error(e);
    }
  }

  buy(event, product){
    
  }

  sellerDetailsClick(event){
    this.router.navigate(['/sellerDetails'], { queryParams: {sellerId: this.product.User.Id}});
  }
}
