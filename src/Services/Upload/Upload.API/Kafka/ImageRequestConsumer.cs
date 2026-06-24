using Confluent.Kafka;
using System.Text.Json;
using Upload.API.Services;

namespace Upload.API.Kafka;

public class ImageRequestConsumer : BackgroundService
{
	private readonly ILogger<ImageRequestConsumer> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly string _topic;
	private readonly string _bootstrapServers;
	private readonly string _groupId;

	public ImageRequestConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<ImageRequestConsumer> logger)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
		_topic = configuration["Kafka:RequestTopic"] ?? "image-upload-requested";
		_groupId = "upload-service-group";
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("🔄 ImageRequestConsumer: starting background task...");

		await Task.Run(async () =>
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var config = new ConsumerConfig
					{
						BootstrapServers = _bootstrapServers,
						GroupId = _groupId,
						AutoOffsetReset = AutoOffsetReset.Earliest,
						EnableAutoCommit = false
					};

					_logger.LogInformation($"🔄 ImageRequestConsumer: connecting to Kafka at {_bootstrapServers}...");

					using var consumer = new ConsumerBuilder<string, string>(config).Build();
					consumer.Subscribe(_topic);
					_logger.LogInformation($"✅ ImageRequestConsumer subscribed to topic: {_topic}");

					while (!stoppingToken.IsCancellationRequested)
					{
						try
						{
							var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
							if (consumeResult != null)
							{
								_logger.LogInformation($"📥 Received message from {_topic}: Key={consumeResult.Message.Key}");
								await ProcessEventAsync(consumeResult.Message, stoppingToken);
								consumer.Commit(consumeResult);
							}
						}
						catch (OperationCanceledException)
						{
							break;
						}
						catch (ConsumeException ex)
						{
							_logger.LogError(ex, $"❌ Consume error: {ex.Error.Reason}");
							await Task.Delay(5000, stoppingToken);
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "❌ Unexpected error in ImageRequestConsumer loop");
							await Task.Delay(5000, stoppingToken);
						}
					}

					consumer.Close();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "❌ Fatal error in ImageRequestConsumer, retrying in 10 seconds...");
					await Task.Delay(10000, stoppingToken);
				}
			}
		}, stoppingToken);
	}

	private async Task ProcessEventAsync(Message<string, string> message, CancellationToken cancellationToken)
	{
		try
		{
			var request = JsonSerializer.Deserialize<ImageUploadRequestedEvent>(message.Value);
			if (request == null) return;

			_logger.LogInformation($"📥 Processing image for painting: {request.PaintingId}");

			using var scope = _serviceProvider.CreateScope();
			var minioService = scope.ServiceProvider.GetRequiredService<MinioService>();
			var eventProducer = scope.ServiceProvider.GetRequiredService<UploadEventProducer>();

			// Здесь должна быть реальная загрузка файла в MinIO
			// Сейчас эмулируем успешную загрузку
			var storageUrl = $"/images/{Guid.NewGuid()}_{request.FileName}";

			var uploadEvent = new ImageUploadedEvent
			{
				ImageId = Guid.NewGuid(),
				PaintingId = request.PaintingId,
				FileName = request.FileName,
				StorageUrl = storageUrl,
				Success = true
			};

			await eventProducer.PublishImageUploadedEventAsync(uploadEvent);
			_logger.LogInformation($"✅ Published image uploaded event for painting: {request.PaintingId}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "❌ Error processing image upload request");
		}
	}
}

public class ImageUploadRequestedEvent
{
	public Guid PaintingId { get; set; }
	public string FileName { get; set; } = string.Empty;
	public string ContentType { get; set; } = string.Empty;
	public long FileSize { get; set; }
}