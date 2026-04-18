using HKShop.Models;

namespace HKShop.Services.Interfaces;

public interface IHomeService
{
	ErrorViewModel BuildErrorModel(string traceIdentifier, string? requestId);
}
