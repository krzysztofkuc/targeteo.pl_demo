import { Component, OnInit, Inject, Input } from '@angular/core';
import { AddAttributeToProduct } from 'src/app/model/addAttributeToProduct';
import { ToastrService } from 'ngx-toastr';
import { ProductsService } from 'src/app/services/products.service';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ViewMode, AllAttributeTypes, AttributeTypesForNewProduct, ViewTextFilterTypes } from 'src/app/model/enums';
import { FormControl, FormBuilder, Validators, FormGroup } from '@angular/forms';
import {map, startWith} from 'rxjs/operators';
import { DatePipe } from '@angular/common';
import { ProductAttributeVm } from 'src/app/model/productAttributeVm';
import { CategoryAttributeVm } from 'src/app/model/categoryAttributeVm';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { SelectItem, TreeNode } from 'primeng/api';
import { ProductVm } from 'src/app/model/productVm';
import { CategoryVm } from 'src/app/model/categoryVm';

@Component({
  selector: 'app-add-attribute-to-product',
  templateUrl: './add-attribute-to-product.component.html',
  styleUrls: ['./add-attribute-to-product.component.css']
})

export class AddAttributeToProductComponent implements OnInit {
  form: FormGroup;
  model: AddAttributeToProduct;
  viewMode: ViewMode;
  mode = ViewMode;
  at = AttributeTypesForNewProduct;
  attributeName = new FormControl();
  filteredOptions: string[];
  options: string[] = [];
  attrTypes: SelectItem[];

  //View filter
  filterTypes = ViewTextFilterTypes;
  filterViewTypes = [];
  selectedViewFilterType: string;

  selectedDate: Date;
  selectedText: string;
  selectedNumber: number;
  selectedBool: boolean;

  selectedTypeCtrl: SelectItem;
  name: string;
  data: any;
  selectedCatCtrl: TreeNode;
  files1: TreeNode[];

  constructor(private router: Router,
              private productService: ProductsService,
              private toastr: ToastrService,
              private fb: FormBuilder,
              private datepipe: DatePipe,
              public config: DynamicDialogConfig) {}

  ngOnInit(): void {

    this.form = this.fb.group({
      attributeName: ['', Validators.required],
      attributeType: ['', Validators.required],
      text: [''],
      liczba: [''],
      data: [''],
      bit: [''],
    });

    this.data = this.config.data;
    this.setViewMode();

    if(this.viewMode == this.mode.Add)
    {
      this.productService.AddAttributeGet().subscribe( res => {
        this.model = res as AddAttributeToProduct;
        this.options = this.model.AllAttributes.map(x => x.Name);
        this.files1 = this.model.AllCategoriesTreeNode;
        // this.model.CurrentProduct = this.data.product;
      });
    }else{

      var attrId = this.data.attribute.ProductAttributeId;
      var prodId = this.data.product.ProductId;

      this.productService.EditAttributeGet(prodId, attrId).subscribe( res => {
        this.model = res as AddAttributeToProduct;
        this.options = this.model.AllAttributes.map(x => x.Name);
        this.model.CurrentProduct = this.data.product;
        this.selectedTypeCtrl = { label: this.model.AttributeType, value: this.model.AttributeType} ;
        this.name = this.model.Name;

        switch(this.selectedTypeCtrl.value){
          case this.at.date: 
            this.selectedDate = new Date(this.model.Value);
            break;
          case this.at.text: 
            this.selectedText = this.model.Value;
            break;
          case this.at.number: 
            this.selectedNumber = +this.model.Value;
            break;
          case this.at.bit: 
            this.selectedBool = !!this.model.Value;
            break;
        }
        this.files1 = this.model.AllCategoriesTreeNode;
        
      });
    }
  }

  nameChanged(event) : void{
    // this.model.Name = event.target.value;

    this.filteredOptions = this._filter(event.query);
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.options.filter(option => option.toLowerCase().includes(filterValue));
  }

