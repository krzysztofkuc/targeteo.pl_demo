import { IUserAccountVm } from './../model/userAccountVm';
import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IUserAccountSummary } from '../model/userAccountSummaryVm';
import { userProfileVm } from '../model/userProfileVm';
import { WithdrawVm } from '../model/withdrawVm';
import { HttpRequestsService } from './http-requests.service';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  _urlBase = "UserProfile";

  constructor(private _http: HttpRequestsService) { }

  Get(login: string): Observable<any>{

    let params = new HttpParams().set("login", login);

    return this._http.getWithParams<any>(this._urlBase + "/Messages/Get", params);
  }

  GetProductMessages(userIdFrom: string, userIdTo: string, prodId: string): Observable<any>{

    let params = new HttpParams().set("userIdFrom", userIdFrom).set("userIdTo", userIdTo).set("prodId", prodId);;

    return this._http.getWithParams<any>(this._urlBase + "/Messages/GetProductMessages", params);
  }

  GetUserAnnouncements(userId): Observable<any>{

    let params = new HttpParams().set("userId", userId).set("userId", userId);

    return this._http.getWithParams<any>(this._urlBase + "/UserAnnouncements", params);
  }

  GetUserProfile(): Observable<userProfileVm>{

    //let params = new HttpParams().set("userId", userId).set("userId", userId);

    return this._http.get<userProfileVm>(this._urlBase + "/get");
  }

  SaveUserProfile(profile: userProfileVm): Observable<userProfileVm>{

    //let params = new HttpParams().set("userId", userId).set("userId", userId);

    return this._http.post(this._urlBase + "/save", profile);
  }

  GetUserAccount(): Observable<IUserAccountSummary>{

    return this._http.get<IUserAccountSummary>(this._urlBase + "/getuseraccount");
  }

  RequestWithdraw(model: WithdrawVm): Observable<boolean>{

    return this._http.post(this._urlBase + "/requestWithdraw", model);
  }

  confirmWithdrawByAdmin(model: IUserAccountVm): Observable<IUserAccountVm>{

    return this._http.post("Root/confirmWithdrawByAdmin", model);
  }

}
