using Confluent.Kafka;
using System.Text.Json;

namespace Upload.API.Kafka;

public class UploadEventProducer
{
	private readonly IProducer<string, string> _producer;
	private readonly string _topic;

	public UploadEventProducer(IConfiguration configuration)
	{
		var config = new ProducerConfig
		{
			BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
		};

		_producer = new ProducerBuilder<string, string>(config).Build();
		_topic = configuration["Kafka:UploadedTopic"] ?? "image-uploaded";
	}

	public async Task PublishImageUploadedEventAsync(ImageUploadedEvent evt)
	{
		var message = JsonSerializer.Serialize(evt);
		await _producer.ProduceAsync(_topic, new Message<string, string> { Key = evt.ImageId.ToString(), Value = message });
	}
}

public class ImageUploadedEvent
{
	public Guid ImageId { get; set; }
	public Guid PaintingId { get; set; }
	public string FileName { get; set; } = string.Empty;
	public string StorageUrl { get; set; } = string.Empty;
	public bool Success { get; set; }
	public string? ErrorMessage { get; set; }
}