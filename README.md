# Latest Version 1.0.2
https://github.com/Russell-KV4S/KV4S.AR.DMR.BM.TG.Exporter/releases/download/v1.2/KV4S.AR.DMR.BM.TG.Exporter.zip

Currently, the applicaiton supports the AnyTone model radios.

# KV4S.AR.DMR.BM.TG.Exporter
Application to download the Brandmeister DMR Talkgroup List from the API: https://api.brandmeister.network/v1.0/groups and converts it to a CSV file for import into a DMR Radio.

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
    <!--If you run this as a job or don't need to see the output then make Unattended Y-->
    <add key="Unattended" value="N"/>
    <!--Y/N value only-->
    <add key="AnyTone" value="Y"/>
    <add key="IDinsteadOfName" value="N"/>
    <!--Set to N for DMARC as local numbers are different-->
    <add key="LessThan91" value="Y"/>
    <!--Use this to filter country 31=US 235=UK. 
    For multiple countried separate by commas.  Example: "31,235,271" (no comma after last one).
    Blank for no filter.-->
    <add key="IDStartsWith" value=""/>
    <!--TGs to add to the end that are not from Brandmeister-->
    <add key="ExtraTGList" value="TG_Extras.csv"/>
  </appSettings>
</configuration>
```

Since this is a console application you can use Windows Task Scheduler to run this in the backgroud on a schedule of your choosing.

The CSV files are writen to the same location as the executable named:
```
AnyTone_TGs.csv

```
Use your radios CPS to import and write to your radio.

Update after 1.0.3
If you would like to add custom TGs to the end of the BM list create a "Extras" file.
It's the same format as the one you import just leave off the IDs as the program will populate them.
You may want this from other networks (TGIF/DMARC) or private groups or private calls.
Pay attention that you don't duplicate numbers or names as you may get import warnings in the CPS.


