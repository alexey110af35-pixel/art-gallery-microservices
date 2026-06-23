namespace Shared.Config;

public static class JwtSettings
{
	private static string? _secretKey;
	private static string? _issuer;
	private static string? _audience;

	public static string SecretKey => _secretKey ??= ReadFromEnv("JWT__SECRET_KEY");
	public static string Issuer => _issuer ??= ReadFromEnv("JWT__ISSUER") ?? "ArtGallery";
	public static string Audience => _audience ??= ReadFromEnv("JWT__AUDIENCE") ?? "ArtGalleryUsers";

	private static string ReadFromEnv(string key)
	{
		return Environment.GetEnvironmentVariable(key)
			?? throw new InvalidOperationException($"JWT setting '{key}' is not configured");
	}
}