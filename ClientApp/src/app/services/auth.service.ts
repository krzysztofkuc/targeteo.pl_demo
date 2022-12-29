import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { HttpRequestsService } from './http-requests.service';
import { UserLogin } from '../model/userLogin';

@Injectable()
export class AuthService {

    private _loginEndpoint = "Login";
    private _registerEndpoint = "Auth/registerUser";
    private _sendEmailToResetPasswordEndpoint = "Auth/sendEmailToResetPassword";
    private _canResetPasswordEndpoint = "Auth/canResetPassword";
    private _canConfirmEmailEndpoint = "Auth/canConfirmEmail";
    private _confirmEmailEndpoint = "Auth/confirmEmail";
    private _resetPasswordEndpoint = "Auth/resetPassword";
    private _getProfiles = "menu";
    private _refreshTokenEndpoint = "Login/Refresh";


    constructor(private _http: HttpRequestsService, private router: Router, private activatedRoute: ActivatedRoute) { }

    login(user: UserLogin): Observable<UserLogin> {

        return this._http.authorize<UserLogin>(user);
    }

    sendEmailToResetPassword(email: string): Observable<boolean> {

        let params = new HttpParams().set("email", email);

        return this._http.getWithParams(this._sendEmailToResetPasswordEndpoint, params);
    }

    register(user: UserLogin): Observable<any> {

        return this._http.post(this._registerEndpoint, user);
    }

    canResetPassword(token: string): Observable<boolean> {

        let params = new HttpParams().set("token", token);

        return this._http.getWithParams(this._canResetPasswordEndpoint, params);
    }

    resetPassword(pass: string, token: string): Observable<boolean> {

        let params = new HttpParams().set("token", token).set("password", pass);

        return this._http.getWithParams(this._resetPasswordEndpoint, params);
    }

    canConfirmEmail(token: string): Observable<boolean> {

        let params = new HttpParams().set("token", token);

        return this._http.getWithParams(this._canConfirmEmailEndpoint, params);
    }

    confirmEmail(token: string): Observable<boolean> {

        let params = new HttpParams().set("token", token);

        return this._http.getWithParams(this._confirmEmailEndpoint, params);
    }

    LoginOnAppServer(user: UserLogin): Observable<boolean> {
        return this._http.post<boolean>(this._loginEndpoint, user);
    }

    // This should be moved somewhere else
    // getProfiles(login?: string): Observable<UserInfo[]> {
    //     let params = new HttpParams();
    //     params.set("login", login);

    //     return this._http.getWithParams<UserInfo[]>(this._getProfiles, params);
    // }

    refreshAccessToken(): Observable<string> {
        var tokenLocal = this.getRefreshToken();

        let token = {
            'Value': tokenLocal
        }

        return this._http.post<any>(this._refreshTokenEndpoint, token);
    }
    
    logout(timeout?) {
        this._http.post('Auth/logout', null).subscribe(res => {
            localStorage.removeItem("token");
            localStorage.removeItem("login");
            localStorage.removeItem("refreshToken");
            // this.router.navigate(['/login'], {
            //     relativeTo: this.activatedRoute,
            //     queryParams: {
            //         timeout: timeout,
            //     },
            //     queryParamsHandling: 'merge'
            // });
        }, error => {
        })
    }

    getAccessToken(): string {
        return localStorage.getItem("token");
    }

    getRefreshToken(): string {
        return localStorage.getItem("refreshToken");
    }

    isLoggedIn():boolean {
        let token = localStorage.getItem('token');
        if (token) {
            return true;
        }
        return false;
    }
}
