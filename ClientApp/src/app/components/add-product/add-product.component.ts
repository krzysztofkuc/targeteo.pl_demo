import { Component, OnInit, ChangeDetectorRef, Input, Inject, ViewChild, AfterViewInit } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';
import { HttpRequest, HttpClient, HttpEventType } from '@angular/common/http';
import { FormGroup, FormControl, FormArray, Validators, FormBuilder, ValidatorFn, AbstractControl } from '@angular/forms';
import { AddProductVm } from 'src/app/model/addProductVm';
import { ProductVm } from 'src/app/model/productVm';
import { Observable } from 'rxjs';
import { map, retry } from 'rxjs/operators';
import { CategoryVm } from 'src/app/model/categoryVm';
import { ActivatedRoute, Router } from '@angular/router';
import { ViewMode } from 'src/app/model/enums';
import { AddAttributeToProductComponent } from '../add-attribute-to-product/add-attribute-to-product.component';
import { ProductAttributeVm } from 'src/app/model/productAttributeVm';
import { ToastrService } from 'ngx-toastr';
import { TreeNode } from 'primeng/api/treenode';
import { AddAttributeToProduct } from 'src/app/model/addAttributeToProduct';
import { DialogService } from 'primeng/dynamicdialog';
import { CategoryAttributeVm } from 'src/app/model/categoryAttributeVm';

import Map from 'ol/Map';
import View from 'ol/View';

import OSM from 'ol/source/OSM';
import {transform} from 'ol/proj.js';

import 'ol/ol.css';
import Feature from 'ol/Feature';
import {Circle} from 'ol/geom';
import {Vector as VectorSource} from 'ol/source';
import {Style} from 'ol/style';
import {Tile as TileLayer, Vector as VectorLayer} from 'ol/layer';
import { MapComponent } from '../map/map.component';
import { CityVm } from 'src/app/model/cityVm';
import { AddAttributesToNewProductComponent } from '../add-attributes-to-new-product/add-attributes-to-new-product.component';
import { UserContextServiceService } from 'src/app/services/user-context-service.service';
import { Tree } from '@angular-devkit/schematics';
import { prepareEventListenerParameters } from '@angular/compiler/src/render3/view/template';
import { SelectItem } from 'primeng/api';
import { SupplyMethodVm } from 'src/app/model/supplyMethodVm';
import { AddSupplyMethodDialogComponent } from '../add-supply-method-dialog/add-supply-method-dialog.component';
// import { TransitiveCompileNgModuleMetadata } from '@angular/compiler';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css'],
  providers: [DialogService]
})
export class AddProductComponent implements OnInit {

  @ViewChild('mapCmpAddProd', {static : false}) private openStreetMap:MapComponent;
  @ViewChild('attrs', {static : false}) private attrs:AddAttributesToNewProductComponent;
  
  timeout: any = null;

  selectedFiles: File[] = [];
  deletedPictures: string[] = [];
  progress: any;
  thumbnail: any;
  urls: string[] = [];
  addProductForm: FormGroup;
  model: AddProductVm;
  mode = ViewMode;
  viewMode: ViewMode;
  selectedCatCtrl: TreeNode;
  files1: TreeNode[];
  addProdAttrVisible: boolean;
  editProdAttrVisible: boolean;
  editAttrModel: AddAttributeToProduct;
  attrsToAdd: CategoryAttributeVm[];
  // attrs: FormArray;
  nestedFormGroup: FormGroup;
  uploadedFiles: any[] = [];
  public siteLoaded: boolean = false;
  foundCategory: boolean = false;
  fillNewAttributes: boolean = false;
  
  // selectedCity: CityVm;

  //lat lon Warszawa
  // latitude: number = 52.22949061515579;
  // longitude: number = 21.01930276865225;
  // map: any;

  // vectorSource: any;
  // vectorLayer: any;
  // mapView: any;


    constructor(private productService: ProductsService, 
                private fb:FormBuilder,
                private _http: HttpClient,
                private route: ActivatedRoute,
                private cd: ChangeDetectorRef,
                private router: Router,
                private toastr: ToastrService,
                private userService: UserContextServiceService,
                public dialogService: DialogService
                ) 
    {
      this.addProductForm = new FormGroup({
        productTitle: new FormControl('',Validators.required),
        desc: new FormControl('',Validators.required),
        price: new FormControl('',Validators.required),
        quantInStock: new FormControl('',Validators.required),
        items: new FormControl(),
        pics: new FormControl(),
        dicountFromPrice: new FormControl(),
        productCity: new FormControl('',Validators.required),
        nestedFormGroup: new FormGroup({}),
     });
    }

