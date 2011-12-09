Ps-Get
======

*NuGet [Package Installer] for PowerShell Modules*

Introduction
------------

If you use PowerShell often there is a good chance the you have seeked out and installed many extension modules, mainly [PowerShell Community Extensions] (http://pscx.codeplex.com/ "PowerShell Community Extensions").

I take it, if you're reading this page you remember how fun and interesting it was to install extension modules. The point of this project is to make extending PowerShell simple and easy.

Requirements
------------

1. [Powershell 2.0](http://www.microsoft.com/web/gallery/install.aspx?appid=PowerShell2)
2. [Microsoft .NET Framework 4](http://www.microsoft.com/web/gallery/install.aspx?appid=NETFramework4)

How to Install
--------------

1. Download the latest source.
2. Run the Build.cmd 
3. Navigate to the 'Bin' Folder.
4. Navigate to the 'Release' Folder.
5. Navigate to the 'Installer' Folder.
6. Double-Click 'PsGet.Installer.exe'.
7. Click Install.

PowerShell Workaround for .NET 4
--------------------------------

Create a file called **powershell.exe.config**, paste in the following:

```xml
<?xml version="1.0"?> 
<configuration> 
    <startup useLegacyV2RuntimeActivationPolicy="true"> 
        <supportedRuntime version="v4.0.30319"/> 
        <supportedRuntime version="v2.0.50727"/> 
    </startup> 
</configuration>
```

Now copy this file and paste it in **%windir%\System32\WindowsPowerShell\v1.0\**