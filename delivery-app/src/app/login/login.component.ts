import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
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
    username : new FormControl(),
    password : new FormControl()
  }) 

  constructor(private api: ApiService, private toastr: ToastrService, private router: Router) { }


  ngOnInit(): void {
  }

  submitLogin(){
    let model = new LoginModel();

    model.username = this.formGroupLogin.get('username')?.value;
    model.password = this.formGroupLogin.get('password')?.value;


    this.api.login(model).subscribe(
      data =>{
          localStorage.setItem('token', data);
      },
      error=>{
        this.toastr.error("Wrong username or password!");
        this.router.navigateByUrl("/login");
      }
      
    )
  }

}
