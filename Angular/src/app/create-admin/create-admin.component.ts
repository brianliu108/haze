import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-create-admin',
  templateUrl: './create-admin.component.html',
  styleUrls: ['./create-admin.component.scss']
})
export class CreateAdminComponent implements OnInit {

  constructor(
    private appComponent: AppComponent) { }

    private token:any;
    private requestInfo:any;

  errors: Array<any> = [];

  ngOnInit(): void {
    this.checkAdmin();

    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };
  }

  success: boolean = false;
  
  firstNameCtrl = new FormControl(null, Validators.required);
  lastNameCtrl = new FormControl(null, Validators.required);
  passwdCtrl = new FormControl(null, [Validators.pattern(/(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}/), Validators.required]);
  birthDateCtrl = new FormControl(null, [Validators.required]);

  registerGroup: FormGroup = new FormGroup({ 
    firstName: this.firstNameCtrl, 
    lastName: this.lastNameCtrl,
    password: this.passwdCtrl,
    birthDate: this.birthDateCtrl
  });

  async attemptCreate() {
    if (this.registerGroup.valid) {
      try {
        let registerInfo = { 
          firstName: this.firstNameCtrl.value, 
          lastName: this.lastNameCtrl.value,
          password: this.passwdCtrl.value,
          birthDate: this.birthDateCtrl.value
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


  checkAdmin(){
    let userData = JSON.parse(localStorage.getItem("currentUser")!);
    if(userData.role != 'Admin'){
      this.appComponent.navigate('/store');
    }
  }
}
