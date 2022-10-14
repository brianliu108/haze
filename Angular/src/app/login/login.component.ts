import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(
    private router: Router,
    private appComponent: AppComponent
  ) { }

  ngOnInit(): void {
  }

  routeToRegister(){
    /*
    console.log("called");
    this.router.navigate(['/register']);
    */

    this.appComponent.navigate("register");
  }

  attemptLogin(){
    this.appComponent.navigate("store");
  }
}
