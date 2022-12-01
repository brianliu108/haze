import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import axios from 'axios';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-approve-user-reviews',
  templateUrl: './approve-user-reviews.component.html',
  styleUrls: ['./approve-user-reviews.component.scss']
})
export class ApproveUserReviewsComponent implements OnInit {

  private token: any;
  userData: any;
  private requestInfo: any;

  shownReviews: Array<any> = [];

  constructor(
    private appComponent: AppComponent
  ) { }

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    if (!this.token)
      return this.appComponent.navigate("");
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };

    this.getReviews();
    this.userData = JSON.parse(localStorage.getItem("currentUser")!);
  }

  async getReviews() {
    try {
      let getReviewsCall = await axios.get(this.appComponent.apiHost + "/ProductReviews", this.requestInfo);

      if (getReviewsCall.status == 200) {
        this.shownReviews = JSON.parse(JSON.stringify(getReviewsCall.data));
        console.log(this.shownReviews);
      }
    }
    catch (err: any) {
      console.error(err);
    }
  }

  async aproveUserReview(item: any) {
    try {
      const createResponse = await axios.post('https://localhost:7105/ApproveProductReview/' + item.productId + "/" + item.id, null, this.requestInfo);

      setTimeout(() => {
        this.getReviews();
      }, 500);
      
    }
    catch (error: any) {
      console.error(error);
    }
  }

  routeToStore() {
    this.appComponent.navigate("/reviews");
  }

  async disAproveUserReview(item: any) {
    try{
      let deleteReviewCall = await axios.delete(this.appComponent.apiHost + "/ProductReviews/" + item.id, {
        headers: {
          Authorization: "Bearer " + this.token
        }
      });
      setTimeout(() => {
        this.getReviews();
      }, 500);
    }
    catch(err: any){
      console.error(err);
    }
  }

}