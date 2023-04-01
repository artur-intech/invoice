# INTECH Invoice

## Overview

INTECH Invoice is a simple PDF invoice generation application. This is a console program written
in C# using NET 7 framework with minimal dependencies. The app is fully dockerized for both development
and production environments. It includes unit, integration (mostly database-related) and
[end-to-end tests](test/ProgramTest.cs) written using NUnit testing framework.

## Description

- All monetary unit input and output values are assumed to be in euros.
- Invoice numbers have the format of yyyyMMddHHmmss where each letter corresponds to the current
year, month, day, hours, minutes and seconds respectively.

## Configuration

- Standard VAT rate is 20% by default. Can be changed using `STANDARD_VAT_RATE` environment variable.
- Time zone affects timestamps that constitute invoice numbers. It is configured as a
case-insensitive string via `TIME_ZONE` environment variable. For example, `Europe/Munich`,
`Europe/Moscow` are valid values. Local (system) time zone is used by default.
[More information](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo.findsystemtimezonebyid?view=net-7.0#remarks).
- Date and money format is specified in RFC 4646 format via the `CULTURE` environment variable.
Example: `en-US`, `de-DE`. System locale is used by default.
[More information](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-8.0#CultureNames)
