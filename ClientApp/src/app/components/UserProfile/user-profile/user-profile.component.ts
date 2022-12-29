import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { DialogService } from 'primeng/dynamicdialog';
import { userProfileVm } from 'src/app/model/userProfileVm';
import { UserProfileService } from 'src/app/services/user-profile.service';
import { AddSupplyMethodDialogComponent } from '../../add-supply-method-dialog/add-supply-method-dialog.component';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {

  model: userProfileVm;

  constructor(private userProfileService: UserProfileService, private toastService: ToastrService, private dialogService: DialogService) { }

  ngOnInit(): void {
    this.userProfileService.GetUserProfile().subscribe( res => {
      this.model = res;
    })
  }

  showAddSupplyMethodDialog(){

    const ref = this.dialogService.open(AddSupplyMethodDialogComponent, {
      data: {
            model: this.model
            // isEditMode: true,
            // product: this.model.CurrentProduct,
            // attribute: attribute
          },
      header: 'Dodaj metodę dostawy'
    });
  }

  deleteSupplyMethod(sm){
    const index = this.model.User.UserSupplyMethods.indexOf(sm, 0);
    if (index > -1) {
      this.model.User.UserSupplyMethods.splice(index, 1);
    }
  }

  saveUserProfile() {
    this.userProfileService.SaveUserProfile(this.model).subscribe( res => {
      this.toastService.success("Zapisano profil")
    },
    error => {
      this.toastService.error("Błąd zapisu")
    }
    
    )
  }

}
