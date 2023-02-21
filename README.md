# DeliveryManagement
The "Delivery management" application was coded using ASP.NET CORE and Angular 13. It allows its users (specifically customers, deliverers and admins)
to operate the system, using different functionalities. The application itself relies on microservice architecture, having 4 components:
1. Frontend Angular client application
2. Gateway API which re-routes client requests towards micorservice APIs
3. User API which operates with user functionalities
4. Delivery API which operates with deliveries, articles, orders etc.

Every event on back-end is logged into console and into txt file via NLog. 
Authorization and autentification are achieved via JWT tokens.

(It is aknowledgeable that UI on client application lacks a lot of quality, have in mind that this is an old college project which was not updated 
due to lack of time. :(  )