  // let foundCategory = false;
  selectCategoryNode(categoryId: number, tree: TreeNode[]){

    if(this.foundCategory){
      return;
    }

    var found = tree?.filter(x => x.data == categoryId.toString())[0] as TreeNode;
    if(found){
      this.foundCategory = true;
      this.selectedCatCtrl = found;
      this.expandAllParents(this.selectedCatCtrl);
      // this.selectedCatCtrl.styleClass = "p-highlightTreeNode";
      // this.selectedCatCtrl = {...this.selectedCatCtrl};
      //have to create event obj because onNodeSelect is event require that obj
      let event = {node: this.selectedCatCtrl}
      this.onNodeSelect(event);
      return;
    }
    else 
    {
      tree?.forEach(node =>{
          //add parent for childrens
          node.children?.forEach(c => {
            c.parent = node;
          });
          this.selectCategoryNode(categoryId, node.children);
      });
    }
  }

  private expandAllParents(node: TreeNode) {
    node.expanded = true;

    if(node.parent){
      this.expandAllParents(node.parent)
    }
  }

    setViewMode(){
      if(this.router.url.includes("edit-product"))
      {
        this.viewMode = this.mode.Edit;
      }else{
        this.viewMode = this.mode.Add;
      }
    }

  ngAfterViewChecked() {
      this.cd.detectChanges();
    }

  ngOnInit() {
    // this.siteLoaded = false;
    // for(let i=0 ; i < 4; i++){
    //   this.urls.push("");
    // }

    this.route.queryParams.subscribe(params => {
      var id = params['id'];

      if (id) {

        this.getModel(id).subscribe(res => {

          this.model = res as AddProductVm;
          this.files1 = this.model.AllCategoriesTreeNode;
          // this.files1[0].partialSelected = true;

          //  this.selectedCatCtrl = this.files1.filter(x => x.data == this.model.CurrentProduct.Category.CategoryId)[0];
          this.foundCategory = false;
          this.selectCategoryNode(this.model.CurrentProduct.Category.CategoryId, this.files1);

          // for(let i=0 ; i < this.model.CurrentProduct.Pictures.length; i++){
          //   var path = this.model.CurrentProduct.Pictures[i].Path;
          //   this.urls[i] = "assets\\upload\\" + path;
          // }
          /*this.siteLoaded = true;*/
          this.siteLoaded = true;
          

        });
      } else {
      }
    });

      this.setViewMode();

      if(this.viewMode.includes(this.mode.Add))
      {
        this.getModel(null).subscribe( res => {
          this.model = res as AddProductVm;
          this.files1 = this.model.AllCategoriesTreeNode;
          var p = new ProductVm();
          p.City = new CityVm();
          this.model.CurrentProduct = p;
          p.Category = new CategoryVm();
          this.siteLoaded = true;
        });
      }
    }

    getModel(id): Observable<any>{
      return this.productService.AddProductGetInitModel(id).pipe(map( res => res));
    }

    selectedCategory(event) {
      let target = event.source.selected._element.nativeElement;

      let selectedData = {
        value: event.value,
        text: target.innerText.trim()
      };

      this.model.CurrentProduct.Category.CategoryId = selectedData.value;
    }

    // addPicture(file: File){

    //   this.resizeImage(file, 500, 500).then(blob => {
    //     //You can upload the resized image: doUpload(blob)
    //     // blob.name = file.name;
    //     var f = new File([blob], file.name);
    //     this.selectedFiles.push(f);
    //     this.toastr.success("Zmniejszono zdjęcie");

    //     // document.getElementById('img').src = URL.createObjectURL(blob);
    //   }, err => {
    //     console.error("Photo error", err);
    //   });

    //   this.resizeImage(file, 100, 100).then(blob => {
    //     //You can upload the resized image: doUpload(blob)
    //     // blob.name = file.name;
    //     var f = new File([blob], file.name + "_thumb");
    //     this.selectedFiles.push(f);
    //     this.toastr.success("Zmniejszono zdjęcie");

    //     // document.getElementById('img').src = URL.createObjectURL(blob);
    //   }, err => {
    //     console.error("Photo error", err);
    //   });

