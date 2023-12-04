# NLog.Targets.RedisJson

This project is an NLog target for Redis server.

## What it does

Write the logged message as a JSON value with, optionally, a TTL (time-to-live).

## Example (With Docker)

Install the NuGet library

Launch the pre-made compose redis-stack
`docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest`

If you want a password-protected Redis stack
`docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 -e REDIS_ARGS="--requirepass mystrongpassword" redis/redis-stack:latest`

Configure the nlog.config (or the appsettings.json), add the assembly

``` xml
    <extensions>
        <add assembly="NLog.Targets.Redis" />
    </extensions>
```

### With JsonLayout

JsonLayout is a NLog layout, you can find additional info on [doc](https://nlog-project.org/documentation/v5.0.0/html/Properties_T_NLog_Layouts_JsonLayout.htm) page and on [wiki](https://github.com/NLog/NLog/wiki/JsonLayout)

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

#### Output

With the JsonLayout you customize the JSON message fields, in the example above is logged a message with the fields:

- time: with the longdate of the message
- level: INFO | DEBUG | etc
- message: the logged message
- exception: if any, otherwise null

### Without JsonLayout

Without JsonLayout you can specify on the `Layout` property

``` xml
<target xsi:type="RedisJson" name="redisjson" host="127.0.0.1" port="6379" db="0" ItemKey="${level:upperCase=true}_${sequenceid}" TTL="365:00:00:00" ConfigurationOptions="name=foo,keepAlive=5"
        layout="${longdate}|${level}|${message} |${all-event-properties} ${exception:format=tostring}">
```

#### Output

Without JsonLayout on Redis, a JSON message with:

- Level: DEBUG | INFO | ecc
- Exception: If any, otherwise null
- SequenceId: The id of the message of NLog
- Timestamp: The timestamp of the message
- StackTrace: If any, otherwise is null
- Message: The message with the Layout specified in the `nlog.config`

### Options

Below is the item value configuration:

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
The Redis password

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
