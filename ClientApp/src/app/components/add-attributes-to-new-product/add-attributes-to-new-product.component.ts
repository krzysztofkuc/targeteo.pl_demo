import { Component, OnInit, Input, SimpleChanges, OnChanges, ChangeDetectorRef } from '@angular/core';
import { CategoryAttributeVm } from 'src/app/model/categoryAttributeVm';
import { AllAttributeTypes, AllFilterTypes, AttributeTypesForNewProduct, ViewTextFilterTypes } from 'src/app/model/enums';
import { FormBuilder, FormGroup, FormArrayName, FormControl } from '@angular/forms';
import { NavigationExtras, Params, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MainModelService } from 'src/app/services/main-model.service';
import { ProductAttributeVm } from 'src/app/model/productAttributeVm';

@Component({
  selector: 'app-add-attributes-to-new-product',
  templateUrl: './add-attributes-to-new-product.component.html',
  styleUrls: ['./add-attributes-to-new-product.component.css']
})
export class AddAttributesToNewProductComponent implements OnInit {

  @Input() model: CategoryAttributeVm[];
  attributeTypes = AllFilterTypes;
  @Input() formGroup: FormGroup;
  @Input() productAttributes: ProductAttributeVm[];
  attrs: FormArrayName;
  CategoryTitle: string;
  displayDialoEditFilter: boolean = false;
  test: boolean = true;

  //this is redundant should be in service
  isMobile:boolean = window.screen.width < 600;

  constructor(private fb: FormBuilder,private ref: ChangeDetectorRef,
    private _router: Router,
    private datepipe: DatePipe,
    private modelService: MainModelService) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(){

    //Here I should use subscribe because update model is
    this.model = this.modelService?.data?.CurrentAttributes;
    this.updateModel();
    this.CategoryTitle = this.modelService?.data?.CurrentCategory?.Name;
  }

  //only when edit
  fillAttributes() {

    this.productAttributes.forEach(attr => {
      switch(attr.CategoryAttribute.AttributeType){
        case "dateFrom":
        case "date":
          this.model.filter(x => x.Name ==attr.CategoryAttribute.Name)[0].dateFromDate = new Date(attr.Value);
          this.model.filter(x => x.Name ==attr.CategoryAttribute.Name)[0].dateFrom = attr.Value;
          this.formGroup.get(attr.CategoryAttribute.Name).setValue(new Date(attr.Value).toLocaleDateString());
          break;
        case "bit":
          this.model.filter(x => x.Name ==attr.CategoryAttribute.Name)[0].Bit = Boolean(JSON.parse(attr.Value));
          this.formGroup.get(attr.CategoryAttribute.Name).setValue(Boolean(JSON.parse(attr.Value)));
          // this.formGroup.get(attr.CategoryAttribute.Name).setValue(!!attr.Value);
          break;
          case "text":
            if(attr.CategoryAttribute.ViewFilterType == 'list'){
              var filter = this.model.filter(x => x.Name == attr.CategoryAttribute.Name)[0];
              var comboValue = filter.ComboboxValues.filter(x => x.Value == attr.Value)[0];
              attr.Value = comboValue.Value;
              attr.CategoryAttribute.Value = comboValue.Value;
              this.model.filter(x => x.Name ==attr.CategoryAttribute.Name)[0].SelectedValue = comboValue;
              this.formGroup.get(attr.CategoryAttribute.Name).setValue(attr.Value);
            }else{
              this.formGroup.get(attr.CategoryAttribute.Name).setValue(attr.Value);

            if(!attr.CategoryAttribute.Value){
              this.model.filter(x => x.Name ==attr.CategoryAttribute.Name)[0].Value = attr.Value;
            }
            
            }
            break;
        default:

            this.formGroup.get(attr.CategoryAttribute.Name).setValue(attr.Value);

            if(!attr.CategoryAttribute.Value){
              this.model.filter(x => x.Name ==attr.CategoryAttribute.Name)[0].Value = attr.Value;
            }
            break;
      }
      // this.formGroup[attr.CategoryAttribute.Name].value = "xxxx";
    });
  }

onDropDownChange(event, filter){
  var value = event.value.Value;
  filter.Value = value;

}
  
