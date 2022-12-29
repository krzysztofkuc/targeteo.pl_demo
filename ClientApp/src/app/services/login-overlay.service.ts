import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class LoginOverlayService {
  show:boolean;
  redirectUrl: string;

  constructor( private router: Router) { }

  showOverlay(redirectTo: string){
    this.redirectUrl = redirectTo;
    this.show = true;
    
  }

  closeOverlay(){
    this.show = false;
    this.router.navigate([this.redirectUrl]);
  }
}
