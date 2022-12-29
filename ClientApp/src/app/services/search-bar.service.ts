import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Helpers } from '../helpers';
import { ProductVm } from '../model/productVm';
import { ProductsService } from './products.service';

@Injectable({
  providedIn: 'root'
})
export class SearchBarService {

  selectedCity: string;
  searchText: any;
  searchInCity: string;
  distances: string[] = ['+5 km','+10 km', '+15 km', '+30 km', '+50 km', '+100 km', '+200 km' ];
  selectedCityDistance: string;
  timeout: any = null;
  citySearchResults: string[];
  quickSearchResults: ProductVm[];

  constructor(private productService: ProductsService, 
              private router: Router,
              private route: ActivatedRoute) {
    this.selectedCityDistance = "+5 km";
   }

   onSelectProduct(event){

    var km  = +this.selectedCityDistance.replace(/\D/g, "");

    if(!this.searchInCity){
      km = null;
    }
    // if(this.searchText == null){
    //   this.searchText ="empty";
    // }

    if(event.Category){

      this.router.navigate(['/kategoria/' + event.Category.ParentName + '/' + event.Category.Name + '/filtrowanie/'], {
        relativeTo: this.route,
        queryParams: {searchString: this.searchText.Title + ',' + this.searchText.Category.Name, distance: km},
        queryParamsHandling: 'merge',
        // preserve the existing query params in the route
        skipLocationChange: false
        // do not trigger navigation
      });

    }else{

      this.router.navigate(['/filtrowanie/'], {
        relativeTo: this.route,
        queryParams: {searchString: this.searchText, distance: km},
        queryParamsHandling: 'merge',
        // preserve the existing query params in the route
        skipLocationChange: false
        // do not trigger navigation
      });

    }

    // this.router.navigate(['/kategoria/' + event.Category.ParentName + '/' + event.Category.Name + '/filtrowanie/'], { queryParams: {searchString: this.searchText.Title, lat:this.latitude, lon:this.longitude, distance: km}});
  }

  private getSearchString(sTxt: any): string {
    let resSearchTxt = null;

    if(sTxt){
        if(sTxt.Title){
            resSearchTxt  = sTxt.Title;
            if(sTxt.Category){
                sTxt += "," + sTxt.Category.name
            }
        }
    }

    return resSearchTxt;
  }

   searchProducts(event){
    this.productService.getProductNamesContains(this.searchText).subscribe(data => {

      this.quickSearchResults = data;

      // this.quickSearchResults = data.map( m => {
      //   return m.Title;
      // });
  });
}

   searchDistanceChanged($event){
    var km  = +this.selectedCityDistance.replace(/\D/g, "");
    // this.openStreetMap.setMap(this.selectedCity, km, km);
  
    // this.onCityChange(null);
    // this.searchProducts(null);
    // this.onSelect($event);
  
    // this.router.navigate(['filtrowanie'], {
    //   relativeTo: this.route,
    //   queryParams: {searchString: this.searchText?.Title, lat:this.latitude, lon:this.longitude, city: this.searchInCity, distance: km},
    //   queryParamsHandling: 'merge',
    //   // preserve the existing query params in the route
    //   skipLocationChange: false
    //   // do not trigger navigation
    // });
  
  }

  onFocusOut(event){

    var km  = +this.selectedCityDistance.replace(/\D/g, "");
  
    if(!this.searchInCity){
      km = null;
    }
  
    var ss = this.searchText?.Title ?? this.searchText;
  
    if(!this.searchText.Title){
      ss = null;
    }
  
    let localSearchString = this.getSearchString(this.searchText);
  
    if(Helpers.IsRouteEmpty(this.route)){
  
      this.router.navigate(['filtrowanie'], {
        relativeTo: this.route,
        queryParams: {searchString: localSearchString, city: this.searchInCity, distance: km},
        queryParamsHandling: 'merge',
        // preserve the existing query params in the route
        skipLocationChange: false
        // do not trigger navigation
      });
    }else{
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: {searchString: ss, city: this.searchInCity, distance: km},
        queryParamsHandling: 'merge',
        // preserve the existing query params in the route
        skipLocationChange: false
        // do not trigger navigation
      });
    }
  
    // this.onCityChange(null);
    // this.searchProducts(null);
    // this.onSelect(null);
    // var km  = +this.selectedCityDistance.replace(/\D/g, "");
    
    // this.router.navigate(['/'], { queryParams: {searchString: this.searchText, lat:this.latitude, lon:this.longitude, distance: km}});
  }

   onCityChange(event){

    this.selectedCity = event;
    this.searchInCity = event;
    // clearTimeout(this.timeout);
    // // var $this = this;
    // this.timeout = setTimeout(() => {
    //   if (event.keyCode != 13) {
    //     var km  = +this.selectedCityDistance.replace(/\D/g, "");
    //     //this.openStreetMap.setMap(this.selectedCity, 12, km );
  
    //     this.productService.GetCityCoordinates(this.selectedCity).subscribe( res =>
    //       {
    //         this.latitude = +res[0].lat;
    //         this.longitude = +res[0].lon;
  
    //         //if route is not empty
    //         // if(Helpers.IsRouteEmpty(this.route)){
  
    //         //   this.router.navigate(['filtrowanie'], {
    //         //     relativeTo: this.route,
    //         //     queryParams: {searchString: this.searchText?.Title, lat:this.latitude, lon:this.longitude, city: this.searchInCity, distance: km},
    //         //     queryParamsHandling: 'merge',
    //         //     // preserve the existing query params in the route
    //         //     skipLocationChange: false
    //         //     // do not trigger navigation
    //         //   });
    //         // }else{
    //         //   this.router.navigate([], {
    //         //     relativeTo: this.route,
    //         //     queryParams: {searchString: this.searchText?.Title, lat:this.latitude, lon:this.longitude, city: this.searchInCity, distance: km},
    //         //     queryParamsHandling: 'merge',
    //         //     // preserve the existing query params in the route
    //         //     skipLocationChange: false
    //         //     // do not trigger navigation
    //         //   });
    //         // }
  
    //         // this.searchProducts(null);
    //         // this.onSelect(event);
    //       });
    //   }
    // }, 500);
  }

  onCityFind(event){

    this.selectedCity = event.query;
    clearTimeout(this.timeout);
    // var $this = this;
  
  
      this.productService.GetCityAutocomplete(this.selectedCity).subscribe( res =>
        {
          var resXX = res.features.filter(x => x.properties.country=="Poland" &&
                                                ( x.properties.osm_value== "city"  ||
                                                x.properties.osm_value== "town" ||
                                                x.properties.osm_value== "village" ));
  
          this.citySearchResults = resXX.map( f => {
            return f.properties.name + ", " + Helpers.translateVoievodeship(f.properties.state.replace(" Voivodeship",""));
          // this.latitude = +res[0].lat;
          // this.longitude = +res[0].lon;
        });
        
      // if (event.keyCode != 13) {
      //   var km  = +this.selectedCityDistance.replace(/\D/g, "");
      //   this.openStreetMap.setMap(this.selectedCity, 12, km );
  
      //   this.productService.GetCityCoordinates(this.selectedCity).subscribe( res =>
      //     {
      //       this.latitude = +res[0].lat;
      //       this.longitude = +res[0].lon;
      //     });
      // }
      });
  }
}
