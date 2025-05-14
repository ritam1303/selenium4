#!/bin/bash

# Define the local NuGet package directory
LOCAL_FEED="LocalNuGetPackages"

# Create the local NuGet feed directory if it doesn't exist
mkdir -p $LOCAL_FEED

# Install NuGet packages locally
echo "Installing NuGet packages..."

nuget install NUnit -Version 3.12.0 -OutputDirectory $LOCAL_FEED
nuget install NUnit3TestAdapter -Version 3.16.1 -OutputDirectory $LOCAL_FEED
nuget install Microsoft.NET.Test.Sdk -Version 17.2.0 -OutputDirectory $LOCAL_FEED
nuget install Selenium.WebDriver -Version 3.141.0 -OutputDirectory $LOCAL_FEED
nuget install Selenium.WebDriverBackedSelenium -Version 3.14.0 -OutputDirectory $LOCAL_FEED
nuget install Selenium.Support -Version 3.141.0 -OutputDirectory $LOCAL_FEED
nuget install ExtentReports -Version 4.1.0 -OutputDirectory $LOCAL_FEED
nuget install DotNetSeleniumExtras.WaitHelpers -Version 3.11.0 -OutputDirectory $LOCAL_FEED
nuget install DotNetSeleniumExtras.PageObjects -Version 3.11.0 -OutputDirectory $LOCAL_FEED
nuget install Microsoft.NETCore.Platforms -Version 2.1.0 -OutputDirectory $LOCAL_FEED
nuget install NETStandard.Library -Version 2.0.3 -OutputDirectory $LOCAL_FEED
nuget install Microsoft.TestPlatform.TestHost -Version 16.5.0 -OutputDirectory $LOCAL_FEED
nuget install System.Reflection.TypeExtensions -Version 4.3.0 -OutputDirectory $LOCAL_FEED
nuget install System.Collections.Immutable -Version 1.3.1 -OutputDirectory $LOCAL_FEED
nuget install Selenium.WebDriver -Version 4.4.0 -OutputDirectory $LOCAL_FEED
# nuget install Selenium.WebDriver.ChromeDriver -Version 113.0.5672.63 -OutputDirectory $LOCAL_FEED


echo "NuGet packages installed successfully at $LOCAL_FEED"
