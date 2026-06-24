using Confluent.Kafka;
using System.Text.Json;

namespace Gallery.API.Kafka;

public class GalleryEventProducer
{
	private readonly IProducer<string, string> _producer;
	private readonly string _topic;

	public GalleryEventProducer(IConfiguration configuration)
	{
		var config = new ProducerConfig
		{
			BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
		};

		_producer = new ProducerBuilder<string, string>(config).Build();
		_topic = configuration["Kafka:RequestTopic"] ?? "image-upload-requested";
	}

	public async Task PublishImageUploadRequestedAsync(ImageUploadRequestedEvent evt)
	{
		var message = JsonSerializer.Serialize(evt);
		await _producer.ProduceAsync(_topic, new Message<string, string> { Key = evt.PaintingId.ToString(), Value = message });
	}
}

public class ImageUploadRequestedEvent
{
	public Guid PaintingId { get; set; }
	public string FileName { get; set; } = string.Empty;
	public string ContentType { get; set; } = string.Empty;
	public long FileSize { get; set; }
}