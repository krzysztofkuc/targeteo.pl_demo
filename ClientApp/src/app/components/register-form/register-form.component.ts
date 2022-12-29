import { Component, OnInit } from '@angular/core';
import { UserLogin } from 'src/app/model/userLogin';
import { AuthService } from 'src/app/services/auth.service';
import { ToastrService, Toast } from 'ngx-toastr';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.css']
})
export class RegisterFormComponent implements OnInit {

  loginModel = new UserLogin("", "", "", "", "", "","","",[],[]);
  form: FormGroup;
  isRegistering: boolean = false;

  constructor(private authService: AuthService, private fb: FormBuilder, private toast: ToastrService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', Validators.required],
      email: ['', Validators.email],
      password: ['', Validators.required]
    });
  }

  register() {
    this.isRegistering = true;
    this.authService.register(this.loginModel)
        .subscribe(
            (responseData: any) => {
              if(responseData == "resendConfirmEmail")
              {
                this.toast.success("Registration email is sent. Please click link in email msg.");
                
              }else if(responseData == "userIsAlreadyRegitered")
              {
                this.toast.success("You can log in");
              }
              else{
                this.toast.success("User has been registered :)");
              }
                
                // localStorage.setItem('token', responseData.token);
                // localStorage.setItem("refreshToken", responseData.refreshToken);
                // localStorage.setItem("refTokExpDate", responseData.refreshTokenExprarationDate);
            },
            (error: any) => {
              this.toast.error("Cannot register.");
            }
        ).add(() => {
          this.isRegistering = false;
     });
}

}
