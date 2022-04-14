# Unity-ModEditor
## Start  
![image](https://user-images.githubusercontent.com/71002504/162974213-834fcdef-c571-40c0-bcc6-992128122e3a.png)  

## Support Version  
__Unity2019.3__ ~  
****
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
|  | /Ctrl + MouseDragLeft | Adjust the BrushRange |
|  | /Space | Adjust the BrushDepth to the vertex with the smallest depth within the mouse range |
| 5 | /Alt + Mouse-ScrollWheel | Extend the vertices selection |
| 6 | /Lock | Lock selected vertices (Once you lock the vertices you can manipulate only those vertices) |
|  | /shift + MouseClickLeft | Add selected locked vertices (Only used in vertex lock state) |
|  | /shift + MouseClickRight | Sub selected locked vertices (Only used in vertex lock state) |
| 9 | /Alt + /Lock | Lock selection (After locking selection you can adjust the write parameters in the control panel and write again) |

* 0:  
![vert_noDepth](https://user-images.githubusercontent.com/71002504/162924636-b5a97b44-b3c6-477f-a1a1-0cea19f11445.png)  
* 1:  
![brushVIew](https://user-images.githubusercontent.com/71002504/162924335-8092bba7-a10f-42b7-a5e1-4636d99bcadc.png)  
* 5: extends the selected vertices based on grid information  
![vert_expand](https://user-images.githubusercontent.com/71002504/162924886-1a364f7f-d937-4730-b46a-f7f9eec4c935.gif)  
* 6: after locking selected vertices, you can only operate in the selection  
![vert_lock](https://user-images.githubusercontent.com/71002504/162925111-42b56e2c-5642-4447-a990-707330ff64f0.gif)  
* 9: after locking the vertex selection, click the button __"Brush Command Switch"__ to open the modification mode and modify the parameters to adjust the writing information  
![image](https://user-images.githubusercontent.com/71002504/162939106-8cac9be1-a393-4e48-8b0d-9b552f3f901b.png)  

## Calc Utility:  
Here are some mesh computing utilities:  

![image](https://user-images.githubusercontent.com/71002504/162929013-a9cf7ae5-d07b-47b0-9323-2f13d74b96e9.png)  

* AvgNormals example:  

![avgNormal](https://user-images.githubusercontent.com/71002504/162929198-23f4caed-77b4-4dc4-b68d-7de991159e33.png)  

Of course you can modify only the vertices selected in the scene (select this option and use shortcut key __"Enter"__ in the scene):  

![image](https://user-images.githubusercontent.com/71002504/162929493-3d8e4658-4c56-4c2e-9e1d-006d6125f17d.png)  

__You can extend this module__  
>Its data source is the original mesh  
>Here is an example of copy data, which you can find in this repository:  

![image](https://user-images.githubusercontent.com/71002504/162931620-3250f33b-6f36-4d36-8756-4750bebb6c92.png)  
![image](https://user-images.githubusercontent.com/71002504/162931780-57f8ff83-b214-4ca1-a829-3c062ce99fdd.png)  

## Brush Write - Other Utility:
Here are some mesh brush utilities  
It will cause your brush to carry special data, __the original data of the brush will be used as interpolation coefficients__  

![image](https://user-images.githubusercontent.com/71002504/162934193-db592567-cfdc-4990-86ab-454de3b17b52.png)
![image](https://user-images.githubusercontent.com/71002504/162934269-8ed76750-11c5-4c23-9443-f11ede1c2a5a.png)  

* LocalRemap example  
(this example is used as a spherical normal mapping):  
__Carry data:__ the vertex local position relative to the object  

![GIFa](https://user-images.githubusercontent.com/71002504/162935694-2c527721-da5a-48f4-a263-07bd565ff954.gif)  
![企业微信截图_16493259079374](https://user-images.githubusercontent.com/71002504/162946225-3699d1f1-59bd-49e3-af74-1efd51e783c5.png)
![企业微信截图_16493264148764](https://user-images.githubusercontent.com/71002504/162946238-a75c1b9e-5d27-47ec-952e-8570c1d29202.png)  

__You can extend this module__  
>In the main method __"BrushWrite()"__, write special data to the buffer __"CalcManager.Cache.RW_BrushResult"__   
>Here is an example that carries __direction data of the mesh itself__, which you can find in this repository:  

![image](https://user-images.githubusercontent.com/71002504/162943008-6ced6808-e64c-41f1-bb06-5c3e3510b5f2.png)  
![image](https://user-images.githubusercontent.com/71002504/162943681-fb1fde6b-aea2-4874-8e28-8ed67756e2bf.png)  
****
## Texture Brush: 
Use this to create a texture paint.  
To use it, you also need to set the scene EditorTool to ModEditorTool mode like using VertexBrush:  

![tool](https://user-images.githubusercontent.com/71002504/162179217-235e57fd-da0e-4c19-82a9-540de246d79a.png)  

__Shader variables:__  
 By declaring a sampler called ___EditorTex__ you will be able to sample the RenderTexture being drawn
 
__Save Path:__  
.../ModEditor/Textures  
 
__Operating command:__  

| Command | Detail |
| ------ | ------ |
| /V | Drawn preview |
| /Ctrl + MouseDragLeft | Adjust the BrushRange |
| /Ctrl + MouseDragRight | Adjust the BrushHardness |
| /Ctrl + Mouse-ScrollWheel | Rotate the Brush |

__Color Mask:__  
Here you can turn on and off writable color channels  
![image](https://user-images.githubusercontent.com/71002504/162965861-f94fd939-1e86-41dd-a6ae-9a6f17142f85.png)  

__Start your drawing:__  
* The mesh you need to draw must be equipped with a __"MeshCollider"__ component  
* Select the meshs you want to draw in the TextureBrush panel  
* Create a new RenderTexture with __Color__ or __Texture__

![企业微信截图_16493238225221](https://user-images.githubusercontent.com/71002504/162949715-2a2e33c1-7eba-4d7d-9e19-411d9622abaf.png)  
![GIF](https://user-images.githubusercontent.com/71002504/162949766-46fab533-4e6a-4baf-ade9-4573f9c135ee.gif)
![企业微信截图_16493247367740](https://user-images.githubusercontent.com/71002504/162949807-a11d0f38-fd29-4099-ba10-ac34d474b1a4.png)  

## Texture Utility:
Here are some texture manipulation utilities  
![image](https://user-images.githubusercontent.com/71002504/162966899-d0df4e6e-c431-4c9e-ad7b-10500cdd47dc.png)  

* TexturePassMerge example  
Use this utility to split the color channel of a Texture and output it to a new RenderTexture

![企业微信截图_16493249079656](https://user-images.githubusercontent.com/71002504/162968016-92fc2160-a8af-4222-b525-bccc924511ae.png)
![GIFs (2)](https://user-images.githubusercontent.com/71002504/162968050-f4a8cebd-8790-4c7d-ab56-b3b8f836d5d9.gif)  

* Blur example  
Use this utility to blur a texture (Gaussian blur)  

![image](https://user-images.githubusercontent.com/71002504/162970066-704e9fe8-ae68-43dc-b131-22edafa3d554.png)
![image](https://user-images.githubusercontent.com/71002504/162970256-cc67c95d-c4c0-4b7e-b8aa-f9625312ffcb.png)  

__You can extend this module__  
>Just write the RenderTexture of the passed parameter with ComputerShader or Graphics (any way you like)  
>Here is the example of blur texture, which you can find in this repository:  

![image](https://user-images.githubusercontent.com/71002504/162971702-df73414c-278f-4290-af10-dda3a22093d0.png)  
