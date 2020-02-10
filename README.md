# Resale Shop Service
REST service for resale shop with phones and mobile app for managing items in database.

Simple shop management system comprised of a mobile app and a back-end server-side application. The purpose of the system is to keep track of products that are delivered to the shop and departed to customers. The system provide information about the amount of each product available in stock. The mobile app is meant to be used by shop employees and managers.

The application distinguish two user roles:

    * warehouse manager - with access to all operations,
    * warehouse employee - with access to all operations except removing products.

Access control is based on OAuth 2.0 and OpenID Connect protocols. The application obtain users identity from two sources:

    * a 3rd party identity provider - Google ("Login with Google" functionality),
    * a 1st party identity provider integrated with server-side application.

The mobile app is also enhanced with an ability to store data about tracked products in the local memory of a mobile device. All operations on products are available regardless of the availability of a network connection. Single user can run the mobile app on multiple devices (e.g. on a smartphone and on a tablet). Each device can store app data in its local memory and can be used offline to modify the local data. Because of that synchronization algorithm handle possible data conflicts without misrepresenting the data.

Service is made with ASP.NET and mobile app with Xamarin.

