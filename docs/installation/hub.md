# Hub installation (deployment)

Hub is ASP.Net Core server application. So it is cross-platform one and can be deploy on Window OS, MacOS or Linux. [More info](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-8.0)  
<br />

## Nginx

Prepare your Linux to deploy Hub:
* install Nginx
* install certs on Nginx *(e.g. via Let's Encrypt)*
* install Postgresql
* enable and configure Firewall and open needed ports (80, 443)
* publish the Hub project with your params *(e.g. "selfcontained")*
* place the published file on Linux *(e.g. in "/var/www/hub")*
* create some needed dirs: "sudo mkdir /var/www/hub/bin" and "sudo mkdir /var/www/hub/logs"
* set needed rights: "sudo chgrp -R www-data /var/www/hub" and "sudo chown -R www-data /var/www/hub"
<br />

Create a unit service file *(e.g. tinyhub.service)* as shown below and then register the unit service on Linux:
:::code language="txt" source="nginx/tinyhub.service"
  \
Start tinyhub service *(it will create new appsettings.json and be started with default settings)* and check it has started without errors. Then stop service and configure needed sections of appssettings.json as shown below and then start the service:
:::code language="json" source="nginx/appsettings-hub.json" highlight="15,20,21,22,25,30"
*Note, this Nginx web site configuration has the Client configuration section too.*
<br />

Create a Nginx web site as shown below and start it (reload Nginx).
:::code language="txt" source="nginx/nginx.conf" highlight="5,16,17,22"
Hub has been deployed and now deploy [Client](/installation/client.html).\
<br/>

After Hub and Client deploying, start tinyPlatform. See video "Hub installation and quick start to get data".
> [!Video https://www.youtube.com/embed/jkAjIlBb3UU?si=nWUtV3DSfzRmkv5A]
<br />


## ISS

Chapter under writing... 