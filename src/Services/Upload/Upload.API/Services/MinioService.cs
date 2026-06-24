using Minio;
using Minio.DataModel.Args;

namespace Upload.API.Services;

public class MinioService
{
	private readonly IMinioClient _minioClient;
	private readonly string _bucketName;

	public MinioService(IConfiguration configuration)
	{
		//TODO: admin123
		_minioClient = new MinioClient()
			.WithEndpoint(configuration["Minio:Endpoint"] ?? "localhost:9000")
			.WithCredentials(configuration["Minio:AccessKey"] ?? "admin", configuration["Minio:SecretKey"] ?? "admin123")
			.WithSSL(false)
			.Build();

		_bucketName = configuration["Minio:BucketName"] ?? "art-gallery";
	}

	public async Task EnsureBucketExistsAsync()
	{
		var args = new BucketExistsArgs().WithBucket(_bucketName);
		var found = await _minioClient.BucketExistsAsync(args);

		if (!found)
		{
			var makeArgs = new MakeBucketArgs().WithBucket(_bucketName);
			await _minioClient.MakeBucketAsync(makeArgs);
		}
	}

	public async Task<string> UploadFileAsync(string objectName, Stream fileStream, string contentType)
	{
		var args = new PutObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName)
			.WithStreamData(fileStream)
			.WithObjectSize(fileStream.Length)
			.WithContentType(contentType);

		await _minioClient.PutObjectAsync(args);

		// Генерируем ссылку на файл (можно сделать публичной или с подписью)
		return $"/images/{objectName}";
	}

	public async Task<bool> FileExistsAsync(string objectName)
	{
		var args = new StatObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName);

		try
		{
			await _minioClient.StatObjectAsync(args);
			return true;
		}
		catch
		{
			return false;
		}
	}
}