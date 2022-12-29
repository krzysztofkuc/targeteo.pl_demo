import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
// import { MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewTextFilterTypes } from 'src/app/model/enums';
import { ProductsService } from 'src/app/services/products.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-filter-dialog',
  templateUrl: './edit-filter-dialog.component.html',
  styleUrls: ['./edit-filter-dialog.component.css']
})
export class EditFilterDialogComponent implements OnInit {
  form: FormGroup;
  selectedTypeCtrl: string;
  types = ViewTextFilterTypes;
  ctrAttrTypes = [];
  hide = false;

  constructor(
              private fb: FormBuilder,
              private productService: ProductsService,
              private toastr: ToastrService,
              private changeDetectorRef: ChangeDetectorRef) { }

  ngOnInit(): void {

    this.form = this.fb.group({
      filterType: [''],
      data: [''],
      attributeType: [''],
      hide: ['']
    });

    //not compatible przez meterialsy
    // this.selectedTypeCtrl = this.data.filter.ViewFilterType;

    // this.ctrAttrTypes = Object.keys(this.types);
    // this.hide = this.data.filter.Hide;
  }

  selectedType(event) {
    let target = event.source.selected._element.nativeElement;

    // let selectedData = {
    //   value: event.value,
    //   text: target.innerText.trim()
    // };

    // this.model.AttributeType = event.value;
  }

  onSubmit() {

    this.changeDetectorRef.detectChanges();

    //niekompatybilne rpzez materialsy
    // this.productService.SaveViewFilter(this.data.filter.PKAttributeId ,this.selectedTypeCtrl, this.hide).subscribe( res =>{
    //   var res = res;

    //   this.toastr.success("Zmieniono rodzaj filtra");
    // },
    // error => {
    //   this.toastr.error("Error");
    // });

  }

}
