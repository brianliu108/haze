import { Component, OnInit, ɵallowSanitizationBypassAndThrow } from '@angular/core';
import { FormControl } from '@angular/forms';
import axios from 'axios';
import jwtDecode from 'jwt-decode';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-social',
  templateUrl: './social.component.html',
  styleUrls: ['./social.component.scss']
})
export class SocialComponent implements OnInit {

  image: string = "data:image/jpeg;base64,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";
  friends: Array<any>;
  private token: any;
  private requestInfo: any;
  users: Array<any> = [];
  decoded: any;
  incomingRequests: Array<any> = [];
  pendingRequests: Array<any> = [];
  username: any;
  searchCtrl: FormControl = new FormControl(null);

  constructor(
    private appComponent: AppComponent
  ) { }

  userData: any;

  async ngOnInit() {
    this.token = JSON.parse(localStorage.getItem("currentUser") || '{}').token
    this.decoded = jwtDecode(this.token);
    if (!this.token)
      return this.appComponent.navigate("");
    this.requestInfo = {
      headers: {
        Authorization: "Bearer " + this.token
      }
    };
    this.username = this.decoded.username;
    await this.getFriends();
    await this.getIncomingPendingRequests();
    await this.getUsers();

    this.userData = JSON.parse(localStorage.getItem("currentUser")!);

  }

  async getFriends() {
    try {
      let getFriendsCall = await axios.get(this.appComponent.apiHost +
        "/Friends", this.requestInfo);

      let test = getFriendsCall.data;

      if (getFriendsCall.status == 200) {
        this.friends = getFriendsCall.data;
        console.log(this.friends);
      }
    }
    catch (err: any) {
      console.error(err);
    }
  }

  enterProfile(item: any) {
    localStorage.setItem("selectedProfile", JSON.stringify(item));
    this.appComponent.navigate("socialProfile");
  }

  async getUsers() {
    try {
      let getUsersCall = await axios.get(this.appComponent.apiHost + "/getMembers", this.requestInfo);

      if (getUsersCall.status == 200) {
        this.users = getUsersCall.data;
      } else return;
      console.log(this.users);
      // Remove current friends from add friend
      let usersClone: Array<any> = [];
      for (let i = 0; i < this.users.length; i++)
        usersClone[i] = this.users[i];
      console.log(usersClone);
      for (const user of usersClone) {
        for (const friend of this.friends) {
          // console.log(friend);
          if (user.username == this.otherUser(friend).username) {
            let index = this.users.indexOf(user, 0);
            if (index !== -1) {
              this.users.splice(index, 1);
            }
          }
        }
      }
      // Remove incoming requests from add friend
      usersClone = [];
      for (let i = 0; i < this.users.length; i++)
        usersClone[i] = this.users[i];
      for (const user of usersClone) {
        for (const incomingRequest of this.incomingRequests) {
          if (user.username == this.otherUser(incomingRequest).username) {
            let index = this.users.indexOf(user, 0);
            if (index !== -1) {
              this.users.splice(index, 1);
            }
          }
        }
      }
      // Remove pending requests from add friend
      usersClone = [];
      for (let i = 0; i < this.users.length; i++)
        usersClone[i] = this.users[i];
      for (const user of usersClone) {
        for (const pendingRequest of this.pendingRequests) {
          if (user.username == this.otherUser(pendingRequest).username) {
            let index = this.users.indexOf(user, 0);
            console.log(this.users[index])
            if (index !== -1) {
              this.users.splice(index, 1);
            }
          }
        }
      }
      // Remove current user from add friend
      let index = this.users.indexOf(this.users.find(x => x.id == this.decoded.userId), 0);
      if (index !== -1) {
        this.users.splice(index, 1);
      }
    }
    catch (err: any) {
      console.error(err);
    }
  }

  async getIncomingPendingRequests() {
    try {
      let requests = await axios.get(this.appComponent.apiHost +
        "/Friends/Requests", this.requestInfo);
      this.incomingRequests = requests.data.incomingRequests;
      this.pendingRequests = requests.data.pendingRequests;
      console.log(this.incomingRequests);
    } catch (error) {
      console.error(error);
    }
  }

  async sendRequest(id: any) {
    try {
      let result = await axios.post(this.appComponent.apiHost + "/Friends/Requests/Send/" + id, null, this.requestInfo);

      window.location.reload();
    } catch (error) {
      console.error(error);
    }
  }

  async acceptRequest(id: any) {
    try {
      let result = await axios.post(this.appComponent.apiHost + "/Friends/Requests/Accept/" + id, null, this.requestInfo);

      window.location.reload();
    } catch (error) {
      console.error(error);
    }
  }

  async ignoreRequest(id: any) {
    try {
      let result = await axios.post(this.appComponent.apiHost + "/Friends/Requests/Ignore/" + id, null, this.requestInfo);

      window.location.reload();
    } catch (error) {
      console.error(error);
    }
  }

  async deleteFriend(id: any) {
    try {
      let result = await axios.delete(this.appComponent.apiHost + "/Friends/Requests/Delete/" + id, {
        headers: {
          Authorization: "Bearer " + this.token
        }
      });

      window.location.reload();
    } catch (error) {
      console.error(error);
    }
  }

  async requestFamily(id: any) {
    try {
      let result = await axios.post(this.appComponent.apiHost + "/Friends/Family/" + id, null, this.requestInfo);

      window.location.reload();
    } catch (error) {
      console.error(error);
    }
  }

  async removeFamily(id: any) {
    try {
      let result = await axios.delete(this.appComponent.apiHost + "/Friends/Family/" + id, {
        headers: {
          Authorization: "Bearer " + this.token
        }
      });

      window.location.reload();
    } catch (error) {
      console.error(error);
    }
  }

  showRequestFamily(record:any) {
    return !record.user1IsFamily && !record.user2IsFamily
  }

  showAcceptFamily(record:any) {
    let userNumber = 1;
    if (record.user2.id == this.decoded.userId)
      userNumber = 2;
    if (userNumber == 1)
      return record.user2IsFamily && !record.user1IsFamily;
    
    return record.user1IsFamily && !record.user2IsFamily;
  }

  showPendingFamily(record:any) {
    let userNumber = 1;
    if (record.user2.id == this.decoded.userId)
      userNumber = 2;
    if (userNumber == 1)
      return record.user1IsFamily && !record.user2IsFamily;

    return record.user2IsFamily && !record.user1IsFamily;
  }

  showRemoveFamily(record: any) {
    return record.user1IsFamily && record.user2IsFamily;
  }

  otherUser(friend: any) {
    if (friend.user1.username == this.decoded.username)
      return friend.user2;

    return friend.user1;
  }
}
