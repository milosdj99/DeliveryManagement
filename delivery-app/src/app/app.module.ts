import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthInterceptor } from './auth/AuthInterceptor';
import { LoginComponent } from './login/login.component';
import { ApiService } from './services/api-service';
import { RegisterComponent } from './register/register.component';
import { ToastrModule } from 'ngx-toastr';
import { NoopAnimationPlayer } from '@angular/animations';
import { DashboardCustomerComponent } from './dashboard-customer/dashboard-customer.component';
import { ArticleComponent } from './article/article.component';
import { OrderComponent } from './order-component/order.component';
import { DashboardDelivererComponent } from './dashboard-deliverer/dashboard-deliverer.component';
import { DashboardAdminComponent } from './dashboard-admin/dashboard-admin.component';
import { DelivererComponent } from './deliverer/deliverer.component';
import { AddArticleComponent } from './add-article/add-article.component';
import { FacebookLoginProvider, SocialAuthServiceConfig, SocialLoginModule } from 'angularx-social-login';
import { FacebookLoginComponent } from './facebook-login/facebook-login.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    DashboardCustomerComponent,
    ArticleComponent,
    OrderComponent,
    DashboardDelivererComponent,
    DashboardAdminComponent,
    DelivererComponent,
    AddArticleComponent,
    FacebookLoginComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    SocialLoginModule,
    ToastrModule.forRoot({
      progressBar: true
    }),
  ],
  providers: [ApiService,
      {
       provide: HTTP_INTERCEPTORS,
       useClass: AuthInterceptor,
       multi: true,
      },
      {
        provide: 'SocialAuthServiceConfig',
        useValue: {
          autoLogin: false,
          providers: [
            {
              id: FacebookLoginProvider.PROVIDER_ID,
              provider: new FacebookLoginProvider(
                '507606531160352'
              )
            }
          ]
        } as SocialAuthServiceConfig}],
      
  bootstrap: [AppComponent]
})
export class AppModule { }
