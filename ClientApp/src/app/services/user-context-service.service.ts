import { Injectable } from '@angular/core';
import { User } from 'oidc-client';
import { UserLogin } from '../model/userLogin';

@Injectable({
  providedIn: 'root'
})
export class UserContextServiceService {

  public User: UserLogin;

  constructor() {}

  public get IsAdmin() {
    
    return localStorage.getItem("roles")?.includes("Admin") ? true : false;
    // return this.User?.Roles?.includes("Admin") ? true : false;
  }

  public get Login() {
    
    return localStorage.getItem("login");
    // return this.User?.Roles?.includes("Admin") ? true : false;
  }

  public get UserId() {
    
    return localStorage.getItem("id");
    // return this.User?.Roles?.includes("Admin") ? true : false;
  }

}
