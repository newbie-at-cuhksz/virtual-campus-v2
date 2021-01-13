## GitHub协作快速入门

### 1. 下载安装git

https://git-scm.com/downloads

下载后找到git bash说明可以了

打开powershell并完成配置

```
$ git config --global user.name "Your Name"
$ git config --global user.email "email@example.com"
```

### 2. 创建本地库

创建你要使用的文件夹, 并在路径栏中输入`powershell`打开(最好路径中不要有中文).

![image-20210113195724283](C:\Users\lenovo\AppData\Roaming\Typora\typora-user-images\image-20210113195724283.png)

![image-20210113195453057](C:\Users\lenovo\AppData\Roaming\Typora\typora-user-images\image-20210113195453057.png)

然后创建本地库

```
❯ git init

Initialized empty Git repository in E:/lenovo/STUDY/Y2_2/CUHKSZ_MODEL/.git/
```

(注意第三行是执行后的提示, 输入git init即可)

然后应该会看到.git文件. 这个不去动它. 没有的话看看是不是被隐藏了

### 3) 从远程仓库克隆

`git@github.com:newbie-at-cuhksz/virtual-campus-v2.git`是我们仓库的ssh地址.

```
❯ git clone git@github.com:newbie-at-cuhksz/virtual-campus-v2.git

Cloning into 'virtual-campus-v2'...
Warning: Permanently added the RSA host key for IP address '13.250.177.223' to the list of known hosts.
Enter passphrase for key '/c/Users/lenovo/.ssh/id_rsa':
remote: Enumerating objects: 29, done.
remote: Counting objects: 100% (29/29), done.
remote: Compressing objects: 100% (24/24), done.
Receiving objects:  68% (20/29), 19.68 MiB | 1.97 MiB/seused 0 eceiving objects:  65% (19/29), 19.68 MiB | 1.97 MiB/s
Receiving objects: 100% (29/29), 19.69 MiB | 485.00 KiB/s, done.
Resolving deltas: 100% (4/4), done.
```

第一次做会有warning

```
The authenticity of host 'github.com (xx.xx.xx.xx)' can't be established.
RSA key fingerprint is xx.xx.xx.xx.xx.
Are you sure you want to continue connecting (yes/no)?
```

yes即可

然后文件就同步完成了.目前应该在master分支上

### 4) push & pull



