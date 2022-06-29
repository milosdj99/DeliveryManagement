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

  selectedImage: File;

  successfulChange = false;
  requiredError = false;
  emailError = false;
  passwordError = false;
  apiError = false;
  isLoggedIn = false;
  imageUrl : string = "";

  formGroupRegister = new FormGroup({
    username : new FormControl("", Validators.required),
    email : new FormControl("", [Validators.required, Validators.email]),
    password1 : new FormControl("", Validators.required),
    password2 : new FormControl("", Validators.required),
    name : new FormControl("", Validators.required),
    surname : new FormControl("", Validators.required),
    dateOfBirth : new FormControl("", Validators.required),
    address : new FormControl("", Validators.required),
    type : new FormControl("", Validators.required),
  })
  
  constructor(private api: ApiService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {

      this.isLoggedIn = localStorage.getItem('isLoggedIn') == 'true' ? true : false;
      this.requiredError = false;
      this.emailError = false;
      this.passwordError = false;
      this.apiError = false;
      this.successfulChange = false;


      if(this.isLoggedIn){


        this.api.getUserById().subscribe(
          data =>{
            this.formGroupRegister.get('username')?.patchValue(data.username);
            this.formGroupRegister.get('email')?.patchValue(data.email);
            this.formGroupRegister.get('name')?.patchValue(data.name);
            this.formGroupRegister.get('surname')?.patchValue(data.surname);
            this.formGroupRegister.get('dateOfBirth')?.patchValue(data.dateOfBirth.getDate);
            this.formGroupRegister.get('address')?.patchValue(data.address);
            //this.formGroupRegister.get('imageUrl')?.patchValue(data.imageUrl);
            this.formGroupRegister.get('type')?.patchValue(data.type);
            //this.imageUrl = " http://localhost:44312/" +  data.imageUrl.replace('\').replace('/');

            this.imageUrl = " http://localhost:44312/" +  "Resources/Images/Capture.png";

            //document.getElementById("slika")?.setAttribute("src", `https://localhost:44312/${data.imageUrl}`);
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
        return;
    }
    

    if(this.formGroupRegister.get('password1')?.value != this.formGroupRegister.get('password2')?.value){
      this.passwordError = true;
      return;
    }

    if(this.formGroupRegister?.get('email')?.errors?['email']: ""){
      this.emailError = true;
      return;
    }
   

      let model = new RegisterModel();

      model.id = "3DAE2E51-4DC8-4108-360F-08DA4FEA0904";
      model.username = this.formGroupRegister.get('username')?.value;
      model.name = this.formGroupRegister.get('name')?.value;
      model.surname = this.formGroupRegister.get('surname')?.value;
      model.email = this.formGroupRegister.get('email')?.value;
      model.dateOfBirth = new Date();
      model.address = this.formGroupRegister.get('address')?.value;
      model.imageUrl = this.formGroupRegister.get('imageUrl')?.value;
      model.type = this.formGroupRegister.get('type')?.value;
      model.password = this.formGroupRegister.get('password1')?.value;
         
      let idForPicChange = "";

      if(!this.isLoggedIn){
          this.api.register(model).subscribe(
            data => {
                

                let imageData = new FormData();
                imageData.append("image", this.selectedImage, this.selectedImage.name);
                this.api.changePicture(imageData, data.id).subscribe();

                this.router.navigateByUrl("/login");
            },
            error => {
                this.apiError = true;
            }
          );

          
      } else {
          this.api.changeProfile(model).subscribe(
            data => {
              //this.router.navigateByUrl("/login");
              this.successfulChange = true;

              let imageData = new FormData();
              imageData.append("image", this.selectedImage);
              this.api.changePicture(imageData, localStorage.getItem("id")).subscribe();

          },
            error => {
              this.apiError = true;
            }
          );
      }
    
      
  }


  onPicChange(event){
      this.selectedImage = <File>event.target.files[0];
  }

}
