import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LoginModel } from '../models/DTO/login-model';
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

  errorMessage = "";

  constructor(private api: ApiService, private toastr: ToastrService, private router: Router) { }


  ngOnInit(): void {
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

              this.router.navigateByUrl('/dashboard/customer');

          }else if(localStorage.getItem('role') == 'deliverer'){

              this.router.navigateByUrl('/dashboard/deliverer');

          } else if(localStorage.getItem('role') == 'admin') {
            
              this.router.navigateByUrl('/dashboard/admin');
          }

          
      },
      error=>{
        this.apiError = true;
        this.errorMessage = error.error;
      }
      
    )
  }

}
