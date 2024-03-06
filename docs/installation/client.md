# Dashboard client installation
  
tinyPlatform's Client ("Tiny dashboard", project Client.Dash) is Blazor Webassembly standalone application. Learn more about deploying applications of this kind in the [official documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly?view=aspnetcore-8.0#standalone-deployment) 
<br/>

## Nginx

First, deploy Hub as specified [here](/installation/hub.html). Nginx web site configuration file already has the Client configuration section as shown below:
:::code language="txt" source="nginx/nginx.conf" highlight="37-52"
<br />

Before Client project publising, prepare Client's appsettings.json as shown below:
:::code language="json" source="nginx/appsettings-client.json" highlight="3,5-7,9-10,15-19,23"

Publish project Client.Dash and place published files to "/var/www/client/dashboard/".\
Set needed rights: *"sudo chgrp -R www-data /var/www/client/"* and *"sudo chown -R www-data /var/www/client/"*\
Reload Nginx.\
Client has been deployed.
<br />

After Hub and Client deploying, start tinyPlatform. See video "Hub installation and quick start to get data".
> [!Video https://www.youtube.com/embed/jkAjIlBb3UU?si=nWUtV3DSfzRmkv5A]
<br />

## IIS
Chapter under writing...