    // }

    //the same in add popinion
    resizeImage(file:File, maxWidth:number, maxHeight:number):Promise<Blob> {
      return new Promise((resolve, reject) => {
          let image = new Image();
          image.src = URL.createObjectURL(file);
          image.onload = () => {
              let width = image.width;
              let height = image.height;
              
              if (width <= maxWidth && height <= maxHeight) {
                  resolve(file);
              }
  
              let newWidth;
              let newHeight;
  
              if (width > height) {
                  newHeight = height * (maxWidth / width);
                  newWidth = maxWidth;
              } else {
                  newWidth = width * (maxHeight / height);
                  newHeight = maxHeight;
              }
  
              let canvas = document.createElement('canvas');
              canvas.width = newWidth;
              canvas.height = newHeight;
  
              let context = canvas.getContext('2d');
  
              context.drawImage(image, 0, 0, newWidth, newHeight);
  
              canvas.toBlob(resolve, file.type);
          };
          image.onerror = reject;
      });
  }
  

    deletePicture(pic: any){

      this.uploadedFiles = this.uploadedFiles.filter(obj => obj !== pic);
      if(!this.model.DeletedPictures){
        this.model.DeletedPictures = [];
      }

      this.model.DeletedPictures.push(pic.Path);
      this.model.CurrentProduct.Pictures = this.model.CurrentProduct.Pictures.filter(obj => obj !== pic);
    }

    addAttribute(){

      const ref = this.dialogService.open(AddAttributeToProductComponent, {
        data: {
          isEditMode: false,
          product: this.model.CurrentProduct
          },
        header: 'Dodawanie atrybutu'
      });
    }

    editAttribute(attribute: ProductAttributeVm){

      const ref = this.dialogService.open(AddAttributeToProductComponent, {
        data: {
              isEditMode: true,
              product: this.model.CurrentProduct,
              model: this.model,
              attribute: attribute
            },
        header: 'Edycja atrybutu'
      });
    }

    // showAddSupplyMethodDialog(){

    //   const ref = this.dialogService.open(AddSupplyMethodDialogComponent, {
    //     data: {
    //           model: this.model
    //           // isEditMode: true,
    //           // product: this.model.CurrentProduct,
    //           // attribute: attribute
    //         },
    //     header: 'Dodaj metodę dostawy'
    //   });
    // }

    deleteAttribute(attribute: ProductAttributeVm){
      this.productService.DeleteProductAttribute(attribute.ProductAttributeId).subscribe( res => {

        var newArray = this.model.CurrentProduct.Attributes.filter(obj => obj !== attribute);

        this.model.CurrentProduct.Attributes = newArray;

        this.toastr.success("Usunięto atrybut");
        },
        error => {
          this.toastr.error("Nie usunięto atrybutu.");
        }
      );
    }

    onSubmit(productData) {
      //validation
      if(this.model.CurrentProduct?.Pictures?.length == 0 && this.uploadedFiles.length == 0){
        this.toastr.error("Produkt musi zawierać przynajmniej jedno zdjęcie");
        return;
      }
      const formData = new FormData();
      let model = new Object();

      this.model.CurrentProduct.Attributes = this.ConvertCategoryAttrsToProductAttrs(productData.nestedFormGroup);

      //how to make deep copy
      //const copy = { ...original }
      const copiedObject = Object.assign(model, this.model);
      copiedObject.AllCategoriesTreeNode = null;
      copiedObject.CurrentProduct.Category.CategoryId = +this.selectedCatCtrl.data;

      formData.append("model", JSON.stringify(model));

      for (let file of this.uploadedFiles)
        formData.append(file.name, file);
        formData.append("mode", this.viewMode)
  
        const uploadReq = new HttpRequest('POST', 'api/LoggedUser/add-product-post', formData, {
          reportProgress: false,
        });

      //deleted pictures
      formData.append("DeletedPictures", JSON.stringify(this.deletedPictures));
  
      this._http.request(uploadReq).subscribe(event => {

        this.toastr.success("zapisano");
        
      },(error) => {
        this.toastr.error(error.error);
      });
    }

    onNodeSelect(event){
      let categoryId = event.node.data;
      // this.attrs = this.addProductForm.get('attrs') as FormArray;
      let group={};

      this.productService.GetAllCategoryAttributes(categoryId).subscribe(res => {

        this.attrsToAdd = res;
        //Clean  ?
        // this.attrs.flash();

        this.fillNewAttributes = true;
        this.toastr.success("Wypełnij nowe parametry ogłoszenia");
        
      },(error) => {
        this.toastr.error(error.error);
      });
    }

