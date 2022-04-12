# Unity-ModEditor
## in URP  
1. uncomment the contents of script which called 'ModEditor/Editor/ModEditorFeature'  
2. add ModEditor renderer feature in the pipeline  
****
## Control Panel:  
![panel](https://user-images.githubusercontent.com/71002504/162159160-3834f553-f211-49c9-b34e-16d23f6d56df.png)  
****
## Object View:  
![view](https://user-images.githubusercontent.com/71002504/162159353-00d388fb-0efb-4b56-9435-b9662ffd6419.png)
****
## Vertex Brush: 
Use this to write mesh information.  
To use it, you need to set the scene EditorTool to ModEditorTool mode:  

![tool](https://user-images.githubusercontent.com/71002504/162179217-235e57fd-da0e-4c19-82a9-540de246d79a.png)  

__Write Type:__  
* Vertex Color
* Vertex
* Normal
* Tangent
* Custom Pass

![企业微信截图_16493235207385](https://user-images.githubusercontent.com/71002504/162178908-d29dca7c-d3bd-40ec-845b-06541fcd224c.png)  

__Operating command:__  

| Example id | Command | Detail |
| ------ | ------ | ------ |
| 0 | /Tab | Draw vertices with ZTest |
| 1 | /V | Write preview |
|  | /Ctrl + Mouse-ScrollWheel | Adjust the BrushDepth |
|  | /Space | Adjust the BrushDepth to the vertex with the smallest depth within the mouse range |
| 4 | /Alt + Mouse-ScrollWheel | Extend the vertices selection |
| 5 | /Lock | Lock selected vertices (Once you lock the vertices you can manipulate only those vertices) |
|  | /shift + MouseClickLeft | Add selected locked vertices (Only used in vertex lock state) |
|  | /shift + MouseClickRight | Sub selected locked vertices (Only used in vertex lock state) |
|  | /Alt + /Lock | Lock selection (After locking selection you can adjust the write parameters in the control panel and write again) |

* 0  
![vert_noDepth](https://user-images.githubusercontent.com/71002504/162924636-b5a97b44-b3c6-477f-a1a1-0cea19f11445.png)  
* 1  
![brushVIew](https://user-images.githubusercontent.com/71002504/162924335-8092bba7-a10f-42b7-a5e1-4636d99bcadc.png)  
* 4  
![vert_expand](https://user-images.githubusercontent.com/71002504/162924886-1a364f7f-d937-4730-b46a-f7f9eec4c935.gif)  
* 5  
![vert_lock](https://user-images.githubusercontent.com/71002504/162925111-42b56e2c-5642-4447-a990-707330ff64f0.gif)

__Calc Util:__  
Here are some mesh computing utilities:  

![image](https://user-images.githubusercontent.com/71002504/162929013-a9cf7ae5-d07b-47b0-9323-2f13d74b96e9.png)  

Avg Normals example:  

![avgNormal](https://user-images.githubusercontent.com/71002504/162929198-23f4caed-77b4-4dc4-b68d-7de991159e33.png)  

Of course you can modify only the vertices selected in the scene (select this option and use shortcut key "Enter" in the scene):  

![image](https://user-images.githubusercontent.com/71002504/162929493-3d8e4658-4c56-4c2e-9e1d-006d6125f17d.png)  

You can extend this module, its data source is the original mesh (here is an example of copy data, which you can find in this repository):  

![image](https://user-images.githubusercontent.com/71002504/162931620-3250f33b-6f36-4d36-8756-4750bebb6c92.png)  
![image](https://user-images.githubusercontent.com/71002504/162931780-57f8ff83-b214-4ca1-a829-3c062ce99fdd.png)  
