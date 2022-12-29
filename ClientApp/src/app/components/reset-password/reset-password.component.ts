import { Component, OnInit } from '@angular/core';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  
  form: FormGroup;
  password: string;
  passwordRepeted: string;
  resetFormInvalid: boolean;
  canResetPassword: boolean;
  token: string;

  constructor(private fb: FormBuilder, 
              private authService: AuthService, 
              private route: ActivatedRoute,
              private toastService: ToastrService) { }

  ngOnInit(): void {
    this.canResetPassword = false;

    this.form = this.fb.group({
      password: ['', Validators.required],
      passwordRepeated: ['', Validators.required]
    });

    let token = this.route.snapshot.queryParams["token"];
    this.authService.canResetPassword(token)
    .subscribe(
        (responseData: boolean) => {
          this.canResetPassword = responseData;
          this.token = token;
        },
        (error: any) => {
          this.canResetPassword = false;
        }
    )
  }

  resetPassword(){
    this.authService.resetPassword(this.password, this.token)
    .subscribe(
        (responseData: boolean) => {
          this.toastService.success("The password has been reset");
        },
        (error: any) => {
          this.toastService.error("Cannot reset password");
        }
    )
  }

  ngOnViewChecked(){
    this.resetFormInvalid = this.password != this.passwordRepeted;
  }
}
