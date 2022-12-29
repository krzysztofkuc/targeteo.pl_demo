import { Injectable } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BreadcrumbService {

  public home: MenuItem;

  // private crumbs: Subject<MenuItem[]>;
  // crumbs$: Observable<MenuItem[]>;
  public crumbsSynchro: MenuItem[];

  constructor() {
    this.home = {icon: 'pi pi-home', routerLink: '/'};
    // this.crumbs = new Subject<MenuItem[]>();
    // this.crumbs$ = this.crumbs.asObservable();
   }

  setCrumbs(items: MenuItem[]) {

    this.crumbsSynchro = items;

    // this.crumbs.next(
    //   (items).map(item =>
    //       Object.assign({}, item, {
    //         routerLinkActiveOptions: { exact: true }
    //       })
    //     )
    // );
  }
}
