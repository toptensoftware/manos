xcopy /E /Y /D ..\docs\*.* .\docs\
xcopy /E /Y /D ..\data\*.* .\data\

SET PATH=%PATH%;C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\

gacutil /I Nini.dll
gacutil /I Manos.IO.dll
gacutil /I Manos.dll
gacutil /I Manos.Mvc.dll
