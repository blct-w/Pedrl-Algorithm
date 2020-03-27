# Pedrl-Algorithm PDERL算法测试
PDERL算法的测试关键代码和测试数据。  
Key code and test data for Pedrl algorithm.  
本工程在VS2017下开发，用C#在.net core 2.1上实现。    
This project is code in vs2017 with C# under .net core 2.1.  
这是一个WebApi工程。    
This a WebApi project.   
RESTFUL风格的接口文件在["./Code/Pderl/Controllers/DemController.cs "](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/Controllers/DemController.cs)处编写。    
The restful service is code in the ["./Code/Pderl/Controllers/DemController.cs "](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/Controllers/DemController.cs); 
主要功能函数在["./Code/Pderl/DemAnalysisHandle.cs"](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/DemAnalysisHandle.cs)处编写。    
The main function of Pderl is code in ["./Code/Pderl/DemAnalysisHandle.cs"](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/DemAnalysisHandle.cs);   
这是一个算法测试工程，测试用的DEM数据并不是连续的，按文件放在["./DEM/"](https://github.com/blct-w/Pedrl-Algorithm/tree/master/DEM)。读者也可自行添加DEM文件（只支持*.tif格式）。  
The DEM used in this project is not continuous. All dems available are in folder ["./DEM/"](https://github.com/blct-w/Pedrl-Algorithm/tree/master/DEM), and you can add your own dem file to this folder (only support *.tif); 
 
# To test follow as this steps 按如下步骤进行代码测试:
1. Copy proj.dll to your computer's execution folder, such as the "C:\Windows\System32" folder for Windows;
2. Restore this project;
3. Run;
4. Decide which DEM file do you want to test on, for example "ASTGTM2_N41E119_dem.tif", then input ".../api/dem/setdem/ASTGTM2_N41E119_dem" in your browser and press the enter key;
5. To test the Pderl, input the restfull api url in your browser, for example ".../api/dem/analysis/119.5/41.5/119.55/42.55/2" then press enter, you will get the result. Note that the latitude and longitude range of the test must not exceed the coverage of the DEM file you set up in the previous step.
6. For more test api, please see the routing definition part of the function in ".Code/Pderl/Controllers/DemController.cs ";
