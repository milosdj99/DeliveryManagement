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

  apiError = false;

  constructor(private api: ApiService, private toastr: ToastrService, private router: Router) { }


  ngOnInit(): void {
  }

  submitLogin(){

    this.apiError = false;

    let model = new LoginModel();

    model.username = this.formGroupLogin.get('username')?.value;
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

          } else {
            //TO DO: admin
          }

          
      },
      error=>{
        this.router.navigateByUrl("/login");
      }
      
    )
  }

}
