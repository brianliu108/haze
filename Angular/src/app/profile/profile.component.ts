import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  firstNameCtrl: FormControl = new FormControl();
  lastNameCtrl: FormControl = new FormControl();
  genderCtrl: FormControl = new FormControl();
  birthDateCtrl: FormControl = new FormControl();
  newsletterCtrl: FormControl = new FormControl();
  private token: any;
  private requestInfo: any;

  public updatingCard: boolean = false;

  creditCardNumCtrl: FormControl = new FormControl(null, [Validators.required,
  Validators.pattern("^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$")]);
  expiryCtrl: FormControl = new FormControl(null, [Validators.required, Validators.pattern(`^[0-9][0-9][/][0-9][0-9]$`)]);
  billingAddressCtrl: FormControl = new FormControl(null, Validators.required);
  shippingAddressCtrl: FormControl = new FormControl();

  profileGroup: FormGroup;

  currentCards: Array<any>;
  currentCard: any;

  constructor(
    private appComponent: AppComponent
  ) { }

  updatingCreditCard(item: any) {
    if (this.updatingCard == false) {
      this.updatingCard = true;
    }
    else {
      this.updatingCard = false;
    }

    this.currentCard = item;
  }

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };

    try {
      let userProfile = await axios.get("https://localhost:7105/UserProfile", this.requestInfo);
      let data = userProfile.data;

      this.firstNameCtrl = new FormControl(data.firstName, Validators.required);
      this.lastNameCtrl = new FormControl(data.lastName, Validators.required);
      this.genderCtrl = new FormControl(data.gender);
      this.birthDateCtrl = new FormControl(data.birthDate);
      this.newsletterCtrl = new FormControl(data.newsletter);

      this.profileGroup = new FormGroup({
        firstName: this.firstNameCtrl,
        lastName: this.lastNameCtrl,
        gender: this.genderCtrl,
        birthDate: this.birthDateCtrl,
        newsLetter: this.newsletterCtrl
      });
    } catch (err) {
      this.appComponent.navigate('store');
    }

    this.getCards();
  }

  async submitProfileChanges() {
    if (!this.profileGroup.valid)
      return;

    let requestBody = {
      email: "",
      password: "",
      roleName: "",
      userName: "",
      firstName: this.firstNameCtrl.value,
      lastName: this.lastNameCtrl.value,
      gender: this.genderCtrl.value,
      birthDate: this.birthDateCtrl.value,
      newsletter: this.newsletterCtrl.value
    };

    await axios.put("https://localhost:7105/UserProfile", requestBody, this.requestInfo);

    this.appComponent.navigate('store');
  }

  navigatePayment() {
    this.appComponent.navigate("payment");
  }

  async getCards() {
    try {
      const response = await axios.get('https://localhost:7105/PaymentInfo', this.requestInfo);
      this.currentCards = response.data;
    } catch (error) {
      console.error(error);
    }
  }

  async updateCardCall(item: any) {
    console.log(item.creditCardNumber);
    let updatedInfo: any = item;
  }

  async submitUpdateCall() {
    let updatedInfo: any = {
      "id": this.currentCard.id,
      "creditCardNumber": this.creditCardNumCtrl.value,
      "expiryDate": this.expiryCtrl.value,
      "billingAddress": this.billingAddressCtrl.value,
      "shippingAddress": this.shippingAddressCtrl.value
    };

    try {
      const response = await axios.put('https://localhost:7105/PaymentInfo', updatedInfo, this.requestInfo);

      if (response.status == 200) {
        alert("Update successful");
        this.getCards();
        this.updatingCard = false;
        this.currentCard = null;
      }
      else {
        alert("Update unsuccessful");
      }
    } catch (error) {
      console.error(error);
    }
  }

  cancelChanges() {
    this.updatingCard = false;
    this.currentCard = null;
  }

  async deleteCard(item: any) {
    try {
      const response = await axios.delete('https://localhost:7105/PaymentInfo', {
        data: item, headers: {
          Authorization: "Bearer " + this.token
        }
      });

      if (response.status == 200) {
        alert("Deletion successful");
        this.getCards();
      }
      else {
        alert("Deletion unsuccessful");
      }
    } catch (error) {
      console.error(error);
    }
  }
}

