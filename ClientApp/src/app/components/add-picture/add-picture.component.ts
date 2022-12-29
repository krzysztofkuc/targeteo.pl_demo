import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { HttpRequest, HttpEventType, HttpClient } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-add-picture',
  templateUrl: './add-picture.component.html',
  styleUrls: ['./add-picture.component.css']
})
export class AddPictureComponent implements OnInit {

  progress: any;
  public deletedUrl: string;
  @Input() url: any;
  @Output() addPicture:EventEmitter<File> = new EventEmitter<File>();
  @Output() deletePicture:EventEmitter<string> = new EventEmitter<string>();
  showIcon: boolean = true;

  constructor() { }

  ngOnInit() {
  }

  onFileChanged(event, file) {

    // this.selectedFiles.push(file);
    var fileX:File = event.target.files[0];
    var reader:FileReader = new FileReader();

    this.addPicture.emit(fileX);

    if (fileX) {
      reader.readAsDataURL(fileX);
    }

    reader.onload = () => {
      this.url = reader.result;
      this.showIcon = false;
      //this.addProduct.emit();
    };
  }

  clearPicture(){
    this.deletePicture.emit(this.url);
    this.url ="";
    this.showIcon = true;
  }
}
