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
  private token:any;
  private requestInfo:any;

  public updatingCard: boolean = false;

  creditCardNumCtrl: FormControl = new FormControl();

  profileGroup: FormGroup;

  currentCards: Array<any>;

  constructor(
    private appComponent: AppComponent
  ) { }

  updatingCreditCard(item: any){
    console.log(this.updatingCard);
    if(this.updatingCard == false){
      this.updatingCard = true;
    }
    else{
      this.updatingCard = false;
    }
    console.log(this.updatingCard);

    this.updateCardCall(item);
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
      //console.log(userProfile.data)

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

    await axios.put("https://localhost:7105/UserProfile", requestBody,this.requestInfo);
    
    this.appComponent.navigate('store');
  }

  navigatePayment() {
    this.appComponent.navigate("payment");
  }
  
  async getCards(){
    try {
      const response = await axios.get('https://localhost:7105/PaymentInfo', this.requestInfo);
      this.currentCards = response.data;
      console.log(this.currentCards);
    } catch (error) {
      console.error(error);
    }
  }

  async updateCardCall(item: any){
    console.log(item.creditCardNumber);
    let updatedInfo:any = item;

    /*
    try {
      const response = await axios.put('https://localhost:7105/PaymentInfo', this.requestInfo);
      this.currentCards = response.data;
      console.log(this.currentCards);
    } catch (error) {
      console.error(error);
    }
    */
  }

  submitUpdateCall(){
    console.log(this.creditCardNumCtrl.value + "  Called");
  }
}

