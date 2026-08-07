using HKShop.Domain;

namespace HKShop.Services.Interfaces;

public interface IHomeService
{
	ErrorViewModel BuildErrorModel(string traceIdentifier, string? requestId);
}
