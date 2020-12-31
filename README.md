# SDN List Monitor

SDN List Monitor is a tool to monitor changes in the entries of OFAC's Specially Designated Nationals (SDN) List. This is done by periodic polling to the location of the SDN List and observing the symmetry between stored and retrieved entries.

## Overview

SDN List Monitor is structured as follows:

- **SdnListMonitor.Core.Abstractions** - provides abstractions for storing (`ISdnDataPersistence`), fetching (`ISdnDataRetriever`), comparing (`ISdnDataChangesChecker`), and monitoring (`SdnMonitorServiceBase`) SDN entries. 
- **SdnListMonitor.Core** - provides implementations for storing (`InMemorySdnDataPersistence`), comparing (`SdnDataSymmetryChecker`), and monitoring (`SdnChangesMonitorService`) SDN entries.
- **SdnListMonitor.Core.Xml** - provides implementations for fetching (`SdnXmlDataRetriever`) and equality comparison (`SdnXmlEntryEqualityComparer`) of SDN entries in SDN.xml file that corresponds to the format defined in [SDN.xsd](https://home.treasury.gov/system/files/126/sdn.xsd) schema file.

**SdnListMonitor.ConsoleApp** acts as an example to demonstrate the workflow.

## Improvements To Make
- Proper error handling with retry logic (e.g. [Polly](https://github.com/App-vNext/Polly)) when the SDN List is malformed.
- Asynchronous deserialization for `SdnXmlEntry` in `SdnXmlDataRetriever`.
- Support for `Microsoft.Extensions.Logging.ILogger` and proper logging.
- Proper `CancellationToken` handling.
- Support for storing entries in an external persistence storage for the next monitoring check. 
- Extended `SdnXmlEntry` equality comparison to support additional properties: `programList`, `idList`, `akaList`, `addressList`, `nationalityList`, `citizenshipList`, `dateOfBirthList`, `placeOfBirthList`, `vesselInfo`.
- Support for the compressed SDN data (SDN.zip) retrieval in order to reduce network data usage.
- Support for the advanced sanctions data model (sdn_advanced.xml) developed by the UN 1267/1988 Security Council Committee.
