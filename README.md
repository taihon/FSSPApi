# FSSPAPI

What is it
API for FSSP API (https://api-ip.fssp.gov.ru/)
To start this up you need:
1. Get Visual Studio. Community edition is fine.
2. Register on FSSP API website, and aquire API key
3. Open FSSPAPI\FSSPAPI.Core\ApiExchanger.cs and replace "FILL_ME" with your API key.
4. Compile project
5. Create "input.xlsx" with following structure:
   |Name|Surname|Birthdate|Region|
   |---|---|---|---|
   |Иван|Иванов|01.01.1990|77|
6. Place "input.xlsx" in FSSPAPI\FSSPAPI.Console\bin\Debug\netcoreapp3.1 folder
7. Run FSSPAPI\FSSPAPI.Console\bin\Debug\netcoreapp3.1\FSSPAPI.Console.exe
8. Get results_region_N_ddMMyyyyHHmmss.json
9. Parse in any way you wish.

Dependencies:
1. EPPlus - for reading\writing office files without installing Microsoft Office.
2. Newtonsoft.Json - for reading\writing json.