import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { RegisterModel } from '../models/DTO/register-model';
import { ApiService } from '../services/api-service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  formGroupRegister = new FormGroup({
    username : new FormControl(),
    email : new FormControl(),
    password1 : new FormControl(),
    password2 : new FormControl(),
    name : new FormControl(),
    surname : new FormControl(),
    dateOfBirth : new FormControl(),
    address : new FormControl(),
    imageUrl : new FormControl(),
    type : new FormControl(),
  })
  
  constructor(private api: ApiService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  submitRegister(){

    if(this.formGroupRegister.get('password1')?.value != this.formGroupRegister.get('password2')?.value){
      this.toastr.error("Lozinke se ne poklapaju!");
      this.router.navigateByUrl("/register");
    }

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
    
    this.formGroupRegister.get('username')?.errors['']
    this.api.register(model).subscribe();
  }

}
