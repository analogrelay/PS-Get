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

1. Open PowerShell
2. Run the following command:
```
(new-object Net.WebClient).DownloadString("http://install.psget.org") | iex
```
3. Follow the prompts!


FAQs
--------------------------------
Q: The installer fails with the following message: 'File ... cannot be loaded because the execution of scripts is disabled on this system. Please see "get-help about_signing" for more details.'

A: PowerShell requires that scripts be signed and we aren't able to sign PS-Get right now. You should temporarily disable signing by running the following command:

```
Set-ExecutionPolicy RemoteSigned
```

We recommend setting the Execution Policy back to normal after installing, but feel free to leave it at your own risk :).

Q: The installer fails with the following message: "PS-Get requires .NET 4.0 ..."

A:

Either download the PowerShell 3.0 CTP from http://www.microsoft.com/download/en/details.aspx?id=27548 or try the following workaround:

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