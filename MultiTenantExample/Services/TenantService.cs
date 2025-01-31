
using Microsoft.Extensions.Options;

namespace MultiTenantExample.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantSittings _tenantSettings;
        private HttpContext? _httpContext;
        private Tenant? _currentTenant;

        public TenantService(IHttpContextAccessor contextAccessor, IOptions<TenantSittings> tenantSettings)
        {
            _httpContext = contextAccessor.HttpContext;
            _tenantSettings = tenantSettings.Value;

            if (_httpContext is not null)
            {
                if (_httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
                {
                    SetCurrentTenant(tenantId!);
                }
                else
                {
                    throw new Exception("No Tenant Provided!");
                }
            }
        }

        public string? GetConnectionString()
        {
            var currentConnectionString = _currentTenant is null
                ? _tenantSettings.Defaults.ConnectionString
                : _currentTenant.ConnectionString;
            return currentConnectionString;
        }

        public Tenant? GetCurrentTenant()
        {
            return _currentTenant;
        }

        public string? GetDatabaseProvider()
        {
            return _tenantSettings.Defaults.DBProvider;
        }

        private void SetCurrentTenant(string tenantId)
        {
            _currentTenant = _tenantSettings.Tenants.FirstOrDefault(t => t.TId == tenantId);
            if (_currentTenant is null)
                throw new Exception("Invalid tenant Id");
            if (String.IsNullOrEmpty(_currentTenant.ConnectionString))
            {
                _currentTenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
            }
        }
    }
}
