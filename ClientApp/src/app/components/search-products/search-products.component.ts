import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CityVm } from 'src/app/model/cityVm';
import { ProductsService } from 'src/app/services/products.service';
import { MapComponent } from '../map/map.component';
import * as L from 'leaflet';

@Component({
  selector: 'app-search-products',
  templateUrl: './search-products.component.html',
  styleUrls: ['./search-products.component.css']
})

//many of this component function are redundant from app.component
export class SearchProductsComponent implements OnInit {

  @ViewChild('mobileSearchModalMap', {static : false}) private openStreetMap:MapComponent;
  quickSearchResults: string[];
  searchText: string;
  selectedCityDistance: string;
  selectedCity: string;
  selectedCityVm: CityVm = new CityVm();
  latitude: number = 52.22949061515579;
  longitude: number = 21.01930276865225;
  timeout: any = null;
  distances: string[];
  searchInCity: string;
  map;
  zoom = 8;
  radious = 5000;

  constructor(private productService: ProductsService, private router: Router) {
    this.selectedCityVm = new CityVm();
    this.selectedCityVm.City = "Warszawa";
    this.selectedCityVm.Latitude =this.latitude;
    this.selectedCityVm.Longitude = this.longitude;

    this.selectedCityDistance = '5 km';
    this.distances = ['5 km','10 km', '15 km', '30 km', '50 km', '100 km', '200 km', '500 km' ];
   }

  ngOnInit(): void {
  }

  initMap(): void {
    
    if(this.map){
      this.map.off();
      this.map.remove();
    }

    this.map = L.map('map', {
      center: [ this.latitude, this.longitude ],
      zoom: this.zoom
    });

    const tiles = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      minZoom: 3,
      attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    });

    //circle
    var circle = L.circle([this.latitude, this.longitude], {
      color: 'red',
      fillColor: '#f03',
      fillOpacity: 0.3,
      radius: this.radious,
    }).addTo(this.map);

    tiles.addTo(this.map);
  }

  searchProducts(event){
    this.productService.getProductNamesContains(event.query).subscribe(data => {
      this.quickSearchResults = data.map( m => {
        return m.Title;
      });
    });
  }

  onSelect(event){
    var km  = +this.selectedCityDistance.replace(/\D/g, "");

    this.router.navigate(['/'], { queryParams: {searchString: event, lat:this.latitude, lon:this.longitude, distance: km}});
  }

  onFocusOut(){

    var km  = +this.selectedCityDistance.replace(/\D/g, "");

    this.router.navigate(['/'], { queryParams: {searchString: this.searchText, lat:this.latitude, lon:this.longitude, distance: km}});
  }

  
onCityChange(event){

  this.selectedCity = event.target.value;

  clearTimeout(this.timeout);

    this.timeout = setTimeout(() => {
    if (event.keyCode != 13) {
      var km  = +this.selectedCityDistance.replace(/\D/g, "");

      this.productService.GetCityCoordinates(this.selectedCity).subscribe( res =>
        {
          this.latitude = +res[0].lat;
          this.longitude = +res[0].lon;
          this.initMap();
        });
    }
  }, 300);
}

searchDistanceChanged($event){
  var km  = +this.selectedCityDistance.replace(/\D/g, "");

  this.zoom = this.transformDistanceToZoom(km);
  this.radious = km * 1000;
  this.initMap();
  // this.openStreetMap.setMap(this.selectedCity, km, km);
}

reloadMap(){
  this.openStreetMap.loadMap();
}

transformDistanceToZoom(distance: number) : number {
  let res = 10;
  switch(distance){
    case 5:
      res = 12;
      break;
    case 10:
      res = 10;
      break;
    case 30:
      res = 8;
      break;
    case 50:
      res = 7;
      break;
    case 100:
      res = 6;
      break;
    case 200:
      res = 6;
      break;
    case 500:
      res = 5;
      break;
  }
  return res;
}

}
