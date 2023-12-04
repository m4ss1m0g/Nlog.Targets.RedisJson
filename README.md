# NLog.Targets:RedisJson

This project is a NLog target for Redis server.

## What it do

Write the logged message as a JSON value with, optionally, a ttl (time-to-live).

## Example (With Docker)

Install the NuGet library

Launch the pre-made compose redis-stack
`docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest`

If you want a password protected Redis stack
`docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 -e REDIS_ARGS="--requirepass mystrongpassword" redis/redis-stack:latest`

Configure the nlog.config (or the appsettings.json)

``` xml

<target xsi:type="RedisJson" name="redisjson" host="127.0.0.1" port="6379" db="0" ItemKey="${level:upperCase=true}_${sequenceid}" TTL="365:00:00:00" ConfigurationOptions="name=foo,keepAlive=5">
            <layout xsi:type="JsonLayout" includeEventProperties="true" excludeProperties="Comma-separated list (string)">
                <attribute name="time" layout="${longdate}" />
                <attribute name="level" layout="${level:upperCase=true}"/>
                <attribute name="message" layout="${message}" />
                <attribute name="exception" layout="${exception:format=tostring}" />
            </layout>
        </target>

```

### Options

Below the item value configuration:

#### Host

> Type string - Required  

The Host name

#### Port

Type int - Optional default to 6379  
The redis port

#### Db

Type int - Optional default to 0  
The redis DB id

#### Password

Type string - Optional
The resis password

#### ItemKey

> Warning! if not unique it will be skipped without notice

Type (Layout) - Optional default to $"log_{DateTime.Now.Ticks}"  
The item key

#### TTL

Type [Timespan format](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.parse?view=net-8.0) - Optional
Set the TTL for the item

#### ConfigurationOptions

Type string - Optional
The additional Redis [configuration](https://stackexchange.github.io/StackExchange.Redis/Configuration.html) options
