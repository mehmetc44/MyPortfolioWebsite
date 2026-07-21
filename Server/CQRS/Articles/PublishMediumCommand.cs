using MediatR;
using Server.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record PublishMediumCommand(string Id, string? Token) : IRequest<PublishMediumResult>;

    public class PublishMediumResult
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string Message { get; set; } = "";
    }

    public class PublishMediumCommandHandler : IRequestHandler<PublishMediumCommand, PublishMediumResult>
    {
        private readonly AppDbContext _context;

        public PublishMediumCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PublishMediumResult> Handle(PublishMediumCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var article = await _context.Articles.FindAsync(new object[] { id }, cancellationToken);
            if (article == null)
            {
                return new PublishMediumResult { Success = false, Message = "Makale bulunamadı." };
            }

            var mediumToken = !string.IsNullOrWhiteSpace(request.Token)
                ? request.Token
                : (Environment.GetEnvironmentVariable("MEDIUM_INTEGRATION_TOKEN") 
                   ?? Environment.GetEnvironmentVariable("MEDIUM_TOKEN"));

            if (string.IsNullOrWhiteSpace(mediumToken))
            {
                return new PublishMediumResult 
                { 
                    Success = false, 
                    Message = "Medium Integration Token bulunamadı. Lütfen token girin veya .env dosyasına MEDIUM_INTEGRATION_TOKEN ekleyin." 
                };
            }

            try
            {
                using var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", mediumToken);

                var meRes = await httpClient.GetAsync("https://api.medium.com/v1/me", cancellationToken);
                if (!meRes.IsSuccessStatusCode)
                {
                    var meErr = await meRes.Content.ReadAsStringAsync(cancellationToken);
                    return new PublishMediumResult { Success = false, Message = $"Medium Kullanıcı Bilgisi Alınamadı ({meRes.StatusCode}): {meErr}" };
                }

                var meJson = await meRes.Content.ReadAsStringAsync(cancellationToken);
                using var doc = System.Text.Json.JsonDocument.Parse(meJson);
                var userId = doc.RootElement.GetProperty("data").GetProperty("id").GetString();

                if (string.IsNullOrEmpty(userId))
                {
                    return new PublishMediumResult { Success = false, Message = "Medium Kullanıcı ID'si okunamadı." };
                }

                var title = !string.IsNullOrWhiteSpace(article.Title_TR) ? article.Title_TR : (!string.IsNullOrWhiteSpace(article.Title_EN) ? article.Title_EN : article.Title_DE);
                var excerpt = !string.IsNullOrWhiteSpace(article.Excerpt_TR) ? article.Excerpt_TR : (!string.IsNullOrWhiteSpace(article.Excerpt_EN) ? article.Excerpt_EN : article.Excerpt_DE);
                var detail = !string.IsNullOrWhiteSpace(article.DetailText_TR) ? article.DetailText_TR : (!string.IsNullOrWhiteSpace(article.DetailText_EN) ? article.DetailText_EN : article.DetailText_DE);

                var htmlContent = $"<h1>{title}</h1>";
                if (!string.IsNullOrWhiteSpace(excerpt))
                {
                    htmlContent += $"<p><strong>{excerpt}</strong></p><hr/>";
                }
                htmlContent += detail;

                var categoryTag = !string.IsNullOrWhiteSpace(article.Category) ? article.Category.ToLower() : "technology";

                var postPayload = new
                {
                    title = title,
                    contentFormat = "html",
                    content = htmlContent,
                    tags = new[] { categoryTag, "software", "tech" },
                    publishStatus = "public"
                };

                var postJson = System.Text.Json.JsonSerializer.Serialize(postPayload);
                using var postContent = new System.Net.Http.StringContent(postJson, System.Text.Encoding.UTF8, "application/json");

                var postRes = await httpClient.PostAsync($"https://api.medium.com/v1/users/{userId}/posts", postContent, cancellationToken);
                if (!postRes.IsSuccessStatusCode)
                {
                    var postErr = await postRes.Content.ReadAsStringAsync(cancellationToken);
                    return new PublishMediumResult { Success = false, Message = $"Medium Paylaşım Hatası ({postRes.StatusCode}): {postErr}" };
                }

                var postResJson = await postRes.Content.ReadAsStringAsync(cancellationToken);
                using var postDoc = System.Text.Json.JsonDocument.Parse(postResJson);
                var mediumUrl = postDoc.RootElement.GetProperty("data").GetProperty("url").GetString();

                return new PublishMediumResult { Success = true, Url = mediumUrl, Message = "Makale Medium'da başarıyla yayınlandı!" };
            }
            catch (Exception ex)
            {
                return new PublishMediumResult { Success = false, Message = $"Medium paylaşımı sırasında sunucu hatası: {ex.Message}" };
            }
        }
    }
}
