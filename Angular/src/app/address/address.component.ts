import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.scss']
})
export class AddressComponent implements OnInit {
  errors: Array<any> = [];
  streetAddressCtrl = new FormControl(null, Validators.required);
  // billingAddressCtrl = new FormControl(null);

  private token:any;
  private requestInfo:any;
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

}
