import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-owned-game-details',
  templateUrl: './owned-game-details.component.html',
  styleUrls: ['./owned-game-details.component.scss']
})
export class OwnedGameDetailsComponent implements OnInit {

  errors: Array<any> = [];
  selectedGame: any;
  userData: any;

  reviewingGame: boolean = false;
  ratingGame: boolean = false;
  gameAddedToCart: boolean = false;

  showReviewBtns: boolean = false;
  showRatingBtns: boolean = false;

  reviewSubmitted: boolean = false;

  reviewCtrl: FormControl = new FormControl(null, Validators.required);
  reviewGroup: FormGroup = new FormGroup({
    review: this.reviewCtrl
  });

  rateCtrl: FormControl = new FormControl(null, Validators.required);
  rateGroup: FormGroup = new FormGroup({
    review: this.reviewCtrl
  });

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

    this.selectedGame = JSON.parse(localStorage.getItem('selectedOwnedGame')!);
    this.userData = JSON.parse(localStorage.getItem("currentUser")!);
  }

  routeToStore(){
    this.appComponent.navigate("store");
  }

  reviewingGameSetter(){
      this.reviewingGame = true;
      this.showReviewBtns = true;
      this.ratingGame = false;
      this.showRatingBtns = false;
  
  }

  ratingGameSetter(){

      this.ratingGame = true;
      this.showRatingBtns = true;
      this.reviewingGame = false;
      this.showReviewBtns = false;
  
  }

  async makeRatingCall(){

  }

  async makeReviewCall(){
    if(this.reviewCtrl.valid){
      try {
        const reviewCallResponse = await axios.post('https://localhost:7105/ProductReviews' + '/' + this.selectedGame.id + '/' + this.reviewCtrl.value, null, this.requestInfo);
  
        if (reviewCallResponse.status = 200) {
          this.showReviewSuccess();
        }
      }
      catch (error: any) {
        console.error(error);
      }
    }
    else{
      alert("bad");
    }
  }

  ratingCanceller(){
    this.rateCtrl.reset();
    this.reviewCtrl.reset();
    this.showRatingBtns = false;
    this.showReviewBtns = false;
    this.reviewingGame = false;
    this.ratingGame = false;
  }

  showReviewSuccess(){
    this.reviewSubmitted = true;
    setTimeout(() => {
      this.reviewSubmitted = false;
    }, 3000);
  }
}
