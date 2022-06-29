import { FacebookLoginProvider, SocialAuthService, SocialUser } from 'angularx-social-login';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../services/api-service';
import { RegisterModel } from '../models/DTO/register-model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-facebook-login',
  templateUrl: './facebook-login.component.html',
  styleUrls: ['./facebook-login.component.css']
})
export class FacebookLoginComponent implements OnInit {

  constructor(private socialAuthService : SocialAuthService, private apiService: ApiService, private router: Router) { }

  formGroupLogin = new FormGroup({
    email: new FormControl("", Validators.required),
    password: new FormControl("", Validators.required)
  })

  user: SocialUser = new SocialUser()
  isSignedin: boolean = false;

  ngOnInit(): void {
    this.socialAuthService.authState.subscribe((user) => {
      this.user = user;
      this.isSignedin = (user != null);
      console.log(this.user);
    });
  }


  submitLogin(){

    this.socialAuthService.signIn(FacebookLoginProvider.PROVIDER_ID);
    
    let fuser = new RegisterModel();

    fuser.name = this.user.name;
    fuser.surname = this.user.lastName;
    fuser.email = this.user.email;
    fuser.imageUrl = this.user.photoUrl;
    fuser.id = "3DAE2E51-4DC8-4108-360F-08DA4FEA0904";

    this.apiService.facebookLogin(fuser).subscribe(
      data => {
        localStorage.setItem('token', data.value);
        localStorage.setItem('isLoggedIn', 'true');

        let decodedJWT = JSON.parse(window.atob(data.value.split('.')[1]));
        localStorage.setItem('id', decodedJWT.id);
        localStorage.setItem('role', decodedJWT.role);

        this.router.navigateByUrl("dashboard/customer");
      }
    )
  }

  

}
