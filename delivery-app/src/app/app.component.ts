import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'delivery-app';

  isLoggedIn : boolean = localStorage.getItem('isLoggedIn') == "true" ? true : false;

  constructor(private router: Router){ }


  ngOnInit(){
    this.isLoggedIn = localStorage.getItem('isLoggedIn') == "true" ? true : false;
  }

  logOut(){
    localStorage.setItem("isLoggedIn", "false");
    localStorage.setItem('role', "null");
    localStorage.removeItem('token');

    this.router.navigateByUrl("/login");
  }

}
