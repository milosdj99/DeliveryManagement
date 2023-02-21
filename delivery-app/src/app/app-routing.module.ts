import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddArticleComponent } from './add-article/add-article.component';
import { DashboardAdminComponent } from './dashboard-admin/dashboard-admin.component';
import { DashboardCustomerComponent } from './dashboard-customer/dashboard-customer.component';
import { DashboardDelivererComponent } from './dashboard-deliverer/dashboard-deliverer.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AdminRouteGuard } from './route-guards/admin-route-guard';
import { CustomerRouteGuard } from './route-guards/customer-route-guard';
import { DelivererRouteGuard } from './route-guards/deliverer-route-guard';

const routes: Routes = [
  {path:'', redirectTo:'login', pathMatch:'full'},
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'dashboard/customer', component: DashboardCustomerComponent, canActivate: [CustomerRouteGuard]},
  {path: 'dashboard/deliverer', component: DashboardDelivererComponent, canActivate: [DelivererRouteGuard]},
  {path: 'dashboard/admin', component: DashboardAdminComponent, canActivate: [AdminRouteGuard]},
  {path: 'add-article', component: AddArticleComponent, canActivate: [AdminRouteGuard]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
