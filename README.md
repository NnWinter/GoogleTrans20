---- 更新说明 ----

这一次的更新相比于 v1.3 重做了整个交互模式<br/>
添加了对于其它 API 的支持可能，如 有道翻译（Beta）<br/>
支持跨平台 -> Windows (主要) Linux (支持) MacOS (可用但不支持)<br/>
允许用户为每个API单独保存设置<br/>
允许用户保存全局设置到本地<br/>
支持自定义翻译次数<br/>
支持自定义翻译间隔<br/>

---- 程序声明 ----

我就很讨厌那种需要安装到系统注册表或者C盘目录之类的操作<br/>
安装之后都删不干净，动不动就残留<br/>
所以这个程序所有的配置文件都在程序目录下<br/>

GlobalOptions.txt 是全局设置<br/>
APIs/<API名>/Config.txt 是 API 的设置<br/>
设置文件使用 "//" 作为注释标记<br/>
读取参数的方式可以对照源代码<br/>
通常使用 <参数名> = <参数> 分割<br/>
手动修改有风险，如果遇到错误可以删除设置文件<br/>
删除后将重新按照默认设置生成新的设置文件<br/>

那个翻译过程如果觉得碍眼，可以在全局设置里关闭<br/>

APIs/<API名>/Language.txt 是 API 官方给出的可用语言列表<br/>
语言列表使用 <缩写>, <全称> 的 Trim() 作为格式<br/>
要给语言添加说明可以修改文件内容如：<br/>
`af` 改为 `af, 阿富汗语`

---- 安装方法 ----

首先你的电脑需要有 [.Net6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 运行时 (否则会闪退)<br/>
(Linux用户可以使用 `sudo apt install dotnet-sdk-6.0` 进行安装)<br/>

-- Windows 用户 --<br/>
-->-- Windows 用户下载 [发布页](https://github.com/515621078/GoogleTrans20/releases) 中的 win-x64_x86.zip<br/>
--- -- 32位 --<br/>
----->-- 32位系统把 x86 文件夹解压出来<br/>
--- -- 64位 --<br/>
----->-- 64位系统把 x64 文件夹解压出来<br/>
-- 运行 exe 根据说明操作即可<br/>
    
-- Linux 用户 --<br/>
-->-- 解压文件夹<br/>
-->-- 为 Net6 添加可执行权限 如: `chmod a+x Net6`<br/>
-->-- 运行 Net6 如: `./Net6` 根据说明操作即可
    
---- 注意事项 ----

如果翻译次数过多会因为操作频繁而被禁用翻译（取决于IP）。

---- 特别鸣谢 ----

感谢 [@baguotao233](https://github.com/baguotao233) 和 [@rdp-studio](https://github.com/rdp-studio) 提出的建议
所以才做了这个2.0版本