  updateModel(){
    // let variableChange = changes['model'];

    let group={}

    for(var i=0; i<this.model?.length; i++){

      let attr = this.model[i];

      if(attr.AttributeType.endsWith("From")){

        if(attr.AttributeType == "dateFrom"){

          this.formGroup.addControl(attr.Name, new FormControl(new Date(attr.Value)));
          // if(attr.dateFrom){
          //   // attr.dateFrom = [new Date(attr.dateFrom[0])];
          //   this.formGroup.addControl(attr.Name, new FormControl());
          // }

        }else if (attr.AttributeType == AttributeTypesForNewProduct.bit){
          this.formGroup.addControl(attr.Name, new FormControl(Boolean(JSON.parse(attr.Value))));
          // group[attr.Name + "From"]=new FormControl();
        }else{
          this.formGroup.addControl(attr.Name, new FormControl(attr.Value));
          // group[attr.Name + "From"]=new FormControl();
        }

        
      }
      // else if(attr.AttributeType.endsWith("To")){

      //   if(attr.AttributeType == "dateTo"){
      //     this.formGroup.addControl(attr.Name + "To",new FormControl(new Date(attr.dateTo)));
      //   }else{
      //     this.formGroup.addControl(attr.Name + "To",new FormControl(attr.Value));
      //   }

      // }
      else{

        if (attr.AttributeType == AttributeTypesForNewProduct.text && attr.ViewFilterType == ViewTextFilterTypes.list){
          this.formGroup.addControl(attr.Name, new FormControl({value: attr.ComboboxValues[0].Value, disabled: attr.Hide})); 
        }else{
          this.formGroup.addControl(attr.Name, new FormControl({value: attr.Value, disabled: attr.Hide})); 
        }
      }
    };

    if(this.model){
      this.fillAttributes();
    }

  }

  
  ngOnChanges(changes: SimpleChanges) {
    if (changes['model'] && this.model) {
      this.updateModel();
  }
}

  editFilter(filter: CategoryAttributeVm){
    this.displayDialoEditFilter = true;
  }

  numberToChange($event, i){
    var v = $event.value;
    
    // this.numberToArray.set(i, v);
    this.model[i + 1].Value = v;
  }

  dateChange(filter, event){
    
    // var value = this.formGroup.controls[filter.Name].value;
    var date = this.datepipe.transform(event.value,'yyyy-MM-dd' );
    filter.Value = date;
  }

  boolChange(filter, $event){
    
    // var value = this.formGroup.controls[filter.Name].value;
    var v = !!$event.value;
    filter.Value = v;
  }

  // selectChange(filter, event){
  //   filter.Value = event.value;
  // }

  // flash(){
  //   setTimeout(function() {
  //     (document.querySelector('.attrs') as HTMLElement).style.backgroundColor = 'red';
  //  }, 500);
  // }

  searchProductsByFilters(){
    let queryParams: Params = {};

    
    this.model.forEach( attr => {
      if(attr.Value || attr.dateFrom){
        var urlAttrName = attr.Name;
        if(attr.AttributeType.endsWith("To")){
          urlAttrName = urlAttrName + "_To";
        }else if (attr.AttributeType.endsWith("From")){
          urlAttrName = urlAttrName + "_From";
        }
        
        if(attr.Value){
          queryParams[urlAttrName] = attr.Value;
        }else if(attr.dateFrom[0]){

          var fromDate = attr.dateFrom;
          queryParams[urlAttrName] = fromDate;

          if(attr.dateFrom[1]){
            queryParams[attr.Name + "_To"] = attr.dateFrom;
          }
        }
      }else if(attr.SelectedValues){

        var array = [];
        attr.SelectedValues.forEach( element => {
          array.push(element.Value)
        });

        queryParams[attr.Name] = array;
      }
    });


    //build Url
    var result = "?";

    for (const [key, value] of Object.entries(queryParams)) {

      if(Array.isArray(value))
      {
        value.forEach(e => {
          result += key + "=" + e + "&";
        });

        continue;
      }
      result += key + "=" + value;
      result += "&";
    }
    result = result.slice(0, -1);

    let url = this._router.routerState.snapshot.url;
    url = url.split('/filtrowanie')[0]
    url = url.replace("/filtrowanie", "");

    var urlParent = url.split("/")[2];
    var urlchild = url.split("/")[3];

    let navigationExtras: NavigationExtras = {
      queryParamsHandling: 'merge',
      preserveFragment: false,
      skipLocationChange: false
    };
    
    // this._router.navigateByUrl(url + "/filtrowanie" + result, navigationExtras);

    if(!urlchild){
      this._router.navigateByUrl(
        this._router.createUrlTree(
          ["kategoria", urlParent,"filtrowanie"], {queryParams: queryParams}
        )
      );
    }else{
      this._router.navigateByUrl(
        this._router.createUrlTree(
          ["kategoria", urlParent, urlchild, "filtrowanie"], {queryParams: queryParams}
        )
      );
    }
  }

}
