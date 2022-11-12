import { Component, OnInit } from '@angular/core';
import axios from 'axios';
import { AppComponent } from '../app.component';

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
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    if (!this.token)
      return this.appComponent.navigate("");
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };
  }
  
  async generateGameListReport() {
    this.errors = []
    let headerList: Array<Array<string>> = [["productName", ""], ["categories", "сategory", "name"], ["platforms", "platform", "name"], ["description", ""], ["price", ""], ["coverImgUrl", ""]];
    let headerDisplay: Array<string> = ["Product Name", "Categories", "Platforms", "Description", "Price", "Cover Image URL"];
    try {
      let response = await axios.get(this.appComponent.apiHost + "/Products", this.requestInfo);
      let responseJson = JSON.stringify(response.data)
      this.downloadFile(response.data, headerList, headerDisplay);

    } catch (e) {
      this.errors.push(e);
    }

  }

  async generateGameDetailReport() {
    this.errors = []
    this.errors.push('This will be implemented in an upcoming iteration')
  }

  async generateMemberListReport() {

  }

  async generateMemberDetailReport() {

  }

  async generateWishlistReport() {

  }

  async generateSalesReport() {

  }

  downloadFile(data: any, arrHeader: Array<any>, arrHeaderDisplay: Array<any>) {
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
    dwldLink.setAttribute("download", "sample.csv");
    dwldLink.style.visibility = "hidden";
    document.body.appendChild(dwldLink);
    dwldLink.click();
    document.body.removeChild(dwldLink);
  }

  convertToCSV(objArray: Array<object>, headerList: Array<string>, headerDisplay: Array<string>) {
    let delimiter = ';';
    let array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    let str = '';
    let row = 'Item No.' + delimiter;
    for (let index in headerDisplay) {
      row += headerDisplay[index] + delimiter;
    }
    row = row.slice(0, -1);
    str += row + '\r\n';
    for (let i = 0; i < array.length; i++) {
      let line = (i + 1) + '';
      for (let index in headerList) {
        let head = array[i][headerList[index][0]]
        if (!headerList[index][1])
          line += delimiter + head;
        else {
          line += delimiter                    
          if (Array.isArray(head)) {   
            let headArr: Array<any> = head;                     

            // for (let item of headArr) {              
            //   line += item[headerList[index][1]][headerList[index][2]] + ', '
            // }

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
