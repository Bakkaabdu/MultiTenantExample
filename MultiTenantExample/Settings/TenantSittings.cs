namespace MultiTenantExample.Settings
{
    public class TenantSittings
    {
        public Configuration Defaults { get; set; } = null!;
        public List<Tenant> Tenants { get; set; } = new();
    }
}
