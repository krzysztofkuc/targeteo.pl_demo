import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { SharedModule } from './sharedModules/shared/shared.module';
import { ShoppingCartDialogComponent } from './components/shopping-cart-dialog/shopping-cart-dialog.component';
import { HttpRequestsService } from './services/http-requests.service';
import { ProductListMainComponent } from './components/product-list-main/product-list-main.component';
import { AddProductComponent } from './components/add-product/add-product.component';

import { AddPictureComponent } from './components/add-picture/add-picture.component';
import { AuthenticationInterceptor } from 'src/app/Interceptors/authentication-interceptor';
import { RefreshTokenInterceptor } from './interceptors/refresh-token-interceptor';
import { ToastrModule } from 'ngx-toastr';
import { AuthService } from './services/auth.service';
import { RegisterFormPopupComponent } from './components/register-form-popup/register-form-popup.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ShoppingCartBtnComponent } from './components/shopping-cart-btn/shopping-cart-btn.component';
import { LoginDialogComponent } from './components/login-dialog/login-dialog.component';
import { ShoppingCartService } from './services/shopping-cart.service';
import { OneUserOrderComponent } from './components/one-user-order/one-user-order.component';
import { AddCategoryComponent } from './components/add-category/add-category.component';
import { AddAttributeToProductComponent } from './components/add-attribute-to-product/add-attribute-to-product.component';
import { MainFiltersComponent } from './components/main-filters/main-filters.component';
import { DatePipe } from '@angular/common';
import { ProductDetailsComponent } from './components/product-details/product-details.component';
import { EditFilterDialogComponent } from './components/edit-filter-dialog/edit-filter-dialog.component';
import { AllOrdersComponent } from './components/all-orders/all-orders.component';


import { NgxGalleryModule } from 'ngx-gallery-9';
import {ConfirmationService} from 'primeng/api';


