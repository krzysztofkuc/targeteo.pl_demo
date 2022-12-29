import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../services/products.service';
import { HomeVm } from '../model/homeVm';
import { ActivatedRoute, ActivationEnd, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { ProductVm } from '../model/productVm';
import { BreadcrumbService } from '../services/breadcrumb.service';
import { SearchBarService } from '../services/search-bar.service';
import { MenuItem } from 'primeng/api';
import WsMenuItem from '../model/wsMenuItem';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})

export class HomeComponent implements OnInit{
      // private model: HomeVm;
      menuItems: WsMenuItem[];
      isSubMenuExpanded: boolean = false;
      lastMenuActiveMenuItem: WsMenuItem;

      bestSellers: ProductVm[] = [];
      lastAddedProducts: ProductVm[] = [];

      isMainPage: boolean = false;
      responsiveOptions: any;
      isLoadingCarousel: boolean = true;
      isLoadingLastAddedProducts: boolean = true;
      isMobilePhone:boolean = window.screen.width < 600;

  constructor(private productService: ProductsService,
              private route: ActivatedRoute,
              private router: Router,
              private authService: AuthService,
              public breadcrumb: BreadcrumbService,
              public searchBarService: SearchBarService,
              private toastServcice: ToastrService) 
              {
                this.setCarouselOptions();
              }

  ngOnInit(): void {

    this.setCarouselVisibility();
    this.router.events.subscribe(val => {
      this.setCarouselVisibility();
    });
      
    
    if(this.isMainPage){
      this.productService.GetBestSellers(10).subscribe(res => {
        this.isLoadingCarousel = false;
        this.bestSellers = res;
      });

      this.productService.GetLastAddedProducts(10).subscribe(res => {
        this.isLoadingLastAddedProducts = false;
        this.lastAddedProducts = res;
      });

      this.productService.GetMenu().subscribe( result => {

        this.menuItems = result;

            this.menuItems.forEach(item => {
            item.expanded = false;
          });
        //heere are loading only one level;p[858585858585858585858585858585858585858585858585858585858585858585585
        // this.categoriesTree = this.items as TreeNode[];
        //this.isLoadingMenu = false;
        //this.addCommandToMenuItems(this.items);
        // this.megaItems = (result.MenuItems as MegaMenuItem[]);
        // this.categoriesTree = result.CategoriesTreeNode;
        // this.addCommandToMenuItems(this.items);
});
    }
  }

  setCarouselVisibility(){
    var s = this.route.snapshot;
    this.isMainPage = (s.url.length == 0 && !s?.params?.categoryParent && !s?.queryParams?.lat) && !s?.queryParams?.searchString;
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

  confirmEmail(token: string){
    this.authService.confirmEmail(token)
        .subscribe(
            (confirmed: boolean) => {
              if(confirmed){
                this.toastServcice.success("Email confirmed. You can log in");
              }
            },
            (error: any) => {
              this.toastServcice.error("Cannot confirm the email. Porpably the link is autdated.");
            }
        )
    }

    //hide if click on the same menu circle
    circleMenuClick(menuItem: WsMenuItem) {

      menuItem.expanded = !menuItem.expanded;

      this.isSubMenuExpanded = menuItem.expanded;

      if(this.lastMenuActiveMenuItem && this.lastMenuActiveMenuItem.label != menuItem.label){
        this.lastMenuActiveMenuItem.expanded = false;
      }

      this.lastMenuActiveMenuItem = menuItem;
      
      
    }
}
