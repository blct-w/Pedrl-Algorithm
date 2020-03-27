# Pedrl-Algorithm
Key code and test data for Pedrl algorithm.
This project is code in vs2017 with C# under .net core 2.1.
This a WebApi project. 
The restful service is code in the ".Code/Pderl/Controllers/DemController.cs ";
The main function of Pderl is code in ".Code/Pderl/DemAnalysisHandle.cs";
The DEM used in this project is not continuous. All dems available are in folder "./DEM/", and you can add your own dem file to this folder (only support *.tif);

To test follow as this steps:
1. Copy proj.dll to your computer's execution folder, such as the "C:\Windows\System32" folder for Windows;
2. Restore this project;
3. Run;
4. Decide which DEM file do you want to test on, for example "ASTGTM2_N41E119_dem.tif", then input ".../api/dem/setdem/ASTGTM2_N41E119_dem" in your browser and press the enter key;
5. To test the Pderl, input the restfull api url in your browser, for example ".../api/dem/analysis/119.5/41.5/119.55/42.55/2" then press enter, you will get the result. Note that the latitude and longitude range of the test must not exceed the coverage of the DEM file you set up in the previous step.
6. For more test api, please see the routing definition part of the function in ".Code/Pderl/Controllers/DemController.cs ";