//PrimeNg
import { PanelMenuModule } from 'primeng/panelmenu';
import {CascadeSelectModule} from 'primeng/cascadeselect';
import {TreeModule} from 'primeng/tree';
import {DropdownModule} from 'primeng/dropdown';
import {MultiSelectModule} from 'primeng/multiselect';
import {CalendarModule} from 'primeng/calendar';
import {InputNumberModule} from 'primeng/inputnumber';
import {DividerModule} from 'primeng/divider';
import {DataViewModule} from 'primeng/dataview';
import {InputTextareaModule} from 'primeng/inputtextarea';
import {InputTextModule} from 'primeng/inputtext';
import {OverlayPanelModule} from 'primeng/overlaypanel';
import {PanelModule} from 'primeng/panel';
import {AvatarModule} from 'primeng/avatar';
import {BadgeModule} from 'primeng/badge';
import {AccordionModule} from 'primeng/accordion';
import {ListboxModule} from 'primeng/listbox';
import {DialogModule} from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import {SkeletonModule} from 'primeng/skeleton';
import {MenubarModule} from 'primeng/menubar';
import {BreadcrumbModule} from 'primeng/breadcrumb';
import {MessageModule} from 'primeng/message';
import {CardModule} from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {DialogService, DynamicDialogConfig, DynamicDialogModule, DynamicDialogRef} from 'primeng/dynamicdialog';
import {SidebarModule} from 'primeng/sidebar';
import {CarouselModule} from 'primeng/carousel';
import {RadioButtonModule} from 'primeng/radiobutton';
import {TableModule} from 'primeng/table';
import {MegaMenuModule} from 'primeng/megamenu';
import { AngularInpostGeowidgetModule } from 'angular-inpost-geowidget';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {ProgressBarModule} from 'primeng/progressbar';
import { PrivacyPolicyComponent } from './components/privacy-policy/privacy-policy.component';
import { RegulationsComponent } from './components/regulations/regulations.component';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {EditorModule} from 'primeng/editor';
import {TabMenuModule} from 'primeng/tabmenu';
import {TabViewModule} from 'primeng/tabview';
import { FileUploadModule} from 'primeng/fileupload';
import {RatingModule} from 'primeng/rating';
import {ImageModule} from 'primeng/image';
import {MessagesModule} from 'primeng/messages';
import {InputMaskModule} from 'primeng/inputmask';
import { OrderSummaryComponent } from './components/order-summary/order-summary.component';
import { ProductDetailsSkeletonComponent } from './components/product-details/product-details-skeleton/product-details-skeleton.component';
import { CarouselSkeletonComponent } from './home/carousel-skeleton/carousel-skeleton.component';
import { ShopSolutionsInfoComponent } from './home/shop-solutions-info/shop-solutions-info.component';
import { RegisterFormComponent } from './components/register-form/register-form.component';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { AskQuestionAboutProductComponent } from './components/product-details/ask-question-about-product/ask-question-about-product.component';
import { UserMessagesComponent } from './components/UserProfile/user-messages/user-messages.component';
import { UserProductMessagesComponent } from './components/UserProfile/user-product-messages/user-product-messages.component';
import { MapComponent } from './components/map/map.component';
import { UserAnnouncmentsComponent } from './components/UserProfile/user-announcments/user-announcments.component';
import { UserAnnouncmentsResolverService } from './components/UserProfile/user-announcments/user-announcments-resolver.service';
import { SearchProductsComponent } from './components/search-products/search-products.component';
import { AddAttributesToNewProductComponent } from './components/add-attributes-to-new-product/add-attributes-to-new-product.component';
import { AnnouncementBoughtComponent } from './components/UserProfile/announcement-bought/announcement-bought.component';
import { AnnouncementSoldComponent } from './components/UserProfile/announcement-sold/announcement-sold.component';
import { AnnouncementActiveComponent } from './components/UserProfile/announcement-active/announcement-active.component';
import { AddOrderOpinionComponent } from './components/add-order-opinion/add-order-opinion.component';
import { SellerDetailsComponent } from './components/seller-details/seller-details.component';
import { AddSupplyMethodDialogComponent } from './components/add-supply-method-dialog/add-supply-method-dialog.component';
import { ValidationSummaryInfoComponent } from './components/validation-summary-info/validation-summary-info.component';
import { OrderAggregatedComponent } from './components/order-aggregated/order-aggregated.component';
import { UserProfileComponent } from './components/UserProfile/user-profile/user-profile.component';
import { UserAccountComponent } from './components/UserProfile/user-account/user-account.component';
import { WithdrawMoneyComponent } from './components/withdraw-money/withdraw-money.component';
import { ProductMainListSkeletonComponent } from './components/product-main-list-skeleton/product-main-list-skeleton.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ShoppingCartDialogComponent,
    ProductListMainComponent,
    AddProductComponent,
    AddPictureComponent,
    RegisterFormPopupComponent,
    ResetPasswordComponent,
    ShoppingCartBtnComponent,
    LoginDialogComponent,
    OneUserOrderComponent,
    AddCategoryComponent,
    AddAttributeToProductComponent,
    MainFiltersComponent,
    ProductDetailsComponent,
    EditFilterDialogComponent,
    AllOrdersComponent,
    PrivacyPolicyComponent,
    RegulationsComponent,
    OrderSummaryComponent,
    ProductDetailsSkeletonComponent,
    CarouselSkeletonComponent,
    ShopSolutionsInfoComponent,
    RegisterFormComponent,
    ConfirmEmailComponent,
    AskQuestionAboutProductComponent,
    UserMessagesComponent,
    UserProductMessagesComponent,
    MapComponent,
    UserAnnouncmentsComponent,
    SearchProductsComponent,
    AddAttributesToNewProductComponent,
    AnnouncementBoughtComponent,
    AnnouncementSoldComponent,
    AnnouncementActiveComponent,
    AddOrderOpinionComponent,
    SellerDetailsComponent,
    AddSupplyMethodDialogComponent,
    ValidationSummaryInfoComponent,
    OrderAggregatedComponent,
    UserProfileComponent,
    UserAccountComponent,
    WithdrawMoneyComponent,
    ProductMainListSkeletonComponent
  ],
  entryComponents: [ShoppingCartDialogComponent],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    RouterModule.forRoot([
      { path: 'kategoria/:categoryParent/:categoryChild', component: HomeComponent },
      { path: 'kategoria/:categoryParent/:categoryChild/filtrowanie', component: HomeComponent },
      { path: 'kategoria/:categoryParent', component: HomeComponent },
      { path: 'kategoria/:categoryParent/filtrowanie', component: HomeComponent },
      { path: 'filtrowanie', component: HomeComponent },
      { path: 'allOrders', component: AllOrdersComponent },
      { path: 'checkout', component: OneUserOrderComponent },
      { path: 'checkoutsummary', component: OrderAggregatedComponent },
      { path: 'ordersummary', component: OrderSummaryComponent },
      { path: 'register', component: RegisterFormComponent },
      { path: 'confirmEmail', component: ConfirmEmailComponent },

      { path: 'admin', component: HomeComponent },
      
      { path: '', component: HomeComponent},
      { path: 'add-product', component: AddProductComponent },
      { path: 'edit-product', component: AddProductComponent },
      { path: 'produkt/:id', component: ProductDetailsComponent },
      { path: 'resetPassword', component: ResetPasswordComponent },
      { path: 'privacyPolicy', component: PrivacyPolicyComponent },
      { path: 'regulations', component: RegulationsComponent},
      { path: 'addorderopinion/:orderDetailId', component: AddOrderOpinionComponent},
      { path: 'user-profile', component: UserProfileComponent},
      { path: 'user-profile/messages', component: UserMessagesComponent},
      { path: 'user-profile/user-product-messages', component: UserProductMessagesComponent},
      { path: 'user-profile/user-announcrments', component: UserAnnouncmentsComponent},
      { path: 'user-profile/user-account', component: UserAccountComponent},
      { path: 'sellerDetails', component: SellerDetailsComponent,
        resolve: { announcements: UserAnnouncmentsResolverService }}
    ],
    { scrollPositionRestoration: 'enabled' }),
    BrowserAnimationsModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,

    //PrimeNg
    PanelMenuModule,
    CascadeSelectModule,
    TreeModule,
    DropdownModule,
    MultiSelectModule,
    CalendarModule,
    DividerModule,
    DataViewModule,
    InputNumberModule,
    InputTextareaModule,
    InputTextModule,
    InputMaskModule,
    OverlayPanelModule,
    BadgeModule,
    AvatarModule,
    AccordionModule,
    ListboxModule,
    DialogModule,
    ButtonModule,
    SkeletonModule,
    MenubarModule,
    BreadcrumbModule,
    MessageModule,
    CardModule,
    AutoCompleteModule,
    DynamicDialogModule,
    SidebarModule,
    CarouselModule,
    RadioButtonModule,
    AngularInpostGeowidgetModule,
    TableModule,
    ConfirmDialogModule,
    NgxGalleryModule,
    ProgressSpinnerModule,
    EditorModule,
    TabMenuModule,
    ProgressBarModule,
    MegaMenuModule,
    FileUploadModule,
    CheckboxModule,
    TabViewModule,
    RatingModule,
    ImageModule,
    MessagesModule,
    PanelModule,
    AvatarModule,

    ToastrModule.forRoot() // ToastrModule added

  ],
  exports: [SharedModule],
  providers: [HttpRequestsService, 
              AuthService, 
              ShoppingCartService, 
              ConfirmationService, 
              DatePipe,
              DialogService,
              DynamicDialogRef,
              DynamicDialogConfig,
    {
        provide: HTTP_INTERCEPTORS,
        useClass: AuthenticationInterceptor,
        multi: true
    },
    {
        provide: HTTP_INTERCEPTORS,
        useClass: RefreshTokenInterceptor,
        multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
