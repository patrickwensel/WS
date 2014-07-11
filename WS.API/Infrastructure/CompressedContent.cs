using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WS.API.Infrastructure
{
	public class CompressedContent : HttpContent
	{
		private readonly HttpContent _originalContent;
		private readonly string _encodingType;
		private readonly CompressionMode _compressionMode;

		public CompressedContent(HttpContent content, string encodingType, CompressionMode compressionMode)
		{
			if (content == null) {
				throw new ArgumentNullException("content");
			}

			if (encodingType == null) {
				throw new ArgumentNullException("encodingType");
			}

			_originalContent = content;
			_encodingType = encodingType.ToLowerInvariant();
			_compressionMode = compressionMode;

			if (this._encodingType != "gzip" && this._encodingType != "deflate") {
				throw new InvalidOperationException(string.Format("Encoding '{0}' is not supported. Only supports gzip or deflate encoding.", this._encodingType));
			}

			// copy the headers from the original content
			foreach (KeyValuePair<string, IEnumerable<string>> header in _originalContent.Headers) {
				this.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			this.Headers.ContentEncoding.Add(encodingType);
		}

		protected override bool TryComputeLength(out long length)
		{
			length = -1;

			return false;
		}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			Stream compressedStream = null;

			if (_encodingType == "gzip") {
				compressedStream = new GZipStream(stream, _compressionMode, leaveOpen: true);
			} else if (_encodingType == "deflate") {
				compressedStream = new DeflateStream(stream, _compressionMode, leaveOpen: true);
			}

			return _originalContent.CopyToAsync(compressedStream).ContinueWith(tsk => {
				if (compressedStream != null) {
					compressedStream.Dispose();
				}
			});
		}
	}
}