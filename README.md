# WiFi-Temperature
WiFi温度计API服务

## 启动容器
```sh
sudo docker run -d -p443:443 -p80:80 -v/root/https.pfx:/app/https.pfx -e HttpsCertPassword=yourpassword temp
```
注意两个参数，上传你的https证书到`home`目录，命名为`https.pfx`
环境变量`HttpsCertPassword`是传入https证书密码
