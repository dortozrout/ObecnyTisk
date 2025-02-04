@echo off
rem C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -r:System.Net.Http.dll  *.cs HtmlParser\*.cs Form\*.cs
set exeName=Labels.exe
IF NOT EXIST .\publish (
    mkdir publish
)
set currentDisc=%cd:~0,3%
set roslynDir=Roslyn-4.12.0
IF EXIST "%currentDisc%\%roslynDir%\csc.exe" (
    %currentDisc%\%roslynDir%\csc.exe -nologo -r:System.Net.Http.dll *.cs .\Form\*.cs -out:.\publish\%exeName%
) ELSE (
    echo Compiller not found!
)
pause
