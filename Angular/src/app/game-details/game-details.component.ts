import { Component, OnInit } from '@angular/core';
import axios, { Axios } from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-game-details',
  templateUrl: './game-details.component.html',
  styleUrls: ['./game-details.component.scss']
})
export class GameDetailsComponent implements OnInit {

  errors: Array<any> = [];
  selectedGame: any;
  userData: any;
  wishlistGames: Array<any>;

  isInWishlist: boolean;

  deleted: boolean = false;
  addedWishList: boolean = false;
  removedWishList: boolean = false;

  private token: any;
  private requestInfo: any;

  constructor(
    private appComponent: AppComponent) { }

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    if (!this.token)
      return this.appComponent.navigate("");
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };

    this.getWishlist();

    this.selectedGame = JSON.parse(localStorage.getItem('selectedGame')!);
    this.userData = JSON.parse(localStorage.getItem("currentUser")!);

  }

  routeToStore() {
    this.appComponent.navigate('/store');
  }

  async deleteGame() {
    try {
      const deleteResponse = await axios.delete('https://localhost:7105/Products' + '/' + this.selectedGame.id, {
        headers: {
          Authorization: "Bearer " + this.token
        }
      });

      if (deleteResponse.status == 200) {
        this.deleted = true;
        setTimeout(() => {
          this.appComponent.navigate('/store');
        }, 3000);
      }
    } catch (error) {
      console.error(error);
    }
  }

  async addToWishlist() {
    try {
      let gameObj: any = {
        "productId": this.selectedGame.id
      };

      const addWishlistResponse = await axios.post('https://localhost:7105/WishList', gameObj, this.requestInfo);

      if (addWishlistResponse.status == 200) {
        this.addedWishList = true;
      }
    } catch (error) {
      this.errors.push("Game already in wishlist!");
    }
  }

  async removeFromWishlist(){
    try {
      let itemId;
      for(let item of this.wishlistGames){
        if(item.product.id == this.selectedGame.id){
          console.log("wishlist id:" + item.product.id);
          console.log(this.selectedGame.id);
          //this.isInWishlist = true;
          itemId = item.id;
        }
      }

      const removeWishlistResponse = await axios.delete('https://localhost:7105/WishList' + '/' + itemId, this.requestInfo);

      if (removeWishlistResponse.status == 200) {
        this.removedWishList = true;
      }
    } catch (error) {
      this.errors.push("Game already removed from wishlist!");
    }
  }

  async getWishlist(){
    try {
      const getWishlistResponse = await axios.get('https://localhost:7105/WishList', this.requestInfo);

      if (getWishlistResponse.status == 200) {
        this.wishlistGames = getWishlistResponse.data;
        console.log(this.selectedGame);
        console.log(this.wishlistGames);
        this.checkIfInWishlist();
      }
    } catch (error) {
      //this.errors.push("Game already in wishlist!");
      console.error(error);
    }
  }

  checkIfInWishlist(){
    this.isInWishlist = false;
    for(let item of this.wishlistGames){
      if(item.product.id == this.selectedGame.id){
        console.log("wishlist id:" + item.product.id);
        console.log(this.selectedGame.id);
        this.isInWishlist = true;
      }
    }
  }
}
