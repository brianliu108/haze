import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.scss']
})
export class AddressComponent implements OnInit {
  errors: Array<any> = [];
  homeAddressCtrl = new FormControl(null, Validators.required);
  homeAddress2Ctrl = new FormControl(null);
  shippingAddressCtrl = new FormControl(null);
  billingAddressCtrl = new FormControl(null, Validators.required);
  phoneNumCtrl = new FormControl(null, [Validators.required, Validators.pattern(/^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$/)]);
  cityCtrl = new FormControl(null, Validators.required);
  provinceCtrl = new FormControl(null, Validators.required);

  private token:any;
  private requestInfo:any;
  provinces: Array<any> = ["Alberta", "Manitoba", "Northwest Territories", "New Brunswick", "Newfoundland and Labrador", "Nova Scotia", "Nunavut", "Ontario", "Prince Edward Island", "Quebec", "Saskatchewan", "Yukon"];
  constructor(private appComponent: AppComponent) { }

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    if (!this.token)
      return this.appComponent.navigate("");
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };
    
    try {
      let userResponse = await axios.get(this.appComponent.apiHost + "/GetUser",this.requestInfo);
      console.log(userResponse.data);
    } catch (e) {
      this.errors.push(e);
    }
  }

  routeToProfile(){
    this.appComponent.navigate("/profile");
  }
}
