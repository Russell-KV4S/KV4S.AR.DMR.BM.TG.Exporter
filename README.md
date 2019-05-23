# Latest Version 1.0.0
https://github.com/Russell-KV4S/KV4S.AR.DMR.BM.TG.Exporter/releases/download/v1.0/KV4S.AR.DMR.BM.TG.Exporter.zip

Currently, the applicaiton supports the AnyTone model radios.

# KV4S.AR.DMR.BM.TG.Exporter
Application to download the Brandmeister DMR Talkgroup List and convert it to a CSV file for import into a DMR Radio.

Contact me if you would like to add more and have the file specs or use Git and create your own and merge them back in.

Other than simply executing the Application you can control what CSV files are created for the type(s) of radio you have.
Simple edit the .config file located with the executable and use Y/N to manipulate the application. 
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
    </startup>
  <appSettings>
    <add key="URL" value="https://www.radioid.net/static/users.json"/>
    <!--Set this to "Y" if you are scheduling this to run and don't need the console window to stay open.-->
    <add key="Unattended" value="N"/>
    <!--Y/N value only-->
    <add key="AnyTone" value="Y"/>
  </appSettings>
</configuration>
```

Since this is a console application you can use Windows Task Scheduler to run this in the backgroud on a schedule of your choosing.

The CSV files are writen to the same location as the executable named:
```
AnyTone_TGs.csv

```

Use your radios CPS to import and write to your radio.
