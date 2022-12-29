import { Component, ElementRef, Inject, Input, OnInit, ViewChild } from '@angular/core';
import Map from 'ol/Map';
import View from 'ol/View';

import OSM from 'ol/source/OSM';
import {transform} from 'ol/proj.js';

import 'ol/ol.css';
import Feature from 'ol/Feature';
import {Circle} from 'ol/geom';
import {Vector as VectorSource} from 'ol/source';
import {Style} from 'ol/style';
import {Tile as TileLayer, Vector as VectorLayer} from 'ol/layer';
import { ProductsService } from 'src/app/services/products.service';
import { CityVm } from 'src/app/model/cityVm';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {


  // @ViewChild(this.mapId, {static : false}) private mapEleRef:ElementRef;
  @Input() mapId: string;
  @Input() selectedCity: CityVm;
  //lat lon Warszawa
  latitude: number = 52.22949061515579;
  longitude: number = 21.01930276865225;
  map: Map;

  vectorSource: any;
  vectorLayer: any;
  mapView: any;

  constructor(private productService: ProductsService,@Inject(DOCUMENT) document ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(){
      //new map
      const circleFeature = new Feature({
        geometry: new Circle(transform([this.longitude, this.latitude ], 'EPSG:4326', 'EPSG:3857'), 5000),
      });
      
      this.vectorSource = new VectorSource({
        features: [circleFeature],
      });

      this.vectorLayer = new VectorLayer({
        source: this.vectorSource,
      });

      this.mapView = new View({
        center: transform([this.longitude, this.latitude ], 'EPSG:4326', 'EPSG:3857'),
        zoom: 12,
      });

      this.map = new Map({
        layers: [
          new TileLayer({
            source: new OSM(),
            visible: true,
          }),
          this.vectorLayer,
        ],
        target: this.mapId,
        view: this.mapView,
      });
  }

  setMap(city: string, zoom: number, distanceInM: number){
    this.productService.GetCityCoordinates(city).subscribe( res =>
      {
        this.latitude = +res[0].lat;
        this.longitude = +res[0].lon;

        var newView = new View({
          center: transform([this.longitude, this.latitude ], 'EPSG:4326', 'EPSG:3857'),
          zoom: this.transformDistanceToZoom(zoom),
        });

        //new map
        const circleFeature = new Feature({
          geometry: new Circle(transform([this.longitude, this.latitude ], 'EPSG:4326', 'EPSG:3857'), distanceInM * 1000),
        });
        
        this.vectorSource = new VectorSource({
          features: [circleFeature],
        });

        this.vectorLayer.setSource(this.vectorSource);

        //set binded Model
        if(this.selectedCity){
          this.selectedCity.City = city;
          this.selectedCity.Latitude = this.latitude;
          this.selectedCity.Longitude = this.longitude;
        }

        this.map.setView(newView);
        // this.map.invalidateSize();
        
        // this.selectedCity.City = city;
        // this.selectedCity = { City: city, Latitude: this.latitude, Longitude: this.longitude } as CityVm;

      });
  }

  public loadMap(){
    this.map = new Map({
      layers: [
        new TileLayer({
          source: new OSM(),
          visible: true,
        }),
        this.vectorLayer,
      ],
      target: this.mapId,
      view: this.mapView,
    });
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
