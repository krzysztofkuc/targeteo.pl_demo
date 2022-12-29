import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { WithdrawVm } from 'src/app/model/withdrawVm';
import { UserProfileService } from 'src/app/services/user-profile.service';

@Component({
  selector: 'app-withdraw-money',
  templateUrl: './withdraw-money.component.html',
  styleUrls: ['./withdraw-money.component.css']
})
export class WithdrawMoneyComponent implements OnInit {
  form: FormGroup;
  model: WithdrawVm = new WithdrawVm();

  constructor(private fb: FormBuilder,
    private taostr: ToastrService,
    private userProfileService: UserProfileService) { }

  ngOnInit(): void {

    this.model.SaveAccountNo = true;
    this.form = this.fb.group({
      accountNo: new FormControl('', Validators.required),
      saveAccountNo: new FormControl(true),

    });

  }

  requestForWithdraw() {
    this.userProfileService.RequestWithdraw(this.model).subscribe( res => {
      this.taostr.success("Wysłano email z prośbą o potwierdzenie wypłaty");
    },
      error => {
        this.taostr.error("Nie wysłano prośby o wypłatę.");
      },
    );
  }

}
