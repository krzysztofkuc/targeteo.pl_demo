import { Title } from '@angular/platform-browser';
import { Component, OnInit, Inject } from '@angular/core';
import { AddCategorytVm } from 'src/app/model/addCategoryVm';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { ViewMode } from 'src/app/model/enums';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductsService } from 'src/app/services/products.service';
import { map } from 'rxjs/operators';
import { CategoryVm } from 'src/app/model/categoryVm';
import { ToastrService } from 'ngx-toastr';
import { CategoryAttributeVm } from 'src/app/model/categoryAttributeVm';
import { TreeNode } from 'primeng/api/treenode';
import { stringify } from '@angular/compiler/src/util';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { HttpClient, HttpRequest } from '@angular/common/http';

@Component({
  selector: 'app-add-category',
  templateUrl: './add-category.component.html',
  styleUrls: ['./add-category.component.css']
})
export class AddCategoryComponent implements OnInit {
  model: AddCategorytVm;
  addCategoryForm: FormGroup;
  selectedCatCtrl: any;
  viewMode: ViewMode = ViewMode.Add;
  mode = ViewMode;
  files1: TreeNode[];
  uploadedFile: any;

  constructor(private productService: ProductsService,
              private toastr: ToastrService,
              private route: ActivatedRoute,
              public ref: DynamicDialogRef,
              public config: DynamicDialogConfig,
              private _http: HttpClient
              ) {

      this.addCategoryForm = new FormGroup({
        categoryName: new FormControl(Validators.required),
        categories: new FormControl(Validators.required),
        categoryIcon: new FormControl(Validators.required)
      });
   }

  ngOnInit(): void {
    this.setViewMode();

    this.setModel(this.config?.data?.parent, this.config?.data?.child);
  }

  setModel(parent: string, child: string){
    if(this.viewMode.includes(this.mode.Add))
    {
      //new category
      this.getModel().subscribe( res => {
        this.model = res as AddCategorytVm;

        this.files1 = this.model.AllCategoriesTree;
        this.model.Category = new CategoryVm();
      });
    }else{
      //edit category
      this.getModel(parent, child).subscribe( res => {
        //this.model = res as AddCategorytVm;

        this.files1 = (res as AddCategorytVm).AllCategoriesTree;
      });
    }
  }

  getModel(parent: string = "", child: string = ""): Observable<any>{
    return this.productService.GetAddCategoryModel(parent, child).pipe(map( res => res));
  }

  setViewMode(){
    if(this.config.data)
    {
      this.model = new AddCategorytVm();
      this.model.Category = this.config.data.category as CategoryVm;
      this.viewMode = this.mode.Edit;
    }else{
      this.viewMode = this.mode.Add;
    }
  }

  onSubmit(){

    const formData = new FormData();

    this.model.AllCategoriesTree = null;
    this.model.Category.ParentId = this.selectedCatCtrl?.data;

    if(this.viewMode == this.mode.Add)
    {

      formData.append("model", JSON.stringify(this.model.Category));
      formData.append(this.uploadedFile.name, this.uploadedFile);
      formData.append("mode", this.viewMode)

      // formData.append("DeletedPictures", JSON.stringify(this.deletedPictures));

      const uploadReq = new HttpRequest('POST', 'api/Root/Manage/editCategory', formData, {
        reportProgress: false,
      });

      this._http.request(uploadReq).subscribe(event => {

        this.toastr.success("zapisano");

      },(error) => {
        this.toastr.error(error.error.title);
      });

      // this.productService.AddCategory(this.model).subscribe(re =>{
      //   this.toastr.success("Dodano kategorię");
      //   this.setModel(null, null);
      // }),
      // (error: any) => {
      //   this.toastr.error("Kategoria nie została dodana.");
      // };
    }else{

      formData.append("model", JSON.stringify(this.model.Category));
      formData.append(this.uploadedFile.name, this.uploadedFile);
      formData.append("mode", this.viewMode)

      // formData.append("DeletedPictures", JSON.stringify(this.deletedPictures));

      const uploadReq = new HttpRequest('POST', 'api/Root/Manage/editCategory', formData, {
        reportProgress: false,
      });

      this._http.request(uploadReq).subscribe(event => {

        this.toastr.success("zapisano");

      },(error) => {
        this.toastr.error(error.error);
      });

      // this.productService.EditCategory(this.model.Category).subscribe(re =>{
      //   this.toastr.success("Edytowano kategorię");
      // }),
      // (error: any) => {
      //   this.toastr.error("Kategoria nie została dodana.");
      // };
    }

    this.ref.close();
  }

  onUpload(event) {


    for(let file of event.files) {

      //this is redundant
      this.resizeImage(file, 200, 200).then(blob => {
        //You can upload the resized image: doUpload(blob)
        // blob.name = file.name;
        var f = new File([blob], file.name);
        // this.selectedFiles.push(f);
        this.uploadedFile = f;
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

  // deletePicture(pic: any){

  //   this.uploadedFiles = this.uploadedFiles.filter(obj => obj !== pic);
  //   if(!this.model.DeletedPictures){
  //     this.model.DeletedPictures = [];
  //   }

  //   this.model.DeletedPictures.push(pic.Path);
  //   this.model.CurrentProduct.Pictures = this.model.CurrentProduct.Pictures.filter(obj => obj !== pic);
  // }

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

}
