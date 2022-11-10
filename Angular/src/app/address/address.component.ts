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
  countryCtrl = new FormControl(null, Validators.required);
  phoneNumCtrl = new FormControl(null, [Validators.required, Validators.pattern(/^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$/)]);
  cityCtrl = new FormControl(null, Validators.required);
  provinceOrStateCtrl = new FormControl(null, Validators.required);

  addressGroup: FormGroup = new FormGroup({
    homeAddress: this.homeAddressCtrl,
    homeAddress2: this.homeAddress2Ctrl,
    country: this.countryCtrl,
    phoneNum: this.phoneNumCtrl,
    city: this.cityCtrl,
    provinceOrState: this.provinceOrStateCtrl
  })

  private token:any;
  private requestInfo:any;
  countries: Array<any> = ["Canada", "United States"];
  states: Array<any> = [ "AK - Alaska", 
  "AL - Alabama", 
  "AR - Arkansas", 
  "AS - American Samoa", 
  "AZ - Arizona", 
  "CA - California", 
  "CO - Colorado", 
  "CT - Connecticut", 
  "DC - District of Columbia", 
  "DE - Delaware", 
  "FL - Florida", 
  "GA - Georgia", 
  "GU - Guam", 
  "HI - Hawaii", 
  "IA - Iowa", 
  "ID - Idaho", 
  "IL - Illinois", 
  "IN - Indiana", 
  "KS - Kansas", 
  "KY - Kentucky", 
  "LA - Louisiana", 
  "MA - Massachusetts", 
  "MD - Maryland", 
  "ME - Maine", 
  "MI - Michigan", 
  "MN - Minnesota", 
  "MO - Missouri", 
  "MS - Mississippi", 
  "MT - Montana", 
  "NC - North Carolina", 
  "ND - North Dakota", 
  "NE - Nebraska", 
  "NH - New Hampshire", 
  "NJ - New Jersey", 
  "NM - New Mexico", 
  "NV - Nevada", 
  "NY - New York", 
  "OH - Ohio", 
  "OK - Oklahoma", 
  "OR - Oregon", 
  "PA - Pennsylvania", 
  "PR - Puerto Rico", 
  "RI - Rhode Island", 
  "SC - South Carolina", 
  "SD - South Dakota", 
  "TN - Tennessee", 
  "TX - Texas", 
  "UT - Utah", 
  "VA - Virginia", 
  "VI - Virgin Islands", 
  "VT - Vermont", 
  "WA - Washington", 
  "WI - Wisconsin", 
  "WV - West Virginia", 
  "WY - Wyoming"];
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

  attemptSubmit(){
    console.log(this.addressGroup.valid);
  }
}
