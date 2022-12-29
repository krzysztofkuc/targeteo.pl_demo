import { Component, OnInit, Input, SimpleChanges, OnDestroy, Output, EventEmitter } from '@angular/core';
import { CategoryAttributeVm } from 'src/app/model/categoryAttributeVm';
import { AllFilterTypes } from 'src/app/model/enums';
import { FormBuilder, FormGroup, FormArrayName, FormControl } from '@angular/forms';
import { ActivatedRoute, NavigationExtras, Params, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MainModelService } from 'src/app/services/main-model.service';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-main-filters',
  templateUrl: './main-filters.component.html',
  styleUrls: ['./main-filters.component.css']
})
export class MainFiltersComponent implements OnInit {

  @Input() model: CategoryAttributeVm[];
  attributeTypes = AllFilterTypes;
  formGroup: FormGroup;
  attrs: FormArrayName;
  CategoryTitle: string;
  displayDialoEditFilter: boolean = false;
  
  @Output() onCloseEvent = new EventEmitter<boolean>();

  //this is redundant should be in service
  isMobile:boolean = window.screen.width < 600;
  
  //index, value
  // numberToArray = new Map<number, number>();

  constructor(private fb: FormBuilder,
              private _router: Router,
              private dialogRef: DynamicDialogRef,
              private _route: ActivatedRoute,
              private datepipe: DatePipe,
              private modelService: MainModelService) { }

  ngOnInit(): void {

    //have to wait until model load data because filters in modal initiate before model loads
    this.modelService?.isReady$.subscribe( ok => {

      if(ok){
        this.model = this.modelService?.data?.CurrentAttributes;
        this.updateModel();
        this.CategoryTitle = this.modelService?.data?.CurrentCategory?.Name;
      }
    });

    // this.form = this.fb.group({

    //   attributes: this.fb.array(this.model),
    //   numberFrom: [''],
    //   numberTo: [''],
    //   dateFrom: [''],
    //   dateTo: ['']
    // });

    // let group={}    
    // this.formGroup = new FormGroup(group);    
  }

  updateModel(){
    let group={}

    //this mess have to be clean, what is going on here
    for(var i=0; i<this.model?.length; i++){

      let attr = this.model[i];
      if(attr.AttributeType.endsWith("From")){
        if(attr.AttributeType == "dateFrom"){
          group[attr.Name + "From"]=new FormControl();

          if(this.model[i+1].dateTo){
            attr.dateRange = [new Date(attr.dateRange[0]), new Date(this.model[i+1].dateRange[0])];
            if(attr.dateRange){
              group[attr.Name + "From"]=new FormControl([attr.dateRange[0], attr.dateRange[1]]);
            }else{
              group[attr.Name + "From"]=new FormControl();
            }
          }else if(attr.dateFrom){
            // attr.dateRange = [new Date(attr.dateFrom)];
            // group[attr.Name + "From"]=new FormControl([new Date(attr.dateFrom)]);
          }
        }else{
          group[attr.Name + "From"]=new FormControl(attr.Name +"From");
        }

        
      }else if(attr.AttributeType.endsWith("To")){

        if(attr.AttributeType == "dateTo"){
          group[attr.Name + "To"]=new FormControl(new Date(attr.dateTo));
        }else{
          group[attr.Name + "To"]=new FormControl(attr.Value);
        }

      }else{
        group[attr.Name]=new FormControl({value: attr.Value, disabled: attr.Hide} ); 
      }

      
    };
    this.formGroup = new FormGroup(group);
  }

  
  //this is wrong approach
  // ngOnChanges(changes: SimpleChanges) {
  //   if (changes['model'] && this.model) {
  //     this.updateModel();
  //   } 
//}

  editFilter(filter: CategoryAttributeVm){
    this.displayDialoEditFilter = true;
    // this.dialog.open(EditFilterDialogComponent, {
    //   data: {
    //     filter: filter
    //   }
    //   // position: {
    //   //   // top: '500px',
    //   //   // left: '500px'
    //   //   // top: this.cartBtn.nativeElement.offsetHeight + 10 + 'px'  ,
    //   //   // left: this.cartBtn.nativeElement.offsetLeft - 20 + 'px'
    //   // }
    // });
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

  // selectChange(filter, event){
  //   filter.Value = event.value;
  // }

  searchProductsByFilters(){
    let queryParams: Params = {};

    
    this.model.forEach( attr => {
      if(attr.Value || attr.dateFrom || attr.dateRange){
        var urlAttrName = attr.Name;
        if(attr.AttributeType.endsWith("To")){
          urlAttrName = urlAttrName + "_To";
        }else if (attr.AttributeType.endsWith("From")){
          urlAttrName = urlAttrName + "_From";
        }
        
        if(attr.Value){
          queryParams[urlAttrName] = attr.Value;
        }else if(attr.dateRange[0] && attr.AttributeType == "dateFrom"){

          var fromDate = new Date(attr.dateRange[0]).toLocaleDateString();
          attr.dateFrom = fromDate;
          queryParams[urlAttrName] = fromDate;

          if(attr.dateRange[1]){
            let dateTo = new Date(attr.dateRange[1]).toLocaleDateString();
            queryParams[attr.Name + "_To"] =  dateTo;
            attr.dateTo = dateTo;
          }
        }
      }else if(attr.SelectedValues){

        var array = [];
        attr.SelectedValues.forEach( element => {
          array.push(element.Value)
        });

        queryParams[attr.Name] = array;
      }
      else if(attr.AttributeType == this.attributeTypes.bit && attr.Bit){

        queryParams[attr.Name]=true;
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
    url = url.split('/filtrowanie')[0];
    url = url.replace("/filtrowanie", "");

    var urlParent = url.split("/")[2];
    var urlchild = url.split("/")[3];

    let navigationExtras: NavigationExtras = {
      queryParamsHandling: 'merge',
      preserveFragment: false,
      skipLocationChange: false
    };
    
    // this._router.navigateByUrl(url + "/filtrowanie" + result, navigationExtras);

    // if(!urlchild){
    //   this._router.navigateByUrl(
    //     this._router.createUrlTree(
    //       ["kategoria", urlParent,"filtrowanie"],  {queryParams: queryParams},
    //     ),
    //     navigationExtras
    //   );
    // }else{
    //   this._router.navigateByUrl(
    //     this._router.createUrlTree(
    //       ["kategoria", urlParent, urlchild, "filtrowanie"], {queryParams: queryParams}
    //     ),
    //     navigationExtras
    //   );
    // }

    this._router.navigate(
      [],
      { queryParams: queryParams }
    );

    this.onCloseEvent.emit(true);
    // this._router.navigate([], {
    //   relativeTo: this._route,
    //   queryParams: queryParams,
    //   queryParamsHandling: 'merge',
    //   preserveFragment: false,
    //   // preserve the existing query params in the route
    //   skipLocationChange: false
    //   // do not trigger navigation
    // });
    

    //  this._router.navigate([url, 'filtrowanie'], {
    //   relativeTo: this._route,
    //   queryParams: queryParams,
    //   queryParamsHandling: 'preserve',
    //   skipLocationChange: false
    // });

      // this._router.navigate([url, 'filtrowanie?', queryParams], {});
  }
}
