using HKShop.Models;
using HKShop.Services.Interfaces;

namespace HKShop.Services;

public class HomeService : IHomeService
{
	public ErrorViewModel BuildErrorModel(string traceIdentifier, string? requestId)
	{
		return new ErrorViewModel
		{
			RequestId = requestId ?? traceIdentifier
		};
	}
}
