1. Into index.html the tag shold be as <script>navigator.serviceWorker.register('service-worker.js');</script>
2. Into service-worker.published.js the code should be as self.importScripts('./service-worker-assets.js');
3. Into manifest.json the parameter should be as "start_url": "./",
4. Into project file (.csproj) <StaticWebAssetBasePath>dashboard</StaticWebAssetBasePath> should be comment <!--<...>--> unless asp.net core hosting model
5. After publishing the code from a publishing directory wwwroot need to copy to wwwroot/{BasePathFolder} on a server
6. On IIS need to install UrlRewrite2.exe from https://www.iis.net/downloads/microsoft/url-rewrite
7. Into web.config need to change a rule for static files in that way: 
<rule name="SPA fallback routing" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
          </conditions>
          <action type="Rewrite" url="wwwroot\{B a s e P a t h}\" />
        </rule>

PS: After publishing the published files cannot be modified because service-worker.js controls hash of these files in service-worker-assets.js 