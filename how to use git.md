## GitHub协作快速入门

### 1. 下载安装git

https://git-scm.com/downloads

下载后找到git bash说明可以了

打开powershell并完成git配置

```
$ git config --global user.name "Your Name"
$ git config --global user.email "email@example.com"
```

### 2.从远程仓库克隆

打开一个文件夹. 最好不要包含中文路径. 在资源管理器中输入`powershell`回车打开.

`git@github.com:newbie-at-cuhksz/virtual-campus-v2.git`是仓库的ssh地址.

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

然后文件就同步完成了.

会看到文件夹里面包含了项目文件夹.

点进项目文件夹, 会看到一个.git文件, 说明ok

项目文件夹打开powershell, 会看到目前在main分支上.

powershell可以下载最新版本, 并使用oh my posh插件, 会好用很多

### 3) push

修改完文件, 可以先提交到本地的版本库, 步骤如下.

先add. 可以add多个文件

```
$ git add "filename.md"
```

然后一次性提交

```
$ git commit -m "注释, 必填"
```

提交完就是一个版本. 本地可以由多个版本

本地完成后, 这个命令把所有版本push到github仓库:

```
$ git push -u origin main
```

origin表示远程库, main是一个分支. 请选择要提交到的分支.

![image-20210113224456444](C:\Users\lenovo\AppData\Roaming\Typora\typora-user-images\image-20210113224456444.png)

### 4) pull

将远程库更新到本地.

```
$ git pull origin main
```





待更新….



