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
Use this to write vertex information.  
To use it, you need to set the scene EditorTool to ModEditorTool mode:  

![tool](https://user-images.githubusercontent.com/71002504/162179217-235e57fd-da0e-4c19-82a9-540de246d79a.png)  

Write Type:  
* Vertex Color
* Vertex
* Normal
* Tangent
* Custom Pass

![企业微信截图_16493235207385](https://user-images.githubusercontent.com/71002504/162178908-d29dca7c-d3bd-40ec-845b-06541fcd224c.png)  

Command:  
* /Tab :  Draw vertices with ZTest
* /V :  Write preview
* /Ctrl + Mouse-ScrollWheel :  Adjust the BrushDepth
* /Space :  Adjust the BrushDepth to the vertex with the smallest depth within the mouse range
* /Alt + Mouse-ScrollWheel :  Expand the mouse range of vertices
* /Lock :  Lock selected vertices (Once you lock the vertices you can manipulate only those vertices)
  * /shift + MouseClickLeft :  Add selected locked vertices (Only used in vertex lock state)
  * /shift + MouseClickRight :  Sub selected locked vertices (Only used in vertex lock state)
* /Alt + /Lock :  Lock selection (After locking selection you can adjust the write parameters in the control panel and write again)

| /Tab | Draw vertices with ZTest |
| ------ | ------ |
| /V | Write preview |
