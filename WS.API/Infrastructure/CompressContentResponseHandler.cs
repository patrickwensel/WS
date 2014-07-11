using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WS.API.Infrastructure
{
	public class CompressContentResponseHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((responseToCompleteTask) => {
				HttpResponseMessage response = responseToCompleteTask.Result;

				if (response.Content != null && response.RequestMessage.Headers.AcceptEncoding != null && response.RequestMessage.Headers.AcceptEncoding.Count > 0) {
					var encodingType = response.RequestMessage.Headers.AcceptEncoding.First().Value;
					response.Content = new CompressedContent(response.Content, encodingType, CompressionMode.Compress);
				}

				return response;
			},
			TaskContinuationOptions.OnlyOnRanToCompletion);
		}
	}
}