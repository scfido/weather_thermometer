# WeatherStation

惠多气象微信小程序服务端

## 构建容器

```sh
cd Server/Server
docker build ./ -t scfido/weatherstation:latest
```

## 配置

服务启动之前要配置微信应用信息，请在appsettings.json文件中配置。
```json
{
    "WeChat": {
        "AppId": "从微信小程序网站获取",
        "AppSecret": "从微信小程序网站获取"
    }
}
```

## 启动容器

使用nginx代理的情况。

```sh
# 创建名为webnet的网络
sudo docker network create webnet

# 启动气象站服务
docker run -d --net=webnet -v/root/weather_station/database:/app/database -v/root/weather_station/appsettings.json:/app/appsettings.json --name ws scfido/weatherstation:latest

# 启动Nginx
docker run -d --net=webnet --restart=always --name nginx -p80:80 -p 443:443 -v/root/nginx/log:/var/log/nginx -v/root/nginx/nginx.conf:/etc/nginx/nginx.conf:ro -v/root/nginx/certs:/root/certs nginx
```

**注意：** Nginx使用的Https证书要存放到certs目录中。
