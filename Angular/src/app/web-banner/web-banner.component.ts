import { Component, OnInit } from '@angular/core';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-web-banner',
  templateUrl: './web-banner.component.html',
  styleUrls: ['./web-banner.component.scss']
})
export class WebBannerComponent implements OnInit {

  constructor(
    private appComponent: AppComponent
  ) { }

  gamesInCart: Array<any>; 
  cartSize: number = 0;

  ngOnInit(): void {
    this.checkIfCart();
  }

  navigateProfile(){
    this.appComponent.navigate("profile");
  }

  navigateStore(){
    this.appComponent.navigate("store");
  }

  routeToCart(){
    this.appComponent.navigate("/checkout");
  }

  navigateLogin(){
    localStorage.clear();
    this.appComponent.navigate("");
  }

  navigateSocial(){
    this.appComponent.navigate("social");
  }

  navigatePreferences(){
    this.appComponent.navigate("preferences");
  }

  checkIfCart(){
    this.gamesInCart = JSON.parse(localStorage.getItem('gamesInCart') || "[]");

    if(this.gamesInCart.length > 0){
      this.cartSize = this.gamesInCart.length;
    }
  }
}
