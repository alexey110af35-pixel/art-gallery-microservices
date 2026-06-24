using Confluent.Kafka;
using System.Text.Json;
using Gallery.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Kafka;

public class GalleryEventConsumer : BackgroundService
{
	private readonly ILogger<GalleryEventConsumer> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly string _topic;
	private readonly string _bootstrapServers;
	private readonly string _groupId;

	public GalleryEventConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<GalleryEventConsumer> logger)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
		_topic = configuration["Kafka:UploadedTopic"] ?? "image-uploaded";
		_groupId = "gallery-service-group";
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("🔄 GalleryEventConsumer: starting background task...");

		// Запускаем цикл в отдельном потоке, не блокируя главный
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

					_logger.LogInformation($"🔄 GalleryEventConsumer: connecting to Kafka at {_bootstrapServers}...");

					using var consumer = new ConsumerBuilder<string, string>(config).Build();
					consumer.Subscribe(_topic);
					_logger.LogInformation($"✅ GalleryEventConsumer subscribed to topic: {_topic}");

					while (!stoppingToken.IsCancellationRequested)
					{
						try
						{
							var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
							if (consumeResult != null)
							{
								_logger.LogInformation($"📥 Received event from {_topic}: Key={consumeResult.Message.Key}");
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
							_logger.LogError(ex, "❌ Unexpected error in GalleryEventConsumer loop");
							await Task.Delay(5000, stoppingToken);
						}
					}

					consumer.Close();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "❌ Fatal error in GalleryEventConsumer, retrying in 10 seconds...");
					await Task.Delay(10000, stoppingToken);
				}
			}
		}, stoppingToken);
	}

	private async Task ProcessEventAsync(Message<string, string> message, CancellationToken cancellationToken)
	{
		try
		{
			var evt = JsonSerializer.Deserialize<ImageUploadedEvent>(message.Value);
			if (evt == null || !evt.Success) return;

			using var scope = _serviceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			var painting = await dbContext.Paintings.FindAsync(new object[] { evt.PaintingId }, cancellationToken);
			if (painting != null)
			{
				painting.ImageUrl = evt.StorageUrl;
				painting.Status = Models.PaintingStatus.Ready;
				painting.UpdatedAt = DateTime.UtcNow;
				await dbContext.SaveChangesAsync(cancellationToken);
				_logger.LogInformation($"✅ Painting {evt.PaintingId} updated with image URL");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "❌ Error processing image uploaded event");
		}
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