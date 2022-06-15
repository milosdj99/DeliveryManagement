import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { RegisterModel } from '../models/DTO/register-model';
import { ApiService } from '../services/api-service';
import  jwt_decode from 'jwt-decode';
import { JSDocComment } from '@angular/compiler';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  requiredError = false;
  emailError = false;
  passwordError = false;
  apiError = false;
  isLoggedIn = false;

  formGroupRegister = new FormGroup({
    username : new FormControl("", Validators.required),
    email : new FormControl("", [Validators.required, Validators.email]),
    password1 : new FormControl("", Validators.required),
    password2 : new FormControl("", Validators.required),
    name : new FormControl("", Validators.required),
    surname : new FormControl("", Validators.required),
    dateOfBirth : new FormControl("", Validators.required),
    address : new FormControl("", Validators.required),
    imageUrl : new FormControl("", Validators.required),
    type : new FormControl("", Validators.required),
  })
  
  constructor(private api: ApiService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {

      this.isLoggedIn = localStorage.getItem('isLoggedIn') == 'true' ? true : false;
      this.requiredError = false;
      this.emailError = false;
      this.passwordError = false;
      this.apiError = false;


      if(this.isLoggedIn){


        this.api.getUserById().subscribe(
          data =>{
            this.formGroupRegister.get('username')?.patchValue(data.username);
            this.formGroupRegister.get('email')?.patchValue(data.email);
            this.formGroupRegister.get('name')?.patchValue(data.name);
            this.formGroupRegister.get('surname')?.patchValue(data.surname);
            this.formGroupRegister.get('dateOfBirth')?.patchValue(data.dateOfBirth);
            this.formGroupRegister.get('address')?.patchValue(data.address);
            this.formGroupRegister.get('imageUrl')?.patchValue(data.imageUrl);
            this.formGroupRegister.get('type')?.patchValue(data.type);

                }
        )
      }
    }
      
  

  submitRegister(){

    this.requiredError = false;
    this.emailError = false;
    this.passwordError = false;
    this.apiError = false;
    


   if(this.formGroupRegister.get('username')?.errors?['required']:""
    || this.formGroupRegister.get('password1')?.errors?['required']:""
    || this.formGroupRegister.get('password2')?.errors?['required']:""
    || this.formGroupRegister.get('name')?.errors?['required']:""
    || this.formGroupRegister.get('surname')?.errors?['required']:""
    || this.formGroupRegister.get('dateOfBirth')?.errors?['required']:""
    || this.formGroupRegister.get('address')?.errors?['required']:""
    || this.formGroupRegister.get('imageUrl')?.errors?['required']:""){
        this.requiredError = true;
    }
    

    if(this.formGroupRegister.get('password1')?.value != this.formGroupRegister.get('password2')?.value){
      this.passwordError = true;
    }

    if(this.formGroupRegister?.get('email')?.errors?['email']: ""){
      this.emailError = true;
    }

    if(this.requiredError || this.emailError || this.passwordError){

      this.router.navigateByUrl("/register");

    } else {

      let model = new RegisterModel();

      model.username = this.formGroupRegister.get('username')?.value;
      model.name = this.formGroupRegister.get('name')?.value;
      model.surname = this.formGroupRegister.get('surname')?.value;
      model.email = this.formGroupRegister.get('email')?.value;
      model.dateOfBirth = new Date();
      model.address = this.formGroupRegister.get('address')?.value;
      model.imageUrl = this.formGroupRegister.get('imageUrl')?.value;
      model.type = this.formGroupRegister.get('type')?.value;
      model.password = this.formGroupRegister.get('password1')?.value;
         
      if(this.isLoggedIn){
          this.api.register(model).subscribe(
            error => {
                this.apiError = true;
                this.router.navigateByUrl("/register");
            }
          );

          this.router.navigateByUrl("/login");
      } else {
          this.api.changeProfile(model);
      }
    }

    
  }

}
