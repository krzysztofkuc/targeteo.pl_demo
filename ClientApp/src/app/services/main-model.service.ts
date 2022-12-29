import { Injectable, OnDestroy } from '@angular/core';
import { HomeVm } from '../model/homeVm';
import { BehaviorSubject, Observable } from 'rxjs';
import { ProductsService } from './products.service';
import { map } from 'rxjs/operators';
import { CategoryAttributeVm } from '../model/categoryAttributeVm';
import { CategoryVm } from '../model/categoryVm';
import { MenuItem } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class MainModelService implements OnDestroy {

  public data: HomeVm;
  private isModelReady = new BehaviorSubject<boolean>(false); // type: boolean - Default value: false

    public get isReady$() {
        return this.isModelReady.asObservable(); // returns BehaviourSubject as an Obervable
    }

    ngOnDestroy() {
      this.isModelReady.complete(); // Good practice to complete subject on lifecycle hook ngOnDestroy
  }

  constructor(private service: ProductsService) { }

  public updateModel(parent: string, child: string) : Observable<HomeVm> {

    if(child == "filtrowanie"){
      child = null;
    }

    return this.service.Index(parent, child, null).pipe(map( responseData => {
      this.data = responseData;

      return responseData;
    }));
  }

  public getHomeModel(parent: string, child: string) : Observable<HomeVm> {

    if(child == "filtrowanie"){
      child = null;
    }

    return this.service.Index(parent, child, null).pipe(map( responseData => {
      return responseData;
    }));
  }

  //this should be extracted to get only filter not all model
  public filterModel(parent: string, child: string, queryParams = null) : Observable<HomeVm> {

    if(child == "filtrowanie"){
      child = null;
    }

    return this.service.Index(parent, child, queryParams).pipe(map( responseData => {
      this.data = responseData;
      this.isModelReady.next(true);
      return responseData;
    }));
  }

  public GetMenu() : Observable<MenuItem[]> {

    return this.service.GetMenu().pipe(map( responseData => {
      return responseData;
    }));
  }
}