    ConvertCategoryAttrsToProductAttrs(attrs): ProductAttributeVm[] {
      // var cena = this.addProductForm.controls['Cena'].value;
      var res: ProductAttributeVm[] = new Array();
      this.attrsToAdd.forEach(item => {
        var prodAttr = new ProductAttributeVm();
        prodAttr.CategoryAttribute = item;
        switch(item.AttributeType){
          case "dateFrom":
            prodAttr.CategoryAttribute.dateFrom = new Date(item.dateFromDate).toLocaleDateString();
            prodAttr.Value = (new Date(item.dateFromDate)).toLocaleDateString();
            break;
          case "text":
            if(item.ViewFilterType == "list" && !item.SelectedValue?.Value){
              item.Value = item.ComboboxValues[0].Value;
              prodAttr.Value = item.ComboboxValues[0].Value;
            }else if(item.ViewFilterType == "list" && item.SelectedValue?.Value){
              prodAttr.Value = item.SelectedValue.Value;
            }
            break;

          default:
              prodAttr.Value = attrs[item.Name];
              break;

        }
        res.push(prodAttr);
      });

      return res;
    }

    onCityChange(event){

      clearTimeout(this.timeout);
      // var $this = this;
      this.timeout = setTimeout(() => {
        if (event.keyCode != 13) {
          this.openStreetMap.setMap(event.target.value, 12, 5);
        }
      }, 1000);
    }

    onUpload(event) {


      for(let file of event.files) {

        this.resizeImage(file, 500, 500).then(blob => {
          //You can upload the resized image: doUpload(blob)
          // blob.name = file.name;
          var f = new File([blob], file.name);
          // this.selectedFiles.push(f);
          this.uploadedFiles.push(f);
          //his.toastr.success("Zmniejszono zdjęcie");
  
          // document.getElementById('img').src = URL.createObjectURL(blob);
        }, err => {
          console.error("Błąd. Nie dodano zdjęcia.", err);
        });

        // this.resizeImage(file, 100, 100).then(blob => {
        //   //You can upload the resized image: doUpload(blob)
        //   // blob.name = file.name;
        //   var f = new File([blob], file.name + "_thumb");
        //   // this.selectedFiles.push(f);
        //   this.uploadedFiles.push(f);
        //   //this.toastr.success("Zmniejszono zdjęcie");
  
        //   // document.getElementById('img').src = URL.createObjectURL(blob);
        // }, err => {
        //   console.error("Photo error", err);
        // });
      }

      // this.messageService.add({severity: 'info', summary: 'File Uploaded', detail: ''});
    }

    // deleteSupplyMethod(sm){
    //   const index = this.model.User.UserSupplyMethods.indexOf(sm, 0);
    //   if (index > -1) {
    //     this.model.User.UserSupplyMethods.splice(index, 1);
    //   }
    // }

    // uploadFiles(files:[]) : string[]  {

    //   // this.selectedFiles.push(file);
    //   // var fileX:File = event.target.files[0];
    //   var urls = [];
    //   var reader:FileReader = new FileReader();
  
    //   if (files) {
    //     files.forEach(file => {
    //       reader.readAsDataURL(file);
    //       reader.onload = () => {
    //         urls.push(reader.result);
    //         // this.showIcon = false;
    //         //this.addProduct.emit();
    //       };
    //     }); 
    //   }

    //   return urls;
    // }



    // setMap(city, zoom){
    //   this.productService.GetCityCoordinates(city).subscribe( res =>
    //     {
    //       this.latitude = +res[0].lat;
    //       this.longitude = +res[0].lon;

    //       var newView = new View({
    //         center: transform([this.longitude, this.latitude ], 'EPSG:4326', 'EPSG:3857'),
    //         zoom: 12,
    //       });

    //             //new map
    //       const circleFeature = new Feature({
    //         geometry: new Circle(transform([this.longitude, this.latitude ], 'EPSG:4326', 'EPSG:3857'), 10000),
    //       });
          
    //       this.vectorSource = new VectorSource({
    //         features: [circleFeature],
    //       });

    //       this.vectorLayer.setSource(this.vectorSource);

    //       this.map.setView(newView);
    //     });
    // }
}

