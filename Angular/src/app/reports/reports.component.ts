import { Component, OnInit } from '@angular/core';
import axios from 'axios';
import { AppComponent } from '../app.component';
import jwt_decode from "jwt-decode";
import { decode } from 'querystring';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
  errors: Array<any> = []
  private token: any;
  private requestInfo: any;
  constructor(private appComponent: AppComponent) { }

  ngOnInit(): void {
    try {
      this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
      let decodedToken:any = jwt_decode(this.token);
      console.log(decodedToken);
      if (decodedToken.role != "Admin") 
        return this.appComponent.navigate("store")    
      if (!this.token)
        return this.appComponent.navigate("");
      this.requestInfo = {
        headers: {
          Authorization: "Bearer " + this.token
        }
      };
    } catch (e) {
      this.appComponent.navigate("")
    }
    
  }
  
  async generateGameListReport() {
    this.errors = []
    let headerList: Array<Array<string>> = [["id", ""], ["productName", ""], ["categories", "сategory", "name"], ["platforms", "platform", "name"], ["description", ""], ["price", ""], ["coverImgUrl", ""]];
    let headerDisplay: Array<string> = ["Product ID", "Product Name", "Categories", "Platforms", "Description", "Price", "Cover Image URL"];
    let fileName = "GameListReport.csv"
    try {
      let response = await axios.get(this.appComponent.apiHost + "/Products", this.requestInfo);
      this.downloadFile(response.data, headerList, headerDisplay, fileName);

    } catch (e) {
      this.errors.push(e);
    }

  }

  async generateMemberLibraryReport() {
    this.errors = []
    let headerList: Array<Array<string>> = [["gameTitle", ""], ["sales", ""]];
    let headerDisplay: Array<string> = ["Username", "Items in Library"];
    let fileName: string = "MemberLibraryReport.csv";
    try {
      let response = await axios.get(this.appComponent.apiHost + "/Reports/MemberLibrary", this.requestInfo)
      this.downloadFile(response.data, headerList, headerDisplay, fileName);
    } catch (e) {
      this.errors.push(e);
    }
  }

  async generateMemberListReport() {
    this.errors = []
    let headerList: Array<Array<string>> = [["email", ""], ["username", ""], ["firstName", ""], 
    ["lastName", ""], ["gender", ""], ["birthDate", ""], ["verified", ""], ["newsletter", ""], ["roleName", ""]];
    let headerDisplay: Array<string> = ["Email", "Username", "First Name", "Last Name", "Gender",
    "Birth Date", "Verified", "Newsletter", "Role Name"];
    let fileName: string = "MemberListReport.csv";
    try {
      let response = await axios.get(this.appComponent.apiHost + "/GetUsers", this.requestInfo)
      this.downloadFile(response.data, headerList, headerDisplay, fileName);
    } catch (e) {
      this.errors.push(e);
    }
  }

  async generateMemberFriendsReport() {
    this.errors = [];
    try {
      let headerList : Array<Array<string>> = [["username"], ["numberOfFriends"]];
      let headerDisplay: Array<string> = ["User", "Number of Friends"];
      let fileName: string = "MemberFriendsReports.csv";
      let response = await axios.get(this.appComponent.apiHost + "/Reports/MemberFriends", this.requestInfo)
      this.downloadFile(response.data, headerList, headerDisplay, fileName);
    } catch (e) {
      this.errors.push(e);
    }
  }

  async generateWishlistReport() {
    this.errors = []
    let headerList: Array<Array<string>> = [["productName", ""], ["numberOfTimesWishlisted", ""]];
    let headerDisplay: Array<string> = ["Product Name", "Number of Times Wishlisted"];
    let fileName: string = "WishListReport.csv";
    try {
      let response = await axios.get(this.appComponent.apiHost + "/Reports/Wishlist", this.requestInfo)
      this.downloadFile(response.data, headerList, headerDisplay, fileName);
    } catch (e) {
      this.errors.push(e);
    }
  }

  async generateSalesReport() {
    this.errors = []
    let headerList: Array<Array<string>> = [["gameTitle", ""], ["sales", ""]];
    let headerDisplay: Array<string> = ["Product Name", "Number of Sales"];
    let fileName: string = "SalesReport.csv";
    try {
      let response = await axios.get(this.appComponent.apiHost + "/Reports/ProductSales", this.requestInfo)
      this.downloadFile(response.data, headerList, headerDisplay, fileName);
    } catch (e) {
      this.errors.push(e);
    }
  }

  downloadFile(data: any, arrHeader: Array<any>, arrHeaderDisplay: Array<any>, fileName: string) {
    let csvData = this.convertToCSV(data, arrHeader, arrHeaderDisplay);
    console.log(csvData)
    let blob = new Blob(['\ufeff' + csvData], { type: 'text/csv;charset=utf-8;' });
    let dwldLink = document.createElement("a");
    let url = URL.createObjectURL(blob);
    let isSafariBrowser = navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1;
    if (isSafariBrowser) {  //if Safari open in new window to save file with random filename.
      dwldLink.setAttribute("target", "_blank");
    }
    dwldLink.setAttribute("href", url);
    dwldLink.setAttribute("download", fileName);
    dwldLink.style.visibility = "hidden";
    document.body.appendChild(dwldLink);
    dwldLink.click();
    document.body.removeChild(dwldLink);
  }

  convertToCSV(objArray: Array<object>, headerList: Array<string>, headerDisplay: Array<string>) {
    let delimiter = ';';
    let array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    let str = '';
    let row = ' ' + delimiter;
    str += 'sep=;\n'
    for (let index in headerDisplay) {
      row += headerDisplay[index] + delimiter;
    }
    row = row.slice(0, -1);
    str += row + '\r\n';
    for (let i = 0; i < array.length; i++) {
      let line = (i + 1) + '';
      for (let index in headerList) {
        let head = array[i][headerList[index][0]]
        console.log(head);
        if (!headerList[index][1])
          line += delimiter + head;
        else {
          line += delimiter                    
          if (Array.isArray(head)) {   
            let headArr: Array<any> = head;                     

            for (let subIndex = 0; subIndex < headArr.length; subIndex++) {
              const item = headArr[subIndex];
              line += item[headerList[index][1]][headerList[index][2]]
              if (subIndex < headArr.length - 1)
                line += ", "
            }
          }        
        }
      }
      str += line + '\r\n';
    }
    return str;
  }
}
