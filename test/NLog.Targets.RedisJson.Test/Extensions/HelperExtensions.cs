using System.Reflection;

namespace NLog.Targets.RedisJson.Test.Extensions;

public static class HelperExtensions
{

    /// <summary>
    /// Set a private field
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="field">The field of the entity to setup</param>
    /// <param name="value">The value to set to the field</param>
    /// <typeparam name="TSource"></typeparam>
    public static void SetPrivateField<TSource>(this TSource entity, string field, object value)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        ArgumentNullException.ThrowIfNull(field);

        entity.GetType()?
            .GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)?
            .SetValue(entity, value);
    }

    /// <summary>
    /// Get a private field
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <param name="field">The field of the entity to setup</param>
    /// <param name="value">The value to set to the field</param>
    /// <typeparam name="TSource"></typeparam>
    public static object GetPrivateField<TSource>(this TSource entity, string field)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        ArgumentNullException.ThrowIfNull(field);

        return entity.GetType()?
            .GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)?
            .GetValue(entity) ?? throw new ArgumentException("Private field not found", field);
    }
}