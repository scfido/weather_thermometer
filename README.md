# WeatherStation
家庭气象站服务

## 构建容器

```sh
docker build ./ -t scfido/weatherstation:latest
```

## 启动容器
```sh
sudo docker run -d -p443:443 -p80:80 -v/root/weatherStation/https.pfx:/app/https.pfx -v/root/weatherStation/database:/app/database -v/root/tempServer/appsettings.json:/app/appsettings.json -e HttpsCertPassword=yourpassword weatherStation
```
注意两个参数，上传你的https证书到`home`目录，命名为`https.pfx`
环境变量`HttpsCertPassword`是传入https证书密码
