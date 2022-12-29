import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {

  constructor(private route: ActivatedRoute, private authService: AuthService, private toast: ToastrService) { }

  ngOnInit(): void {

    // var token = this.route.snapshot.paramMap.get('token');

    this.route.queryParams.subscribe(params => {
      let token = params['token'];
      
      //tutaj użyć switchmap
      this.authService.confirmEmail(token)
      .subscribe(
          (responseData: any) => {
              this.toast.success("Email został potwierdzony, możesz się zalogować.")
              // localStorage.setItem('token', responseData.token);
              // localStorage.setItem("refreshToken", responseData.refreshToken);
              // localStorage.setItem("refTokExpDate", responseData.refreshTokenExprarationDate);
          },
          (error: any) => {
            this.toast.error("Email nie został potwierdzony.");
          }
      );
    });
  }
}
