import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';

@Component({
  selector: 'app-create-admin',
  templateUrl: './create-admin.component.html',
  styleUrls: ['./create-admin.component.scss']
})
export class CreateAdminComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  success: boolean = false;
  
  usernameCtrl = new FormControl(null, Validators.required);
  emailCtrl = new FormControl(null, [Validators.required, Validators.email]);
  passwdCtrl = new FormControl(null, [Validators.pattern(/(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}/), Validators.required]);
  registerGroup: FormGroup = new FormGroup({ email: this.emailCtrl, passwd: this.passwdCtrl });

  async attemptCreate() {
    if (1) {
      try {
        let registerInfo = {
          "email": this.emailCtrl.value,
          "username": this.usernameCtrl.value,
          "password": this.passwdCtrl.value
        }
        const response = await axios.post('https://localhost:7105/Register', registerInfo);
        console.log(response.data)

        if (response.status = 200) {
          this.showSuccess();
          setTimeout(() => {
            this.routeToLogin();
          }, 3000);
        }
        else if (response.status != 200) {
          this.showFailure();
        }
      } catch (error: any) {
        if (error.response.data.errors) {
          // this.errors = error.response.data.errors;
          // console.log(this.errors);
        }
      }
    }
  }

  showSuccess() {
    this.success = true;
  }

  showFailure() {
    // this.errors.push("Unsuccessfull registration attempt, please try again.");
  }

  routeToLogin() {
    // this.appComponent.navigate("");
  }

}
