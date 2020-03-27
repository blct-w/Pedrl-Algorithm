# Pedrl-Algorithm工程介绍
PDERL算法的测试关键代码和测试数据。  
本工程在VS2017下开发，用C#在.net core 2.1上实现。    
这是一个RESTFUL风格的WebApi工程。    
接口文件在["./Code/Pderl/Controllers/DemController.cs "](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/Controllers/DemController.cs)处编写。    
主要功能函数在["./Code/Pderl/DemAnalysisHandle.cs"](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/DemAnalysisHandle.cs)处编写。    
这是一个算法测试工程，测试用的DEM数据并不是连续的，按文件放在["./DEM/"](https://github.com/blct-w/Pedrl-Algorithm/tree/master/DEM)。读者也可自行添加DEM文件（只支持*.tif格式）。   
如有需要可以联系我[邮箱](blct_w@foxmail.com)。

# 按如下步骤进行代码测试:
1. 拷贝proj.dll到您电脑的执行文件夹，Windows系统为“C:\Windows\System32”；
2. 还原工程；
3. 启动运行；
4. 决定好您要在哪块DEM文件上进行测试，比如"ASTGTM2_N41E119_dem.tif"文件，请在浏览器地址栏输入".../api/dem/setdem/ASTGTM2_N41E119_dem"并回车；
5. 在浏览器中输入RESTFUL风格的API资源地址访问服务，比如说".../api/dem/analysis/119.5/41.5/119.55/42.55/2"，然后回车，您将得到PDERL的分析结果。值得注意的是，您输入的经纬度范围不可超过上一步中DEM文件的覆盖范围。
6. 更多的测试API请查看["./Code/Pderl/Controllers/DemController.cs "](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/Controllers/DemController.cs)文件的路由定义部分。


# Pedrl-Algorithm   
Key code and test data for Pedrl algorithm.  
This project is code in vs2017 with C# under .net core 2.1.  
This a restful WebApi project.   
The api service is code in the ["./Code/Pderl/Controllers/DemController.cs "](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/Controllers/DemController.cs); 
The main function of Pderl is code in ["./Code/Pderl/DemAnalysisHandle.cs"](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/DemAnalysisHandle.cs);   
The DEM used in this project is not continuous. All dems available are in folder ["./DEM/"](https://github.com/blct-w/Pedrl-Algorithm/tree/master/DEM), and you can add your own dem file to this folder (only support *.tif); 
 
# To test following these steps :
1. Copy proj.dll to your computer's execution folder, such as the "C:\Windows\System32" folder for Windows;
2. Restore this project;
3. Run;
4. Decide which DEM file do you want to test on, for example "ASTGTM2_N41E119_dem.tif", then input ".../api/dem/setdem/ASTGTM2_N41E119_dem" in your browser and press the enter key;
5. To test the Pderl, input the restfull api url in your browser, for example ".../api/dem/analysis/119.5/41.5/119.55/42.55/2" then press enter, you will get the result. Note that the latitude and longitude range of the test must not exceed the coverage of the DEM file you set up in the previous step.
6. For more test api, please see the routing definition part of the function in ["./Code/Pderl/Controllers/DemController.cs "](https://github.com/blct-w/Pedrl-Algorithm/blob/master/Code/Pderl/Controllers/DemController.cs);
