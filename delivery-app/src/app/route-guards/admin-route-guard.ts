import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";




@Injectable({providedIn: 'root'})
export class AdminRouteGuard implements CanActivate {

  constructor(public router: Router) {}
  
  canActivate(): boolean {
    
    let role = localStorage.getItem('role');

    if(role == 'admin'){
        return true;
    } else {
        this.router.navigateByUrl('/login');
        return false;
    }
  }
}