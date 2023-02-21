import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FacebookLoginProvider, SocialAuthService, SocialUser } from 'angularx-social-login';
import { ToastrService } from 'ngx-toastr';
import { LoginModel } from '../models/DTO/login-model';
import { RegisterModel } from '../models/DTO/register-model';
import { ApiService } from '../services/api-service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  formGroupLogin = new FormGroup({
    email : new FormControl("", [Validators.required, Validators.email]),
    password : new FormControl("", Validators.required)
  }) 

  apiError = false;
  requiredError = false;
  emailError = false;
  apiErrorF = false;
  errorMessage = "";

  constructor(private api: ApiService, private toastr: ToastrService, private router: Router, private socialAuthService : SocialAuthService) { }

  user: SocialUser = new SocialUser()
  isSignedin: boolean = false;

  ngOnInit(): void {
      this.socialAuthService.authState.subscribe((user) => {
      this.user = user;
      this.isSignedin = (user != null);
      console.log(this.user);
    }); 
  }

  facebookLogin(){
    this.apiError = false;
    this.requiredError = false;
    this.emailError = false;


    

    this.socialAuthService.signIn(FacebookLoginProvider.PROVIDER_ID).then(
      data => {
        let fuser = new RegisterModel();

        fuser.name = data.name;
        fuser.surname = data.lastName;
        fuser.email = data.email;
        fuser.id = "3DAE2E51-4DC8-4108-360F-08DA4FEA0904";
    
        this.api.facebookLogin(fuser).subscribe(
          data => {
            localStorage.setItem('token', data.value);
            localStorage.setItem('isLoggedIn', 'true');
    
            let decodedJWT = JSON.parse(window.atob(data.value.split('.')[1]));
            localStorage.setItem('id', decodedJWT.id);
            localStorage.setItem('role', decodedJWT.role);
            console.log(decodedJWT.role);
    
            this.router.navigateByUrl("dashboard/customer");
          }
        )
      },
      error => {
          this.apiErrorF = true;
      }
    )

    
    
  }

  submitLogin(){

    this.apiError = false;
    this.requiredError = false;
    this.emailError = false;


    if(this.formGroupLogin.get('email')?.errors?['email']:""){
      this.emailError = true;
      return;
    }


    if(this.formGroupLogin.get('email')?.errors?['required']:"" || this.formGroupLogin.get('password')?.errors?['required']:""){
        this.requiredError = true;
        return;
    }

    let model = new LoginModel();

    model.email = this.formGroupLogin.get('email')?.value;
    model.password = this.formGroupLogin.get('password')?.value;


    this.api.login(model).subscribe(
      data =>{
          localStorage.setItem('token', data.value);
          localStorage.setItem('isLoggedIn', 'true');

          let decodedJWT = JSON.parse(window.atob(data.value.split('.')[1]));
          localStorage.setItem('id', decodedJWT.id);
          localStorage.setItem('role', decodedJWT.role);

          if(localStorage.getItem('role') == 'customer'){

              this.router.navigateByUrl('dashboard/customer');

          }else if(localStorage.getItem('role') == 'deliverer'){

              this.router.navigateByUrl('dashboard/deliverer');

          } else if(localStorage.getItem('role') == 'admin') {
            
              this.router.navigateByUrl('dashboard/admin');
          }

          
      },
      error=>{
        this.apiError = true;
        this.errorMessage = error.error;
      }
      
    )
  }

}
