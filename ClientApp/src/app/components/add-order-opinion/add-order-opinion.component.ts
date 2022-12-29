import { HttpClient, HttpRequest } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { OrderOpinionVm } from 'src/app/model/orderOpinionVm';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-add-order-opinion',
  templateUrl: './add-order-opinion.component.html',
  styleUrls: ['./add-order-opinion.component.css']
})

export class AddOrderOpinionComponent implements OnInit {
  selectedFiles: File[] = [];
  text: string;
  ratingVal: number = 1;
  model: OrderOpinionVm;
  uploadedFiles: any[] = [];

  constructor(private toastr: ToastrService,
              private route: ActivatedRoute,
              private _http: HttpClient,
              private productsvc: ProductsService) { }

  ngOnInit(): void {

    let orderDetailId: number = Number(this.route.snapshot.paramMap.get('orderDetailId'));

    this.productsvc.AddOrderOpinion(orderDetailId).subscribe(res => {
      this.model = res;
    },
    error => {
      this.toastr.error("error");
    });

  }

  addPicture(file: File){

    this.resizeImage(file, 700, 700).then(blob => {
      //You can upload the resized image: doUpload(blob)
      // blob.name = file.name;
      var f = new File([blob], file.name);
      this.selectedFiles.push(f);
      this.toastr.success("Zmniejszono zdjęcie");

      // document.getElementById('img').src = URL.createObjectURL(blob);
    }, err => {
      console.error("Photo error", err);
    });

  }

  //the same in add product
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

onUpload(event) {
  for(let file of event.files) {
      this.uploadedFiles.push(file);
  }

  // this.messageService.add({severity: 'info', summary: 'File Uploaded', detail: ''});
}

save(){

  // this.productsvc.AddOrderOpinionPost(this.model).subscribe(res => {
  //   // this.announcements = res;
  //   //this.model = res;
  //   this.toastr.success("Dodano opinię");
  // },
  // error => {
  //   this.toastr.error("error");
  // });

  //===============================================================================
  const formData = new FormData();
  let model = new Object();

  //this.model.CurrentProduct.Attributes = this.ConvertCategoryAttrsToProductAttrs(productData.nestedFormGroup);



  //how to make deep copy
  //const copy = { ...original }
  model = { ...this.model };
  //copiedObject.AllCategoriesTreeNode = null;
  //copiedObject.CurrentProduct.Category.CategoryId = +this.selectedCatCtrl.data;

  formData.append("model", JSON.stringify(model));

  for (let file of this.uploadedFiles)
    formData.append(file.name, file);
    //formData.append("mode", this.viewMode)

    const uploadReq = new HttpRequest('POST', 'api/LoggedUser/addorderopinion', formData, {
      reportProgress: false,
    });

    this._http.request(uploadReq).subscribe(event => {

      this.toastr.success("zapisano");
      
    },(error) => {
      this.toastr.error(error.error);
    });
    
  //deleted pictures
  //formData.append("DeletedPictures", JSON.stringify(this.deletedPictures));
}

}
