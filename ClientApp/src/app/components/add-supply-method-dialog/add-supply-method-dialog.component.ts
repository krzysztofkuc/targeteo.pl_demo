import { Component, Inject, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AddProductVm } from 'src/app/model/addProductVm';
import { SupplyMethodVm } from 'src/app/model/supplyMethodVm';
import * as cloneDeep from 'lodash/cloneDeep';
import { UserSupplyMethodVm } from 'src/app/model/userSupplyMethodVm';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DOCUMENT } from '@angular/common';
import { userProfileVm } from 'src/app/model/userProfileVm';

@Component({
  selector: 'app-add-supply-method-dialog',
  templateUrl: './add-supply-method-dialog.component.html',
  styleUrls: ['./add-supply-method-dialog.component.css']
})
export class AddSupplyMethodDialogComponent implements OnInit {
  // modelRefrence: AddProductVm;
  form: FormGroup;
  model: userProfileVm;
  selectedSupplyMethod: SupplyMethodVm;
  productSupplyMethod: UserSupplyMethodVm;
  requiredFields: string[] = [];

  constructor(public config: DynamicDialogConfig, private ref: DynamicDialogRef, private fb: FormBuilder, @Inject(DOCUMENT) document: Document ) { }

  ngOnInit(): void {
    this.productSupplyMethod = new UserSupplyMethodVm();
    // this.modelRefrence = this.config.data.model;
    this.model = cloneDeep(this.config.data.model);;

    this.form = this.fb.group({
      price: new FormControl('', Validators.required),
      time: new FormControl('', Validators.required),
      supplyMethodCtrl: new FormControl('', Validators.required)
   });

    // this.model = { ...this.config.data.model };
  }

  // onSelectDeliveryMethod($event, selectedMethod){
  //   if(!this.model.CurrentProduct.SupplyMethods){
  //     this.model.CurrentProduct.SupplyMethods = [];
  //   }

  //   var alreadyContains = this.model.CurrentProduct.SupplyMethods.indexOf(selectedMethod) > -1;
  //   if(selectedMethod && !alreadyContains){
  //     this.model.CurrentProduct.SupplyMethods.push(selectedMethod);
  //   }
  // }

  // deleteSupplyMethod(sm){
  //   const index = this.model.CurrentProduct.SupplyMethods.indexOf(sm, 0);
  //   if (index > -1) {
  //     this.model.CurrentProduct.SupplyMethods.splice(index, 1);
  //   }
  // }

  saveSupplyMethods(){

    var methods = this.config.data.model.User.UserSupplyMethods;
    if(!methods){
      this.config.data.model.User.UserSupplyMethods = [];
    }

    this.productSupplyMethod.SupplyMethod = this.selectedSupplyMethod;
    // this.productSupplyMethod.ProductId = this.config.data.model.CurrentProduct.ProductId;
    this.productSupplyMethod.SupplyMethodId = this.selectedSupplyMethod.Id;

    this.config.data.model.User.UserSupplyMethods.push(this.productSupplyMethod);
    this.ref.close();
  }

}
