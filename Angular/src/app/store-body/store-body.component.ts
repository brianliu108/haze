import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-store-body',
  templateUrl: './store-body.component.html',
  styleUrls: ['./store-body.component.scss']
})
export class StoreBodyComponent implements OnInit {

  constructor() { }

  userData: any;

  ngOnInit(): void {
    this.userData = JSON.parse(localStorage.getItem("currentUser")!);
  }

}
