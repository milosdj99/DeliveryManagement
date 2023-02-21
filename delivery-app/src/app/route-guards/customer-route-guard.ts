import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";




@Injectable({providedIn: 'root'})
export class CustomerRouteGuard implements CanActivate {

  constructor(public router: Router) {}
  
  canActivate(): boolean {
    
    let role = localStorage.getItem('role');

    if(role == 'customer'){
        return true;
    } else {
        this.router.navigateByUrl('/login');
        return false;
    }
  }
}