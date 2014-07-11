using System.Net.Http;

namespace WS.API.Infrastructure
{
	public class MultipartContentDispositionAwareFileStreamProvider : MultipartFileStreamProvider
	{
		public MultipartContentDispositionAwareFileStreamProvider(string rootPath)
			: base(rootPath) { }

		public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
		{
			var filename = headers.ContentDisposition.FileName;
			filename = filename.Trim('"');

			return filename;
		}
	}
}