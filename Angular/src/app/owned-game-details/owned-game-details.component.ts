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

  enableReviewSection: boolean = true;

  reviewsList: Array<any> = [];

  noRatings: boolean = false;
  gameRating: Number;

  badReviewSubmission: boolean = false;

  reviewSubmitted: boolean = false;
  rateSubmitted: boolean = false;

  reviewCtrl: FormControl = new FormControl(null, [Validators.required, Validators.minLength(10)]);
  rate1Ctrl: FormControl = new FormControl(null, Validators.required);
  reviewGroup: FormGroup = new FormGroup({
    review: this.reviewCtrl,
    rating: this.rate1Ctrl
  });

  rate2Ctrl: FormControl = new FormControl(null, Validators.required);
  rateGroup: FormGroup = new FormGroup({
    rating: this.rate2Ctrl
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

    this.getReviews();
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

  downloadGame() {
    let stringData = this.generateStringData();
    let blob = new Blob(['\ufeff' + stringData], {type: 'text/plain'});
    let dwldLink = document.createElement("a");
    let url = URL.createObjectURL(blob);
    let isSafariBrowser = navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1;
    if (isSafariBrowser) {  //if Safari open in new window to save file with random filename.
      dwldLink.setAttribute("target", "_blank");
    }
    dwldLink.setAttribute("href", url);
    dwldLink.setAttribute("download", this.selectedGame.productName);
    dwldLink.style.visibility = "hidden";
    document.body.appendChild(dwldLink);
    dwldLink.click();
    document.body.removeChild(dwldLink);
  }

  generateStringData() {
    let str = ''
    str += this.selectedGame.productName + '\n';
    str += 'Description: ' + this.selectedGame.description + '\n';
    return str;
  }

  async makeRatingCall(){
    if(this.rate2Ctrl.valid){
      try {
        const rateCallResponse = await axios.post('https://localhost:7105/AddProductRating' + '/' + this.selectedGame.id + '/' + this.rate2Ctrl.value, null, this.requestInfo);
  
        if (rateCallResponse.status = 200) {
          this.showReviewSuccess();
        }
      }
      catch (error: any) {
        console.error(error);
      }
    }
  }

  async makeReviewCall(){
    if(this.reviewGroup.valid){
      try {
        const reviewCallResponse = await axios.post('https://localhost:7105/ProductReviews' + '/' + this.selectedGame.id + '/' + this.reviewCtrl.value + '/' + this.rate1Ctrl.value, null, this.requestInfo);
  
        if (reviewCallResponse.status = 200) {
          this.showReviewSuccess();
        }
      }
      catch (error: any) {
        console.error(error);
      }
    }
    else{
      this.showBadReview();
    }
  }

  ratingCanceller(){
    this.rate1Ctrl.reset();
    this.rate2Ctrl.reset();
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
      window.location.reload();
    }, 3000);
  }

  showBadReview(){
    this.badReviewSubmission = true;
    setTimeout(() => {
      this.badReviewSubmission = false;
    }, 3000);
  }

  showRateSuccess(){
    this.rateSubmitted = true;
    setTimeout(() => {
      this.rateSubmitted = false;
      window.location.reload();
    }, 3000);
  }

  async getReviews(){
    try {
      const reviewResponse = await axios.get('https://localhost:7105/ProductReviews/' + this.selectedGame.id, this.requestInfo);
      console.log(reviewResponse.data);
      if (reviewResponse.status = 200) {
        this.reviewsList = reviewResponse.data;
        this.checkReviews(reviewResponse.data);
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
      let ignoreCounter = 0;
      for( let item of data){
        if(item.rating == 0){
          ignoreCounter++;
        }
        else{
          tempScore += item.rating;
        }
      }
      tempScore /= (data.length - ignoreCounter);
      this.gameRating = parseFloat(tempScore.toFixed(2));
    }
    else{
      this.noRatings = true;
    }
  }

  checkReviews(data: Array<any>){
    for(let item of data){
      if(item.user.username == atob(this.userData.username)){
        this.enableReviewSection = false;
      }
    }
  }
}
