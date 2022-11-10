import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-payment-page',
  templateUrl: './payment-page.component.html',
  styleUrls: ['./payment-page.component.scss']
})
export class PaymentPageComponent implements OnInit {

  creditCardNumCtrl: FormControl = new FormControl(null, [Validators.required,
    Validators.pattern("^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$")]);
  expiryCtrl: FormControl = new FormControl(null, [Validators.required, Validators.pattern(`^[0-9][0-9][/][0-9][0-9]$`)]);

  private token: any;
  private requestInfo: any;

  paymentGroup = new FormGroup({
    creditCardNum: this.creditCardNumCtrl,
    expiry: this.expiryCtrl
  });

  constructor(
    private appComponent: AppComponent
  ) { }

  ngOnInit(): void {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };
  }

  navigateProfile() {
    this.appComponent.navigate('profile');
  }

  async submitPaymentInfo() {
    if (!this.paymentGroup.valid)
      return;

    let requestBody = {
      "creditCardNumber": this.creditCardNumCtrl.value,
      "expiryDate": this.expiryCtrl.value
    }
    console.log(JSON.stringify(requestBody));
    try {
      await axios.post("https://localhost:7105/PaymentInfo", requestBody, this.requestInfo);

      this.appComponent.navigate('store');
    } catch (error) {
      console.log(error);
    }
  }

}
