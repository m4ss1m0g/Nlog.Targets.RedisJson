using System.Net;
using NLog.Targets.RedisJson.Test.Extensions;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.Extensions;
using StackExchange.Redis;

namespace NLog.Targets.RedisJson.Test;

public class RedisConnectionTest
{
    [Fact]
    public void Should_correctly_create_a_redis_connection()
    {
        // Arrange
        using var conn = new RedisConnection("localhost", 6379, 11);

        // Act

        // Assert
        var config = (conn.GetPrivateField("_config") as ConfigurationOptions)!;
        Assert.Single(config.EndPoints);

        var f = (config.EndPoints.First() as DnsEndPoint)!;

        Assert.Equal("localhost", f.Host);
        Assert.Equal(6379, f.Port);
        Assert.Null(config.Password);
        Assert.False(config.AllowAdmin);

        Assert.Equal(11, conn.GetPrivateField("_db") as int?);
    }

    [Fact]
    public void Should_correctly_create_a_redis_connection_with_all_parameters()
    {
        // Arrange
        using var conn = new RedisConnection("localhost", 6379, 112, "password", TimeSpan.FromSeconds(11), "allowAdmin=true");

        // Act
        // Assert
        var config = (conn.GetPrivateField("_config") as ConfigurationOptions)!;
        Assert.Single(config.EndPoints);

        var f = (config.EndPoints.First() as DnsEndPoint)!;

        Assert.Equal("localhost", f.Host);
        Assert.Equal(6379, f.Port);
        Assert.Equal("password", config.Password);
        Assert.True(config.AllowAdmin);

        Assert.Equal(112, conn.GetPrivateField("_db") as int?);
        Assert.Equal(11, (conn.GetPrivateField("_ttl") as TimeSpan?)?.TotalSeconds);
    }

    [Fact]
    public void Should_correctly_write_to_server()
    {
        // Arrange
        using var conn = new RedisConnection("localhost", 6379, 1);

        var connection = Substitute.For<IConnectionMultiplexer>();
        var db = Substitute.For<IDatabase>();
        var server = Substitute.For<IServer>();
        var ep = Substitute.For<EndPoint>();

        var result = RedisResult.Create(new RedisValue("OK"));

        conn.SetPrivateField("_connection", connection);
        conn.SetPrivateField("_database", db);

        server.Version.Returns(new Version(7, 1, 242));
        db.Multiplexer.GetEndPoints().Returns(new EndPoint[] { ep });
        db.Multiplexer.GetServer(ep).Returns(server);
        db.Execute(Arg.Any<string>(), Arg.Any<object[]>()).Returns(result);

        // Act
        conn.WriteJson("key", "value");

        // Assert
        db.DidNotReceive().KeyExpire("key", Arg.Any<TimeSpan>());
    }

    [Fact]
    public void Should_correctly_write_to_server_with_ttl()
    {
        // Arrange
        using var conn = new RedisConnection("localhost", 6379, 1, ttl: TimeSpan.FromSeconds(11));

        var connection = Substitute.For<IConnectionMultiplexer>();
        var db = Substitute.For<IDatabase>();
        var server = Substitute.For<IServer>();
        var ep = Substitute.For<EndPoint>();

        var result = RedisResult.Create(new RedisValue("OK"));

        conn.SetPrivateField("_connection", connection);
        conn.SetPrivateField("_database", db);

        server.Version.Returns(new Version(7, 1, 242));
        db.Multiplexer.GetEndPoints().Returns(new EndPoint[] { ep });
        db.Multiplexer.GetServer(ep).Returns(server);
        db.Execute(Arg.Any<string>(), Arg.Any<object[]>()).Returns(result);

        // Act
        conn.WriteJson("key", "value");

        // Assert
        db.Received(1).KeyExpire("key", TimeSpan.FromSeconds(11));
    }


}