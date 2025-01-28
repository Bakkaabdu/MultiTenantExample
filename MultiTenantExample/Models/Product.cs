namespace MultiTenantExample.Models
{
    public class Product : IMustHaveTenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string TenantId { get; set; } = null!;
    }
}
