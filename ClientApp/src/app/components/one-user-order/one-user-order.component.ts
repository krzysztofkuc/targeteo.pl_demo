import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AngularInpostGeowidgetService, GeowidgetTypeEnum } from 'angular-inpost-geowidget';
import { ToastrService } from 'ngx-toastr';
import { OneUserOrderVm } from 'src/app/model/oneUserOrderVm';
import { OrderService } from 'src/app/services/order.service';
import { CartItem, ShoppingCartService } from 'src/app/services/shopping-cart.service';
import { HttpClient} from '@angular/common/http';
import { Accordion } from 'primeng/accordion';

@Component({
  selector: 'app-one-user-order',
  templateUrl: './one-user-order.component.html',
  styleUrls: ['./one-user-order.component.css']
})
export class OneUserOrderComponent implements OnInit {

  form: FormGroup;
  paczkomatAdress: string;
  geowidgetType =  GeowidgetTypeEnum;
  spinnerVisible = false;
  orderCompleted: boolean =  false;

  @Input() model: OneUserOrderVm;
  @ViewChild('accordionPersonalData') accordionPersonalData: Accordion;

  get totalSum(): number {
    if(this.model.SelectedSupplyMethod?.PricePerUnit){
      return this.shoppingCartService.totalSum + this.model.SelectedSupplyMethod?.PricePerUnit;
    }

    return this.shoppingCartService.totalSum;
  }
  
  constructor(private fb: FormBuilder,
              public angularInpostGeowidgetService: AngularInpostGeowidgetService,
              private shoppingCartService: ShoppingCartService) { }

  ngOnInit(): void {

    this.form = this.fb.group({
      firstName: new FormControl('', Validators.required),
      surname: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      region: ['', Validators.required],
      zipCode: ['', Validators.required],
      email: ['', Validators.required],
      phoneNo: ['', Validators.required],
      supplyMethod: ['', Validators.required]
    });
  }

  clearItem(item: CartItem){
    this.shoppingCartService.clearCartItem(item);
  }

  selectPaczkomat(event){
    this.paczkomatAdress = event.name + " " + event.address.line1;
    this.model.PaczkomatDestinationAddress = this.paczkomatAdress;
  }

}
