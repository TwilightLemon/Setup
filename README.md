# Setup
使用WPF创建 Modern&Elegant 安装程序  
[![LICENSE](https://img.shields.io/badge/license-GPL%20v3.0-blue.svg?style=flat-square)](https://github.com/TwilightLemon/Setup/blob/master/LICENSE)
[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
[![996.icu](https://img.shields.io/badge/link-996.icu-red.svg)](https://996.icu)  
## 主要功能
- 释放zip文件
- 添加快捷方式
- 添加注册表项
- 卸载程序
## 如何使用
修改Setup.csproj 和 Uninstall.csproj 项目签名  
修改Setup/MainWindow.xaml.cs 和 Uninstall/Program.cs 中的安装信息  
替换掉Setup/Data.zip文件
发布成单文件即可