import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UserLogin } from 'src/app/model/userLogin';
import { ToastrService } from 'ngx-toastr';
import { RegisterFormPopupComponent } from '../register-form-popup/register-form-popup.component';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { ProductsService } from 'src/app/services/products.service';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { ShoppingCartService } from 'src/app/services/shopping-cart.service';
import { LoginOverlayService } from 'src/app/services/login-overlay.service';

@Component({
  selector: 'app-login-dialog',
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.css']
})
export class LoginDialogComponent implements OnInit {

  form: FormGroup;
  public loginInvalid: boolean;
  // private formSubmitAttempt: boolean;
  displayRegisterPopup: boolean = false;
  // @Output() logged = new EventEmitter();

  loginModel = new UserLogin("", "", "", "", "", "","","",[],[]);
  imageNumber: number;
  chooseProfile: boolean = false;
  isLogging: boolean = false;
  numberOfUnreadMessages: number = 0;

  get isSigned(): boolean {
    return this.authService.isLoggedIn();
  }


  

  get login(): string {
    return localStorage.getItem("login");
  }

  constructor(  private fb: FormBuilder,
                private route: ActivatedRoute,
                private authService: AuthService,
                private productSvc: ProductsService,
                private router: Router,
                private loginOverlayService: LoginOverlayService,
                private shoppingCartService: ShoppingCartService,
                public ref: DynamicDialogRef,
                private userContextService: UserContextServiceService,
                private toastr: ToastrService) { }

  async ngOnInit()
  {
    // this.returnUrl = this.route.snapshot.queryParams.returnUrl || '/login';
    if(this.userContextService.Login){
      this.productSvc.GetCountedUnreadMessages().subscribe( res => {
        this.productSvc.numberOfUnreadMessaes = res;
        this.numberOfUnreadMessages = res;
      });
    }

    this.form = this.fb.group({
      username: ['', [Validators.email, Validators.required]],
      password: ['', Validators.required]
    });
  }

  onTryLogin() {
    this.isLogging = true;
    this.authService.login(this.loginModel)
        .subscribe(
            (responseData: UserLogin) => {
                localStorage.setItem('token', responseData.Token);
                localStorage.setItem("refreshToken", responseData.RefreshToken);
                localStorage.setItem("refTokExpDate", responseData.RefreshTokenExprarationDate);
                localStorage.setItem("roles", responseData.Roles.toString());
                localStorage.setItem("login", responseData.Login);
                localStorage.setItem("id", responseData.Id);
                this.userContextService.User = responseData;
                this.toastr.success("Zalogowano");
                this.loginOverlayService.closeOverlay();
                // this.logged.emit();
            },
            (error: any) => {

              this.toastr.error("Bad password or login");

                if (error.status === 401) {
                    this.isLogging = false;
                    // this.toastr.error("Bład logowania",
                    //     "Nie można zalogować się na dwóch urządzeniach jednocześnie.")
                } else {
                    this.isLogging = false;
                    console.error("error login !", error);
                    this.toastr.error("Błąd logowania", "Podano niepoprawne dane logowania.");
                }
            }
        )
  }

  resetPassword() {

    this.authService.sendEmailToResetPassword(this.loginModel.Email)
        .subscribe(
            (responseData: boolean) => {
              this.toastr.success("The email has been send");
            },
            (error: any) => {

              this.toastr.error("Bad password or login");
            }
        )
  }

  openDialogRegister() {
    this.displayRegisterPopup = true;
    // this.dialog.open(RegisterFormPopupComponent, {
    //   // position: {
    //   //   // top: '500px',
    //   //   // left: '500px'
    //   //   // top: this.cartBtn.nativeElement.offsetHeight + 10 + 'px'  ,
    //   //   // left: this.cartBtn.nativeElement.offsetLeft - 20 + 'px'
    //   // }
    // });
  }

  logout(e){
    // var roles = localStorage.getItem("roles");
    this.authService.logout();


    this.shoppingCartService.closeShoppingCartOverlay(e);
    localStorage.removeItem("roles");
  }

}
