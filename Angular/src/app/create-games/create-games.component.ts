import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-create-games',
  templateUrl: './create-games.component.html',
  styleUrls: ['./create-games.component.scss']
})
export class CreateGamesComponent implements OnInit {

  constructor(    
    private appComponent: AppComponent) { }

  private token:any;
  private requestInfo:any;

  success: boolean = false;

  errors: Array<any> = [];

  productNameCtrl: FormControl = new FormControl(null, Validators.required);
  categoryCtrl: FormControl = new FormControl(null, Validators.required);
  platformCtrl: FormControl = new FormControl(null, [Validators.pattern(/(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}/), Validators.required]);
  descriptionCtrl: FormControl = new FormControl(null, [Validators.required]);
  priceCtrl: FormControl = new FormControl(null, Validators.required);

  gameGroup: FormGroup = new FormGroup({ 
    productName: this.productNameCtrl, 
    category: this.categoryCtrl,
    platform: this.platformCtrl,
    description: this.descriptionCtrl,
    price: this.priceCtrl
  });

  ngOnInit(): void {
    this.checkAdmin();

    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };
  }

  checkAdmin(){
    let userData = JSON.parse(localStorage.getItem("currentUser")!);
    if(userData.role != 'Admin'){
      alert("User does not have priveleges for this function!");
      this.appComponent.navigate('/store');
    }
  }

  async attemptCreate() {
    if (this.gameGroup.valid) {
      try {
        let registerInfo = { 
          productName: this.productNameCtrl, 
          category: this.categoryCtrl,
          platform: this.platformCtrl,
          description: this.descriptionCtrl,
          price: this.priceCtrl
        }
        const response = await axios.post('https://localhost:7105/RegisterAdmin', registerInfo, this.requestInfo);
        console.log(response);

        if (response.status = 200) {
          this.showSuccess();
          setTimeout(() => {
            this.routeToStore();
          }, 3000);
        }
        else if (response.status != 200) {
          this.showFailure();
        }
      } catch (error: any) {
        if (error.response.data.errors) {
          this.errors = error.response.data.errors;
        }
      }
    }
  }

  showSuccess() {
    this.success = true;
  }

  showFailure() {
    this.errors.push("Unsuccessfull admin registration attempt, please try again.");
  }

  routeToStore(){
    this.appComponent.navigate("/store");
  }
}
