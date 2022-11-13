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

  private token: any;
  private requestInfo: any;

  success: boolean = false;

  errors: Array<any> = [];

  platforms: Array<any>;
  categories: Array<any>;

  selectedPlatforms: Array<any> = [];
  selectedCategories: Array<any> = [];

  productNameCtrl: FormControl = new FormControl(null, Validators.required);
  categoryCtrl: FormControl = new FormControl(null);
  platformCtrl: FormControl = new FormControl(null);
  coverImgUrlCtrl: FormControl = new FormControl(null);
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

    this.getCategories();
    this.getPlatforms();
  }

  checkAdmin() {
    let userData = JSON.parse(localStorage.getItem("currentUser")!);
    if (userData.role != 'Admin') {
      this.appComponent.navigate('/store');
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

  showSuccess() {
    this.success = true;
  }

  showFailure() {
    this.errors.push("Unsuccessfull game creation, check inputs and try again");
  }

  routeToStore() {
    this.appComponent.navigate("/store");
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
    console.log(this.selectedCategories);
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
    console.log(this.selectedPlatforms);
  }

  async createGame() {
    if(this.gameGroup.valid){
      let gameInfo: any = {
        "productName": this.productNameCtrl.value,
        "categoryIds": this.selectedCategories,
        "platformIds": this.selectedPlatforms,
        "description": this.descriptionCtrl.value,
        "price": parseFloat(this.priceCtrl.value),
        "coverImgUrl": this.coverImgUrlCtrl.value
      }
      console.log(gameInfo);
      
      try {
        const createResponse = await axios.post('https://localhost:7105/Products', gameInfo, this.requestInfo);
  
        if (createResponse.status = 200) {
          console.log(createResponse);
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
}
