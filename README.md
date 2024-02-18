# tinyPlatform 0.20-beta
Free and open source simple Web IoT platform for data acquisition, remote control and online monitoring, where you can register devices, creates widgets and monitors and share them to other users.

## Demo video
[Short video presentation](https://youtu.be/jkAjIlBb3UU)

## Demo version
[Demo tinyPlaform](https://demo.tinyplat.com)
  
## Docs
[Documentation](https://docs.tinyplat.com)
  
# How it works
tinyPlatform is cross-platform, and it can be run it on Windows, Linux, or Mac.\
It runs on .NET 8 and supports MSSQL or Postgres database. (Device dispatcher use SQLite as a data storage).\
The solution has three main components:
 - Hub (ASP.Net Core aplication);
 - Client (Blazor Webassebmly standalone application);
 - Device dispatcher (ASP.Net Core application).

The platform has a classic client-server scheme:\
[General scheme](https://docs.tinyplat.com/images/general_scheme.png)

## Communication
As shown in the picture above:
 - сommunication between Hub (server) and Device dispatchers (clients) based on gRPC (unary calls, client stream and duplex stream) and WebAPI for auth.
 - сommunication between Hub (server) and Web clients based on gRPC (unary calls) and SignalR for online monitoring and server notifications and WebAPI for auth and some functions.
 - сommunication between Device dispather and data source handlers based on WebAPI and your data source handlers can use a http client.

## About the concept of "Device"
"Device" is an abstraction that describes a logic unit that contains one Device dispatcher and its data sources ("Sensors"). Data source handlers are not a part of the platform and can be created by you using any technology stack (e.g. C, C++, Phyton and etc.) in your trusted environment, ensuring the exchange of data and commands with Device dispatcher via WebAPI. A physical device (e.g. a controller or a computer) can have some dispatchers and some handlers.\
"Sensor" is an abstraction that describes a logic unit of the data source. It can be a scalar data source (e.g. a temperature sensor) or a complex data source (like a GNNS sensor) or a media data sorce (like a IP cam) or your custom complex data source that provides scalar, binary and/or JSON format data. 

## Security
All admin functions has its own admin area and available only platform administrators. Access to administration is provided only via the hub administration panel not via the client (Tiny dashboard).\
The client (Tiny dashboard) has some admin functions for admin roles. A registered malicious user cannot harm because access to the hub methods divided by user roles, user data scopes and all methods have its own access levels based on roles and/or permissions and have verification logic in method implementations.

## Data providers
Currently the platform hub uses EF Core and supports two data providers: MS SQL Server and Postgres.\
To reduce database requests the hub and client use caching.

## About livescheme widget
Unlike other widget types the livescheme widgets can be created by platform users and can have inner logic to handle incomming live data (see "Heartbeat monitor" in the demo). This logic is implemented using javascript code in the livescheme svg file. This approach provides a powerful tool to create rich HMI and UI for any scenarios but have security risks. Do not load the livescheme widgets from unreliable sources, do check livescheme code before granting access. 

## Some other features
 - note that all client's tables and dropdown lists have filters. Use it.
 - you can control the number of connections for each account (the number of working browser tabs with the client application).
 - the rate limiter service is implemented. It allows to control request frequency
 - authorization is based on openid and has custom implementation (not Duende)
 - for user interaction the platform has public pages and they can be used as a platform website. It is adjusted with the hub admin panel and can contain static pages, forums, blogs, news, polls, newsletter and mail compaign.

