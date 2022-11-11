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
  billingHomeAddressCtrl = new FormControl(null, Validators.required);
  billingHomeAddress2Ctrl = new FormControl(null);
  billingCountryCtrl = new FormControl(null, Validators.required);
  billingPhoneNumCtrl = new FormControl(null, [Validators.required, Validators.pattern(/^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$/)]);
  billingCityCtrl = new FormControl(null, Validators.required);
  billingPostalZipCodeCtrl = new FormControl(null, Validators.required);
  billingProvinceOrStateCtrl = new FormControl(null, Validators.required);

  success: boolean = false;
  showShipping: boolean = false;

  shippingHomeAddressCtrl = new FormControl(null, Validators.required);
  shippingHomeAddress2Ctrl = new FormControl(null, Validators.required);
  shippingCountryCtrl = new FormControl(null, Validators.required);
  shippingPhoneNumCtrl = new FormControl(null, [Validators.required, Validators.pattern(/^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$/)]);
  shippingCityCtrl = new FormControl(null, Validators.required);
  shippingPostalZipCodeCtrl = new FormControl(null, Validators.required);
  shippingProvinceOrStateCtrl = new FormControl(null, Validators.required);

  storedBillingAddress: any;
  storedShippingAddress: any;

  billingAddressGroup: FormGroup = new FormGroup({
    homeAddress: this.billingHomeAddressCtrl,
    homeAddress2: this.billingHomeAddress2Ctrl,
    country: this.billingCountryCtrl,
    phoneNum: this.billingPhoneNumCtrl,
    city: this.billingCityCtrl,
    postalZipCode: this.billingPostalZipCodeCtrl,
    provinceOrState: this.billingProvinceOrStateCtrl
  });

  shippingAddressGroup: FormGroup = new FormGroup({
    homeAddress: this.shippingHomeAddressCtrl,
    homeAddress2: this.shippingHomeAddress2Ctrl,
    country: this.shippingCountryCtrl,
    phoneNum: this.shippingPhoneNumCtrl,
    city: this.shippingCityCtrl,
    postalZipCode: this.shippingPostalZipCodeCtrl,
    provinceOrState: this.shippingProvinceOrStateCtrl
  })



  private token: any;
  private requestInfo: any;
  countries: Array<any> = ["Canada", "United States"];
  states: Array<any> = ["AK - Alaska",
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
      let userResponse = await axios.get(this.appComponent.apiHost + "/Addresses", this.requestInfo);
      console.log(userResponse.data);
      this.storedBillingAddress = userResponse.data.billingAddress;
      this.storedShippingAddress = userResponse.data.shippingAddress;
      this.setBillingInfo();
      this.setShippingInfo();
    } catch (e) {
      this.errors.push(e);
    }
  }

  routeToProfile() {
    this.appComponent.navigate("/profile");
  }

  setBillingInfo() {
    if (this.storedBillingAddress != null) {
      this.billingHomeAddressCtrl.setValue(this.storedBillingAddress.streetAddress);
      this.billingHomeAddress2Ctrl.setValue(this.storedBillingAddress.unitApt);
      this.billingCityCtrl.setValue(this.storedBillingAddress.city);
      this.billingPostalZipCodeCtrl.setValue(this.storedBillingAddress.postalZipCode);
      this.billingPhoneNumCtrl.setValue(this.storedBillingAddress.phone);
      this.billingCountryCtrl.setValue(this.storedBillingAddress.country);
      this.billingProvinceOrStateCtrl.setValue(this.storedBillingAddress.provinceState);
    }
  }

  setShippingInfo() {
    if (this.storedShippingAddress != null) {
      this.shippingHomeAddressCtrl.setValue(this.storedShippingAddress.streetAddress);
      this.shippingHomeAddress2Ctrl.setValue(this.storedShippingAddress.unitApt);
      this.shippingCityCtrl.setValue(this.storedShippingAddress.city);
      this.shippingPostalZipCodeCtrl.setValue(this.storedShippingAddress.postalZipCode);
      this.shippingPhoneNumCtrl.setValue(this.storedShippingAddress.phone);
      this.shippingCountryCtrl.setValue(this.storedShippingAddress.country);
      this.shippingProvinceOrStateCtrl.setValue(this.storedShippingAddress.provinceState);
      this.showShipping = true;
    }
    else{
      this.showShipping = false;
    }
  }

  async attemptSubmit() {
    if (this.billingAddressGroup.valid) {
      try {
        let billingObj: any = {
          streetAddress: this.billingHomeAddressCtrl.value,
          unitApt: this.billingHomeAddress2Ctrl.value,
          country: this.billingCountryCtrl.value,
          phone: this.billingPhoneNumCtrl.value,
          city: this.billingCityCtrl.value,
          postalZipCode: this.billingPostalZipCodeCtrl.value,
          provinceState: this.billingProvinceOrStateCtrl.value
        }

        let shippingObj: any = {
          streetAddress: this.shippingHomeAddressCtrl.value,
          unitApt: this.shippingHomeAddress2Ctrl.value,
          country: this.shippingCountryCtrl.value,
          phone: this.shippingPhoneNumCtrl.value,
          city: this.shippingCityCtrl.value,
          postalZipCode: this.shippingPostalZipCodeCtrl.value,
          provinceState: this.shippingProvinceOrStateCtrl.value
        }

        let billingCall = await axios.put(this.appComponent.apiHost + "/BillingAddress", billingObj, this.requestInfo);
        let shippingCall: any;

        if(this.showShipping == true){
          console.log(shippingObj);
          shippingCall = await axios.put(this.appComponent.apiHost + "/ShippingAddress", shippingObj, this.requestInfo);
        }

        if(this.showShipping == true){
          if(billingCall.status == 200 && shippingCall.status == 200){
            this.success = true;
          }
        }
        else{
          if(billingCall.status == 200){
            this.success = true;
          }
        }
      }
      catch (err: any) {
        console.error(err);
      }
    }
  }

  async deleteSavedBillingAddress() {
    try{
      let deleteBillCall = await axios.delete(this.appComponent.apiHost + "/BillingAddress", {
        headers: {
          Authorization: "Bearer " + this.token
        }
      });
      console.log(deleteBillCall);
    }
    catch(err: any){
      this.errors.push(err);
      console.error(err);
    }
  }

  revealShipping() {
    if(this.showShipping == true){
      this.showShipping = false;
    }
    else{
      this.showShipping = true;
    }
  }

  async deleteSavedShippingAddress() {
    try{
      let deleteShipCall = await axios.delete(this.appComponent.apiHost + "/ShippingAddress", {
        headers: {
          Authorization: "Bearer " + this.token
        }
      });
      console.log(deleteShipCall);
    }
    catch(err: any){
      this.errors.push(err);
      console.error(err);
    }
  }
}
