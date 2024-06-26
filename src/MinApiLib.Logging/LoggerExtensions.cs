﻿using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MinApiLib.Logging;

[Obsolete("Use LoggerMessageAttribute(https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator) instead")]
public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> _loggerMessageCritical = LoggerMessage.Define<string>(LogLevel.Critical, new EventId(100, nameof(LogLevel.Critical)), "{Critical}");
    private static readonly Action<ILogger, string, Exception?> _loggerMessageDebug = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(200, nameof(LogLevel.Debug)), "{Debug}");
    private static readonly Action<ILogger, string, Exception?> _loggerMessageError = LoggerMessage.Define<string>(LogLevel.Error, new EventId(300, nameof(LogLevel.Error)), "{Error}");
    private static readonly Action<ILogger, string, Exception?> _loggerMessageInformation = LoggerMessage.Define<string>(LogLevel.Information, new EventId(400, nameof(LogLevel.Information)), "{Information}");
    private static readonly Action<ILogger, string, Exception?> _loggerMessageTrace = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(500, nameof(LogLevel.Trace)), "{Trace}");
    private static readonly Action<ILogger, string, Exception?> _loggerMessageWarning = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(600, nameof(LogLevel.Warning)), "{Warning}");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LogFaster(this ILogger logger, LogLevel logLevel, string message, Exception? ex)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);

        var @delegate = logLevel switch
        {
            LogLevel.Critical => _loggerMessageCritical,
            LogLevel.Debug => _loggerMessageDebug,
            LogLevel.Error => _loggerMessageError,
            LogLevel.Trace => _loggerMessageTrace,
            LogLevel.Warning => _loggerMessageWarning,
            _ => _loggerMessageInformation
        };

        @delegate(logger, message, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Critical(this ILogger logger, string message, Exception? ex = default)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);
        _loggerMessageCritical(logger, message, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Debug(this ILogger logger, string message)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);
        _loggerMessageDebug(logger, message, default);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Error(this ILogger logger, string message, Exception? ex = default)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);
        _loggerMessageError(logger, message, ex);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Information(this ILogger logger, string message)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);
        _loggerMessageInformation(logger, message, default);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Trace(this ILogger logger, string message)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);
        _loggerMessageTrace(logger, message, default);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Warning(this ILogger logger, string message, Exception? ex = default)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(message);
        _loggerMessageWarning(logger, message, ex);
    }
}
