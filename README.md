## YumikoToys-Lite合盖不睡眠工具

因为经常带着笔记本远程开会，但是又不想被别人看到屏幕上的东西

所以 一个奇怪和合盖不休眠工具出现了

名字来源于微软的小工具合集：PowerToys（推荐使用 很好用）

使用后可以变成大号MP3（bushi)

## 主要代码文件说明

```
├── .YumikoToys
    ├── .bin - 生成的exe文件目录
    ├── .Resources - 主程序界面上的图片字体素材
    ├── About.cs - 关于界面窗体（开机自启动）
    ├── FormMain.cs - 主窗体的代码文件
    ├── Display.cs - 屏幕旋转一类的
    ├── Power*.cs - 电源一类的
    ├── TaskBarUtil.cs - 托盘图标一类的
```

写的很垃圾 部分没实现 反正能跑就不是事

## 程序BUG和缺陷
- 程序最小化会有2个托盘图标（用于添加电源方案）
- 部分情况下会失效（会让合盖不睡眠功能失效）
- 没做获取电池最大容量的代码

## 截图
![Snipaste_2023-01-18_15-27-44](https://user-images.githubusercontent.com/8959123/213109841-c4a7b310-6801-44af-b2a6-8223650e077b.png)
![Snipaste_2023-01-18_15-29-07](https://user-images.githubusercontent.com/8959123/213110072-69c2b547-9e3f-4f04-ac7e-797d5bf91b55.png)


## 程序准备工作

- clone 仓库的代码
- 打开VS Studio
- 修改界上的元素面
- 保存并生成

编译后去bin文件夹下找exe食用即可

### 额外说明

##### 编译环境

Visual Studio 2022 Preview

Windows 11 Home Insider Preview 10.0.25276 Build 25276（破苏菲自带的系统）


## 想说的

本程序写的很烂 市面上能实现类似功能的成熟的工具很多 该项目纯属想自定义一个自己的工具罢了
