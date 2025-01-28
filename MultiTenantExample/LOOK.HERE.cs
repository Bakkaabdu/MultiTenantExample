/*
        ==================================================
        Multi-Tenancy Implementation with Entity Framework Core
        ==================================================

        1. **What is Multi-Tenancy?**
           - A design pattern where a single application serves multiple tenants (clients/customers) with isolated data.
           - Each tenant's data is segregated using a `TenantId` column in shared database tables.

        2. **Tenant Entity Interface (`ITenantEntity`)**
           - Defines a contract for entities requiring tenant isolation.
           - Ensures all tenant-specific entities implement a `TenantId` property.
           - Example:
             public interface ITenantEntity {
                 string TenantId { get; set; }
             }

        3. **Entity Configuration**
           - Entities implement `ITenantEntity` to include the `TenantId` property.
           - Example:
             public class Product : ITenantEntity {
                 public int Id { get; set; }
                 public string Name { get; set; }
                 public string TenantId { get; set; } // Tenant identifier
             }

        4. **Global Query Filter (EF Core)**
           - Automatically applies a filter to queries for `ITenantEntity` types.
           - Ensures users only access data for their tenant.
           - Added in `OnModelCreating`:
             foreach (var entityType in modelBuilder.Model.GetEntityTypes())
             {
                 if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                 {
                     modelBuilder.Entity(entityType.ClrType)
                         .HasQueryFilter(e => EF.Property<string>(e, "TenantId") == _currentTenantId);
                 }
             }

        5. **Automatic TenantId Assignment**
           - Override `SaveChanges` to set `TenantId` for new `ITenantEntity` entries.
           - Example:
             public override int SaveChanges()
             {
                 foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
                 {
                     if (entry.State == EntityState.Added)
                     {
                         entry.Entity.TenantId = _currentTenantService.GetTenantId();
                     }
                 }
                 return base.SaveChanges();
             }

        6. **Best Practices**
           - **Security:** Never expose `TenantId` to clients.
           - **Validation:** Prevent manual updates to `TenantId` post-creation.
           - **Async:** Use `SaveChangesAsync` for non-blocking operations.
*/
