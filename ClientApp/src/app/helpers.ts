import { ActivatedRoute } from "@angular/router";

export abstract class Helpers{

    static ships: Map<string, string> = new Map([
            ['Pomeranian','pomorskie'],
            ['Kuyavian-Pomeranian', 'kujawsko-pomorskie'],
            ['Lower Silesian', 'dolnośląskie'],
            ['Lublin', 'lubelskie'],
            ['Lubusz', 'lubuskie'],
            ['Lesser Poland', 'małopolskie'],
            ['Masovian', 'mazowieckie'],
            ['Opole', 'opolskie'],
            ['Subcarpathian', 'podkarpackie'],
            ['Podlaskie', 'podlaskie'],
            ['Silesian', 'śląskie'],
            ['Świętokrzyskie', 'świętokrzyskie'],
            ['Greater Poland', 'wielkopolskie'],
            ['West Pomeranian', 'zachodniopomorskie'],
            ['Łódź', 'łudzkie'],
            ['Warmian-Masurian', 'warmińsko-mazurskie'],
            
    ]);

    public static translateVoievodeship(ship){
        return this.ships.get(ship) ?? "";
    }

    public static IsRouteEmpty(route: ActivatedRoute): boolean {

        if(route.snapshot.queryParams?.length > 0) 
        {
          return false;
        }
    
        return true;
    }

    // // A little bit simplified version
    // public static groupBy = <T, K extends keyof any>(arr: T[], key: (i: T) => K) =>
    // arr.reduce((groups, item) => {
    //   (groups[key(item)] ||= []).push(item);
    //   return groups;
    // }, {} as Record<K, T[]>);
}