using Azure;
using Azure.Data.Tables;
using System;

namespace DotNet.Training.VideoService.Function;

internal class VideoState : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string OutputValue { get; set; }

    public static VideoState FromState(string outputValue, string videoId, string functionName) => new VideoState()
    {
        PartitionKey = videoId,
        RowKey = functionName,
        OutputValue = outputValue,
        Timestamp = DateTimeOffset.UtcNow,
    };
}
