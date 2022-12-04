import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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
  wishlistGames: Array<any> = [];
  reviewsList: Array<any> = [];

  noRatings: boolean = false;
  gameRating: Number;

  selectedPlatforms: Array<any> = [];
  selectedCategories: Array<any> = [];

  platforms: Array<any> = [];
  categories: Array<any> = [];

  gamesInCart: Array<any> = [];

  editingGame: boolean = false;
  updatedGame: boolean = false;
  gameAddedToCart: boolean = false;

  gameNameCtrl: FormControl = new FormControl(null, Validators.required);
  categoryCtrl: FormControl = new FormControl(null);
  platformCtrl: FormControl = new FormControl(null);
  coverImgUrlCtrl: FormControl = new FormControl(null);
  descriptionCtrl: FormControl = new FormControl(null, [Validators.required]);
  priceCtrl: FormControl = new FormControl(null, Validators.required);
  updateGroup: FormGroup = new FormGroup({
    productName: this.gameNameCtrl,
    category: this.categoryCtrl,
    platform: this.platformCtrl,
    description: this.descriptionCtrl,
    price: this.priceCtrl
  })

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

    this.selectedGame = JSON.parse(localStorage.getItem('selectedGame')!);
    this.userData = JSON.parse(localStorage.getItem("currentUser")!);
    this.gamesInCart = JSON.parse(localStorage.getItem("gamesInCart") || "[]");

    this.getWishlist();
    this.getReviews();
  }

  addToCart(){
    console.log(this.gamesInCart);

    if(this.gamesInCart.length != 0){
      let isInCart: boolean = false;
      for(let i: number = 0; i < this.gamesInCart.length; i++){
        console.log(this.gamesInCart[i].productName);
        console.log(this.selectedGame.productName);
        if(this.gamesInCart[i].productName == this.selectedGame.productName){
          isInCart = true;
        }
      }
      
      if(isInCart == false){
        this.gamesInCart.push(this.selectedGame);
        this.gameAddedToCart = true;
        localStorage.setItem("gamesInCart", JSON.stringify(this.gamesInCart));
      }
      else{
        this.errors.push("Game is already in cart");
      }
    }
    else{
      this.gamesInCart.push(this.selectedGame);
      this.gameAddedToCart = true;
      localStorage.setItem("gamesInCart", JSON.stringify(this.gamesInCart));
    }
  }

  cancelEdits(){
    this.editingGame = false;
  }

  routeToStore() {
    this.appComponent.navigate('/store');
  }

  editGame(){
    this.editingGame = true;
    this.getCategories();
    this.getPlatforms();
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

  async getCategories() {
    try {
      const categoriesResponse = await axios.get('https://localhost:7105/Categories', this.requestInfo);

      if (categoriesResponse.status = 200) {
        this.categories = categoriesResponse.data;
      }
    }
    catch (error: any) {
      console.error(error);
    }
  }

  async getPlatforms() {
    try {
      const platformsResponse = await axios.get('https://localhost:7105/Platforms', this.requestInfo);

      if (platformsResponse.status = 200) {
        this.platforms = platformsResponse.data;
      }
    }
    catch (error: any) {
      console.error(error);
    }
  }

  addToCategories(item: any) {
    if (!this.selectedCategories.includes(item.id)) {
      this.selectedCategories.push(item.id);
    }
    else if (this.selectedCategories.includes(item.id)) {
      for (let i: number = 0; i < this.selectedCategories.length; i++) {
        if (this.selectedCategories[i] == item.id) {
          this.selectedCategories.splice(i, 1);
        }
      }
    }
  }

  addToPlatforms(item: any) {
    if (!this.selectedPlatforms.includes(item.id)) {
      this.selectedPlatforms.push(item.id);
    }
    else if (this.selectedPlatforms.includes(item.id)) {
      for (let i: number = 0; i < this.selectedPlatforms.length; i++) {
        if (this.selectedPlatforms[i] == item.id) {
          this.selectedPlatforms.splice(i, 1);
        }
      }
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
        this.isInWishlist = true;
      }
    }
  }

  async updateGame() {
    if(this.updateGroup.valid){
      let gameInfo: any = {
        "id": this.selectedGame.id,
        "productName": this.gameNameCtrl.value,
        "categoryIds": this.selectedCategories,
        "platformIds": this.selectedPlatforms,
        "description": this.descriptionCtrl.value,
        "price": parseFloat(this.priceCtrl.value),
        "coverImgUrl": this.coverImgUrlCtrl.value
      }

      try {
        const createResponse = await axios.put('https://localhost:7105/Products', gameInfo, this.requestInfo);
  
        if (createResponse.status = 200) {
          this.showSuccess();
          setTimeout(() => {
            this.routeToStore();
          }, 3000);
        }
        else{
          this.showFailure();
        }
      }
      catch (error: any) {
        console.error(error);
      }
    }
  }
  
  showSuccess() {
    this.updatedGame = true;
  }

  showFailure() {
    this.errors.push("Unsuccessfull game editing, check inputs and try again");
  }

  async getReviews(){
    try {
      const reviewResponse = await axios.get('https://localhost:7105/ProductReviews/' + this.selectedGame.id, this.requestInfo);
      console.log(reviewResponse.data);
      if (reviewResponse.status = 200) {
        this.reviewsList = reviewResponse.data;
        this.ratingGetter(reviewResponse.data);
      }
    }
    catch (error: any) {
      console.error(error);
    }
  }

  ratingGetter(data: Array<any>){
    if(data.length != 0){
      let tempScore = 0;
      for( let item of data){
        tempScore += item.rating;
      }
      tempScore /= data.length;
      this.gameRating = parseFloat(tempScore.toFixed(2));
    }
    else{
      this.noRatings = true;
    }
  }
}
