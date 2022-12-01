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
  taxCost: number = 0;
  fullCost: number = 0;

  currentCards: Array<any> = [];
  currentCard: any = null;

  gamesPurchased: boolean = false;
  submitError: boolean = false;

  cvcCtrl: FormControl = new FormControl(null, [Validators.required, Validators.pattern(/^\d{3}$/)]);

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

    this.taxCost = this.cartCost * 0.13;
    this.fullCost = this.cartCost * 1.13;
  }


  async purchaseGames(){
    if(this.cvcCtrl.valid){
      for(let i: number = 0; i < this.gamesInCart.length; i++){
        try {
          const response = await axios.post('https://localhost:7105/UserLibrary/' + this.gamesInCart[i].id, null, this.requestInfo);
        } catch (error) {
          console.error(error);
        }
      }

      this.gamesPurchased = true;
      localStorage.removeItem('gamesInCart');
    }
    else{
      this.submitError = true;
      this.setSubmitError();
    }
  }

  setSubmitError(){
    setTimeout(() => {
      this.submitError = false;
    }, 3000)
  }
}
