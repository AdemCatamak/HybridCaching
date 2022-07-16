using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;

namespace HybridCaching.Api.Controllers;

[ApiController]
public class Values : ControllerBase
{
    private readonly IEasyCachingProviderFactory _cachingProviderFactory;
    private readonly IHybridCachingProvider _cachingProvider;

    public Values(IEasyCachingProviderFactory cachingProviderFactory, IHybridCachingProvider cachingProvider)
    {
        _cachingProviderFactory = cachingProviderFactory;
        _cachingProvider = cachingProvider;
    }

    [HttpPut("keys/{key}/values")]
    public async Task<IActionResult> PutValuesAsync([FromRoute] string key, [FromBody] List<string> values)
    {
        await _cachingProvider.SetAsync(key, values, TimeSpan.FromSeconds(60));
        return StatusCode(StatusCodes.Status200OK, new
                                                   {
                                                       MachineName = Environment.MachineName
                                                   });
    }

    [HttpGet("keys/{key}/values")]
    public async Task<IActionResult> GetValuesAsync([FromRoute] string key)
    {
        CacheValue<List<string>> cacheValue = await _cachingProvider.GetAsync<List<string>>(key);
        return StatusCode(StatusCodes.Status200OK, new
                                                   {
                                                       MachineName = Environment.MachineName,
                                                       Values = cacheValue?.Value
                                                   });
    }

    [HttpGet("in-memory/keys/{key}/values")]
    public async Task<IActionResult> GetValuesFromInMemoryAsync([FromRoute] string key)
    {
        IEasyCachingProvider memoryCacheProvider = _cachingProviderFactory.GetCachingProvider("m");
        CacheValue<List<string>> cacheValue = await memoryCacheProvider.GetAsync<List<string>>(key);
        return StatusCode(StatusCodes.Status200OK, new
                                                   {
                                                       MachineName = Environment.MachineName,
                                                       Values = cacheValue?.Value
                                                   });
    }
}