import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-validation-summary-info',
  templateUrl: './validation-summary-info.component.html',
  styleUrls: ['./validation-summary-info.component.css']
})
export class ValidationSummaryInfoComponent implements OnInit {

  @Input() form: FormGroup;

  constructor() { }

  ngOnInit(): void {
  }

  validateForm(): string[] {

    let result: string[] = [];
    for (const field in this.form.controls) { // 'field' is a string

      const control = this.form.get(field);// 'control' is a FormControl  

      if(control.errors && control.touched){
        result.push(field);
      }
    
    }

    return result;
  }

}
