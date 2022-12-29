import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from '../../environments/environment';

@Injectable()
export class HttpRequestsService {

    private BASE_URL: string;
    private AUTH_URL: string;
    //private baseUrl: string = "http://localhost:56796/api/";
    //private baseUrl: string = "http://retro.shop.pl/api/";
    private baseUrl: string = environment.host


    constructor(private httpClient: HttpClient) {
        this.BASE_URL = this.baseUrl;
        this.AUTH_URL = this.BASE_URL + "Auth/login";
    }

    get<T>(endpoint: string): Observable<T> {
        return this.httpClient.get<T>(this.BASE_URL + endpoint, { headers: this.getHeader() });
    }

    getWithParams<T>(endpoint: string, params: any): Observable<T> {
        return this.httpClient.get<T>(this.BASE_URL + endpoint, { headers: this.getHeader(), params: params });
    }

    getWithParamsBlob<T>(endpoint: string, params: any): Observable<T> {
        return this.httpClient.get<T>(this.BASE_URL + endpoint, { headers: { responseType: 'blob', observe: 'response' }, params: params });
    }

    post<T>(endpoint: string, body: any): Observable<T> {
        return this.httpClient.post<T>(this.BASE_URL + endpoint,
            body,
            {
                headers: this.getHeader()
            })
    }

    postFormData<T>(endpoint: string, formData: FormData): Observable<T> {
        return this.httpClient.post<any>(this.BASE_URL + endpoint, formData);
    }

    authorize<T>(body: any): Observable<T> {
        return this.httpClient.post<T>(this.AUTH_URL,
            body,
            {
                headers: this.getHeader()
            })
    }

    protected getHeader(): HttpHeaders {
        return new HttpHeaders({
            'Access-Control-Allow-Origin': '*',
            'Content-Type': 'application/json',
        })
    }

    // private getHeaderBlob(): HttpHeaders {
    //     return new HttpHeaders({
    //         'Access-Control-Allow-Origin': '*',
    //         'Content-Type': 'application/json',
    //     })
    // }

    private getToken(): string {
        return localStorage.getItem('token');
    }
}
