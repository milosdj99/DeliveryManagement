import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";




@Injectable({providedIn: 'root'})
export class DelivererRouteGuard implements CanActivate {

  constructor(public router: Router) {}
  
  canActivate(): boolean {
    
    let role = localStorage.getItem('role');

    if(role == 'deliverer'){
        return true;
    } else {
        this.router.navigateByUrl('/login');
        return false;
    }
  }
}