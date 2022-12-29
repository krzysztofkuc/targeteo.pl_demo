import { IUserAccountSummary } from './../../../model/userAccountSummaryVm';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { DialogService } from 'primeng/dynamicdialog';
import { UserProfileService } from '../../../services/user-profile.service';
import { WithdrawMoneyComponent } from '../../withdraw-money/withdraw-money.component';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { IUserAccountVm } from 'src/app/model/userAccountVm';
import { AccountOperationStatus } from 'src/app/model/enums';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.css']
})

export class UserAccountComponent implements OnInit {

  model: IUserAccountSummary;
  isSiteLoaded: boolean = false;



  constructor(private userProfileService: UserProfileService,
              private toastr: ToastrService,
              private userContextService: UserContextServiceService,
              private dialogService: DialogService) { }

  ngOnInit(): void {
    this.userProfileService.GetUserAccount().subscribe( res => {
      this.model = res;
      this.isSiteLoaded = true;
    },
    error => {
      this.isSiteLoaded = true;
      this.toastr.error("Nie udało się załadować danych");
    }
    );
  }

  showWithdrawMoneyDialog(){

    const ref = this.dialogService.open(WithdrawMoneyComponent, {
      data: {
            model: this.model
            // isEditMode: true,
            // product: this.model.CurrentProduct,
            // attribute: attribute
          },
      header: 'Wypłać środki'
    });
  }

  canConfirmWithdraw(userAccount: IUserAccountVm) : boolean{
    return this.userContextService.IsAdmin && userAccount?.StatusId == AccountOperationStatus.WthdrawPending;
  }

  confirmWithdrawByAdmin(userAccount: IUserAccountVm){
    this.userProfileService.confirmWithdrawByAdmin(userAccount).subscribe(res => {
      // var modelRow = this.model.UserAccounts.find(x => x.Id == res.Id);
      var index = this.model.UserAccounts.findIndex(x => x.Id == res.Id);

      this.model.UserAccounts[index]= res;

      // this.model.UserAccounts.

      // modelRow.Status = res.Status;
      // modelRow.StatusId = res.StatusId;
    });
  }

}
