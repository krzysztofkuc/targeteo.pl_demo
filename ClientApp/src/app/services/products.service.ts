import { Injectable } from '@angular/core';
import { HttpRequestsService } from './http-requests.service';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { HomeVm } from '../model/homeVm';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { AddCategorytVm } from '../model/addCategoryVm';
import { CategoryVm } from '../model/categoryVm';
import { AddAttributeToProduct } from '../model/addAttributeToProduct';
import { ProductAttributeVm } from '../model/productAttributeVm';
import { ProductVm } from '../model/productVm';
import { AddProductVm } from '../model/addProductVm';
import { CategoryAttributeVm } from '../model/categoryAttributeVm';
import { MsgToUserVm } from '../model/msgToUserVm';
import { OrderVm } from '../model/orderVm';
import { orderDetailVm } from '../model/orderDetailVm';
import { MenuItem } from 'primeng/api';
import { OrderOpinionVm } from '../model/orderOpinionVm';
import WsMenuItem from '../model/wsMenuItem';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  public filteredProducts: ProductVm[];

  _urlBase = "Home";
  _urlIndex = this._urlBase + "/Index";
  isLoading: boolean = false;
  public numberOfUnreadMessaes: number = 0;

  public filtersForProducts: CategoryAttributeVm[];

  public filteredProducts$:BehaviorSubject<ProductVm[]> = new BehaviorSubject<ProductVm[]>([]);

  constructor(private _http: HttpRequestsService, private httpClient: HttpClient) {}

  Index(parentCategory: string, childCategory: string, queryParams) : Observable<HomeVm>{
    this._urlIndex = this._urlBase + "/Index";

    if(parentCategory){
      this._urlIndex += "/" + parentCategory;
    }

    if(childCategory && childCategory != "filtrowanie"){
      this._urlIndex += "/" + childCategory;
    }

    if(queryParams != null){
      let params = new HttpParams();
      queryParams.keys.forEach(key => {
        var value = queryParams.params[key];

        if(Array.isArray(value)){
          value.forEach(e => {
            params = params.append(key, e);    
          });
        }else{
          params = params.append(key, value);
        }
      });

      return this._http.getWithParams(this._urlIndex, params);
    }

    return this._http.get(this._urlIndex);
  };

  setProducts(products: ProductVm[]){
    this.filteredProducts = products;
    this.filteredProducts$.next(this.filteredProducts);
  }
  
  getProductNamesContains(name: string){

    let params = new HttpParams().set("productName", name);
    return this._http.getWithParams<ProductVm[]>("home/Index" + "/get-products-contains", params);
  }

  UploadPhoto(formData: FormData): Observable<any>{
    return this._http.postFormData(this._urlIndex + "/file-upload", formData);
  }

  GetPhoto<T>(fileName: string): Observable<T>{
    return this._http.getWithParamsBlob(this._urlIndex + "/get-image", fileName);
  }

  AddProductGetInitModel(id): Observable<AddProductVm>{
    let params = new HttpParams().set("id", id);
    return this._http.getWithParams<AddProductVm>("LoggedUser/Manage/add-product-get", params);
  }

  DeleteProduct(product: ProductVm): Observable<any>{
    
    let params = new HttpParams().set("id", product.ProductId);

    var index = this.filteredProducts.indexOf(product);
    this.filteredProducts.splice(index, 1);
    this.setProducts(this.filteredProducts);

    return this._http.getWithParams("LoggedUser/Manage/delete-product", params);
  }

  GetBestSellers(length: number): Observable<any>{
    
    let params = new HttpParams().set("length", length.toString());

    return this._http.getWithParams(this._urlBase + "/Index/get-bestSellers", params);
  }

  GetLastAddedProducts(length: number): Observable<any>{
    
    let params = new HttpParams().set("length", length.toString());

    return this._http.getWithParams(this._urlBase + "/Index/get-lastAddedProducts", params);
  }

  GetProduct(id): Observable<any>{
    
    let params = new HttpParams().set("id", id);

    return this._http.getWithParams(this._urlBase + "/Index/get-product", params);
  }

  GetAddCategoryModel(parent: string = "", child: string = ""): Observable<AddCategorytVm>{

    let params = new HttpParams().set("parent", parent).set("child", child);

    return this._http.getWithParams<AddCategorytVm>("LoggedUser/Manage/addCategory", params);
  }

  AddCategory(data: AddCategorytVm): Observable<AddCategorytVm>{

    return this._http.post<AddCategorytVm>("LoggedUser/Manage/addCategory", data);
  }

  EditCategory(data: CategoryVm): Observable<CategoryVm>{

    return this._http.post<CategoryVm>("Root/Manage/editCategory", data);
  }

  DeleteCategory(parent: string, child: string): Observable<boolean>{

    let params = new HttpParams().set("parent", parent).set("child", child);
    return this._http.getWithParams("Root/Manage/deleteCategory", params);
  }

  AddAttributeGet(): Observable<AddAttributeToProduct>{

    return this._http.get<AddAttributeToProduct>("LoggedUser/Manage/addAttribute");
  }

  EditAttributeGet(prodId, attrId): Observable<AddAttributeToProduct>{
    let params = new HttpParams().set("prodId", prodId).set("attrId", attrId);

    return this._http.getWithParams<AddAttributeToProduct>("LoggedUser/Manage/editProductAttribute", params);
  }

  DeleteProductAttribute(attrId): Observable<boolean>{
    let params = new HttpParams().set("attrId", attrId);

    return this._http.getWithParams<boolean>("LoggedUser/Manage/deleteProductAttribute", params);
  }

  SaveAttribute(data: AddAttributeToProduct): Observable<any>{

    return this._http.post<any>("LoggedUser/Manage/addAttribute", data);
  }

  // SaveCategoryAttribute(data: CategoryAttributeVm): Observable<any>{

  //   return this._http.post<any>(this._urlBase + "/Manage/addAttribute", data);
  // }

  SaveViewFilter(categoryAttrId ,viewFilterType, hide): Observable<any>{
    
    let params = new HttpParams().set("categoryAttrId", categoryAttrId).set("viewFilterType", viewFilterType).set("hide", hide);

    return this._http.getWithParams<AddAttributeToProduct>(this._urlBase + "/Manage/saveViewFilter", params);

  }

  GetAllCategoryAttributes(catId: string): Observable<any>{
    
    let params = new HttpParams().set("categoryId", catId);

    return this._http.getWithParams<CategoryAttributeVm[]>(this._urlBase + "/get-category-attrs", params);
  }

  AskQuestionToProduct(msg: MsgToUserVm): Observable<any>{
    
    // let params = new HttpParams().set("categoryId", catId);

    return this._http.post<MsgToUserVm>("LoggedUser/Manage/ask-question-for-product", msg);
  }

  GetCityCoordinates(city: string): Observable<any>{
    
    let params = new HttpParams().set("q", city).set("format", "json");
    params = params.append("countrycode", "pl");
    let url = 'https://nominatim.openstreetmap.org/search';

    return this.httpClient.get<any>(url, { headers: this.getHeaderForCity(), params: params });
  }

  GetCityAutocomplete(city: string): Observable<any>{
    
    let params = new HttpParams().set("q", city);
    let url = 'https://photon.komoot.io/api/';

    return this.httpClient.get<any>(url, { headers: this.getHeaderForCity(), params: params });
  }

  private getHeaderForCity(): HttpHeaders {
    return new HttpHeaders({
        'Access-Control-Allow-Origin': '*',
        'Content-Type': 'application/json',
    })
  };

  EndAnnouncementClick(productId: string){
    let params = new HttpParams().set("productId", productId);
    
    return this._http.getWithParams<ProductVm>("LoggedUser/manage/endAnnouncement", params);
  }

  activateAnnouncementClick(produyctId: string){
    let params = new HttpParams().set("productId", produyctId);
    
    return this._http.getWithParams<ProductVm>("LoggedUser/manage/activateAnnouncement", params);
  }

  addAnnouncementActivityDaysClick(productId: string){
    let params = new HttpParams().set("productId", productId);
    
    return this._http.getWithParams<ProductVm>("LoggedUser/manage/addAnnouncementActivityDays", params);
  }

  GetCountedUnreadMessages(): Observable<number>{
    
    return this._http.get<number>("LoggedUser/manage/getCountedUnreadMessages");
  }

  GetSoldAnnouncements(userId: string): Observable<orderDetailVm[]>{
    let params = new HttpParams().set("userId", userId);
    return this._http.getWithParams<orderDetailVm[]>("UserProfile/soldannouncements", params);
  }

  GetBoughtAnnouncements(userId: string): Observable<orderDetailVm[]>{
    let params = new HttpParams().set("userId", userId);
    return this._http.getWithParams<orderDetailVm[]>("UserProfile/boughtannouncements", params);
  }

  GetActiveAnnouncements(userId: string): Observable<ProductVm[]>{
    let params = new HttpParams().set("userId", userId);
    return this._http.getWithParams<ProductVm[]>("UserProfile/activeannouncements", params);
  }

  GetMenu(): Observable<WsMenuItem[]>{
    return this._http.get<WsMenuItem[]>("Home/GetMenu");
  }

  AddOrderOpinion(orderDetailId: number): Observable<OrderOpinionVm>{
    let params = new HttpParams().set("orderDetailId", orderDetailId);
    return this._http.getWithParams<OrderOpinionVm>("LoggedUser/addOrderOpinion", params);
  }

  AddOrderOpinionPost(opinion: OrderOpinionVm): Observable<any>{
    
    // let params = new HttpParams().set("categoryId", catId);

    return this._http.post<any>("LoggedUser/addorderopinion", opinion);
  }

  GetProductOpinions(id: string): Observable<OrderOpinionVm[]>{
    let params = new HttpParams().set("prodId", id);
    return this._http.getWithParams<OrderOpinionVm[]>("LoggedUser/getorderopinions", params);
  }

  GetUserOpinions(userId: string): Observable<OrderOpinionVm[]>{
    let params = new HttpParams().set("userId", userId);
    return this._http.getWithParams<OrderOpinionVm[]>("Home/getuseropinions", params);
  }
}