  setViewMode(){
    if(this.data.isEditMode)
    {
      this.viewMode = this.mode.Edit;
    }else{
      this.viewMode = this.mode.Add;
    }

      //  this.attrTypes = this.at;

       this.attrTypes = Object.keys(this.at).map(key => {
        return {
            label: key,
            value: this.at[key]
        };
    });
  }

  selectedType(event) {

    this.model.AttributeType = event.value.value;

    //this is to refactor// quick fix
    if(this.model.AttributeType == "number" || this.model.AttributeType == "date"){
      this.model.ViewFilterType = "text"  
    }else if(this.model.AttributeType == "text"){
      this.model.ViewFilterType = "list"  
    }
    else if(this.model.AttributeType == "bit"){
      this.model.ViewFilterType = "list"  
    }

    // this.selectedTypeCtrl = event.value;
  }

  // onNodeSelect(event){
  //   let categoryId = event.node.data;
  //   this.model.CategoryId = +this.selectedCatCtrl.data;
  //   // this.attrs = this.addProductForm.get('attrs') as FormArray;
  //   let group={};
  
  //   this.productService.GetAllCategoryAttributes(categoryId).subscribe(res => {
  
  //     res.forEach( item => {
        
  //       group[item.Name] = new FormControl(item.Value || '');
  //     });
      
  //     // var fg = new FormGroup(group);
  //     // this.nestedFormGroup = fg;
  //     // this.addProductForm.addControl('nestedFormGroup', fg);
  //     // this.addProductForm.setControl("nestedFormGroup", fg);
  
  //     // this.attrsToAdd = res;
  
  //     this.toastr.success("mam atrybuty katgorii");
      
  //   },(error) => {
  //     this.toastr.error(error.error);
  //   }
    
  //   );
  // }

//   selectedViewFitlerType(event) {
// debugger;
//     this.selectedTypeCtrl = event.value.value;
//     this.model.ViewFilterType = event.value;
//   }

  onSubmit() {

    this.model.CurrentProduct = new ProductVm();
    this.model.CurrentProduct.Title = "dummyProduct";
    this.model.CurrentProduct.Category = new CategoryVm();
    this.model.CurrentProduct.Category.CategoryId = +this.selectedCatCtrl.data;

    this.model.CurrentProductAttribute = new ProductAttributeVm();
    this.model.CurrentProductAttribute.CategoryAttribute = new CategoryAttributeVm();

    // if(this.selectedTypeCtrl.value == this.at.date)
    // {
    //   var date = this.datepipe.transform(this.selectedDate,'yyyy-MM-dd' );
    //     this.model.Value = date;
    // }else if(this.selectedTypeCtrl.value == this.at.number)
    // {
    //     this.model.Value = this.selectedNumber.toString();
    // }
    
    switch(this.selectedTypeCtrl.value){
      case this.at.date: 
        var date = this.datepipe.transform(this.selectedDate,'yyyy-MM-dd' );
        this.model.Value = date;
        // this.selectedDate = new Date(this.model.Value);
        break;
      case this.at.text: 
        this.selectedText = this.model.Value;
        break;
      case this.at.number: 
        this.selectedNumber = +this.model.Value;
        break;
    }

    this.model.CurrentProductAttribute.CategoryAttribute.AttributeType = this.model.AttributeType;
    // this.model.CurrentProduct = this.data.product;
    this.model.AttributeType = this.selectedTypeCtrl.value;
    // this.model.CurrentProductAttribute.CategoryAttribute.ViewFilterType = this.model.ViewFitlerType;

    //habve to null this because is circular dependancy on serialize
    this.model.AllCategoriesTreeNode = null;
    this.productService.SaveAttribute(this.model).subscribe( addedAttribute =>{
      this.model.CurrentProduct.Attributes.push(addedAttribute);
      this.toastr.success("Zapisano atrybut");
      },
      error => {
        if(error.status == 500){
          this.toastr.error(error.error);
        }
        else{
          this.toastr.error("Błąd zapisu"); 
        }
      }
    );
  }

}
