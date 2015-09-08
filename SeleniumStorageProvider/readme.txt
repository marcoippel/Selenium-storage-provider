For the azure blob provider fill in your connectionstring and your storage containername.

<add key="AzureBlob:StorageContainer" value="" xdt:Transform="InsertIfMissing" xdt:Locator="Match(key)"/>
<add key="AzureBlob:StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName={your accountname};AccountKey={your acount key}" xdt:Transform="InsertIfMissing" xdt:Locator="Match(key)"/>


For the slack provider fill in your slack token and your slack channelname
 
<add key="Slack:Token" value="{your slack token}" xdt:Transform="InsertIfMissing" xdt:Locator="Match(key)"/>
<add key="Slack:Channel" value="{#channel}" xdt:Transform="InsertIfMissing" xdt:Locator="Match(key)"/>


To add the environment in your azure blob url add the following format to your config:
The host of your environment and the environment letter that will be added to your azure blob url

<add key="o.unittest.nl" value="o"/>
<add key="t.unittest.nl" value="t"/>
<add key="a.unittest.nl" value="a"/>
<add key="p.unittest.nl" value="p"/>