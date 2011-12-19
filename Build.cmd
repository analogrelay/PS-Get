path=%path%;%windir%\Microsoft.NET\Framework\v4.0.30319\
if not '%1'=='' (msbuild FullBuild.msbuild /t:%1) else (msbuild FullBuild.msbuild)