import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-cart-checkout',
  templateUrl: './cart-checkout.component.html',
  styleUrls: ['./cart-checkout.component.scss']
})
export class CartCheckoutComponent implements OnInit {

  constructor(
    private appComponent: AppComponent
  ) { }

  gamesInCart: Array<any>; 
  private token: any;
  private requestInfo: any;

  cartCost: number = 0;

  currentCards: Array<any> = [];
  currentCard: any = null;

  cvcCtrl: FormControl = new FormControl(null, [Validators.required, Validators.pattern(/[1-9]\d\d/)]);

  ngOnInit(): void {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };

    this.gamesInCart = JSON.parse(localStorage.getItem('gamesInCart') || "[]");

    this.getCards();
    this.calculateCost();
  }

  routeToDetails(item: any) {
    localStorage.setItem('selectedGame', JSON.stringify(item));
    this.appComponent.navigate('/gameDetails');
  }

  async getCards() {
    try {
      const response = await axios.get('https://localhost:7105/PaymentInfo', this.requestInfo);
      this.currentCards = response.data;
    } catch (error) {
      console.error(error);
    }
  }

  setCurrentCard(item: any){
    this.currentCard = item;
  }

  calculateCost(){
    for(let i: number = 0; i < this.gamesInCart.length; i++){
      this.cartCost += this.gamesInCart[i].price;
    }
  }
}
