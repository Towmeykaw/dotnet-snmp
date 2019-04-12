[![Build Status](https://dev.azure.com/tommykindmark/tommykindmark/_apis/build/status/Towmeykaw.dotnet-snmp?branchName=master)](https://dev.azure.com/tommykindmark/tommykindmark/_build/latest?definitionId=1&branchName=master)
# dotnet-snmp
A dotnet global tool for sending Snmp messages

Install with command 
```
dotnet tool install -g SnmpSender
```
This tool currently support version 1 and 2 of get, set and trap. 

Invoke with command
```
snmp get 1.2.3.4 -a 127.0.0.1 -p 161
```
