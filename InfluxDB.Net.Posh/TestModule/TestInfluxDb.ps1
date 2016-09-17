#***************************************************************************************************************
#** TestInfluxDb.ps1
#***************************************************************************************************************
#** 
#***************************************************************************************************************

param (
	[switch]$copyFiles = $true
)

$host.ui.rawui.WindowTitle = "Test InfluxDb"

if($copyFiles)
{
	$srcPath = "..\bin\Debug\*"
	$dstPath = ".\Modules\InfluxDb"
	new-Item -Path ".\Modules" -ItemType:Directory -Force
	new-Item -Path ".\Modules\InfluxDb" -ItemType:Directory -Force
	write-host "Copy '$srcPath' to '$dstPath'" -f Yellow
	#Try
	#{
		copy-item $srcPath $dstPath -container -recurse -force -erroraction:Stop
	#}
	#Catch
	#{
	#	write-host "Oups!" -f Red
	#	write.host $_.Exception
	#}
}

import-module .\modules\InfluxDb -force
get-command -module:InfluxDb | select Name | ft 

#Create a database connection
$db = Open-InfluxDb -Uri:"http://...:8086" -User:"root" -Password:"root"

#Ping the connection
$pong = Ping-InfluxDb -Connection:$db
$pong

#Create a new database
Add-InfluxDb -Connection:$db -Name:"TheTest"

#Write measurement data to the database
Write-InfluxDb -Connection:$db -Name:"TheTest" -Data:"{'Measurement':'SomeData','Tags':{'Tag1':'A','Tag2':'B'},'Fields':{'Val1':12,'Val2':1.2},'Precision':1,'Timestamp':null}"

remove-module InfluxDb

#***************************************************************************************************************

pause