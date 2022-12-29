import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpRequestsService } from './http-requests.service';
import { Observable } from 'rxjs';
import { OneUserOrderVm } from '../model/oneUserOrderVm';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class OrderService {

  _urlBase = "Home";
  _urlIndex = this._urlBase + "/Order";

  constructor(private route: ActivatedRoute,
              private _http: HttpRequestsService) { }

  SaveOrder(orders: OneUserOrderVm[]): Observable<any>{
    return this._http.post<any>("LoggedUser/Order/completeOrder", orders);
  }

  GetSupplyMethods(): Observable<any>{
    return this._http.get("LoggedUser/Order/getSupplyMeythods");
  }

  GetAllorders(): Observable<any>{
    return this._http.get("LoggedUser/Order/getAllOrders");
  }

  ChangeStatus(orderId: string, status: string, trackingNo: string): Observable<any>{

    let params = new HttpParams().set("orderId", orderId).set("changeToStatus", status).set("trackingNo", trackingNo);;
    return this._http.getWithParams<any>("LoggedUser/order/changeStatus", params);
  }

  GetOrders(orderId: string): Observable<any>{
    let params = new HttpParams().set("orderSummaryId", orderId);
    return this._http.getWithParams<any>("LoggedUser/Order/getOrders", params);
  }

  GenerateOrderId(): Observable<any>{
    return this._http.get<any>("LoggedUser/order/generateOrderId");
  }

  GetString(): Observable<any>{
    return this._http.get<any>("LoggedUser/getString");
  }
}
