using ERPKeys.Application.Common.Interfaces;
using ERPKeys.Application.Modules.Organization.DTOs;
using ERPKeys.Domain.Modules.GeneralLedger;
using ERPKeys.Domain.Modules.Organization;
using Microsoft.EntityFrameworkCore;

namespace ERPKeys.Application.Modules.Organization.Services;

public interface IOrganizationService
{
    Task<IEnumerable<OrganizationDto>> GetAccessibleAsync(
        Guid assignedOrganizationId,
        bool hasAllOrganizationAccess,
        CancellationToken ct = default);
    Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrganizationDto> CreateAsync(CreateOrganizationRequest req, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateOrganizationRequest req, CancellationToken ct = default);
    Task SuspendAsync(Guid id, CancellationToken ct = default);
    Task ActivateAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public class OrganizationService : IOrganizationService
{
    private readonly IAppDbContext _db;

    public OrganizationService(IAppDbContext db) => _db = db;

    public async Task<IEnumerable<OrganizationDto>> GetAccessibleAsync(
        Guid assignedOrganizationId,
        bool hasAllOrganizationAccess,
        CancellationToken ct = default)
    {
        var query = _db.Organizations.Where(o => !o.IsDeleted);
        if (!hasAllOrganizationAccess)
        {
            query = query.Where(o =>
                o.Id == assignedOrganizationId &&
                o.Status == OrganizationStatus.Active);
        }

        var orgs = await query
            .OrderBy(o => o.Name)
            .ToListAsync(ct);

        return orgs.Select(Map);
    }

    public async Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var org = await _db.Organizations
            .Where(o => o.Id == id && !o.IsDeleted)
            .FirstOrDefaultAsync(ct);

        return org is null ? null : Map(org);
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationRequest req, CancellationToken ct = default)
    {
        // Code must be unique
        var exists = await _db.Organizations
            .AnyAsync(o => o.Code == req.Code.ToUpperInvariant() && !o.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"An organization with code '{req.Code}' already exists.");

        var org = Domain.Modules.Organization.Organization.Create(
            req.Code, req.Name, req.BaseCurrency,
            req.FiscalYearStartMonth, req.Address, req.Phone, req.Email, req.TaxId);

        var standardDimensions = new[]
        {
            new FinancialDimension(org.Id, "COST_CENTER", "Cost Center",
                "Responsibility center used to track costs and profitability"),
            new FinancialDimension(org.Id, "DEPARTMENT", "Department",
                "Organizational department responsible for the transaction"),
            new FinancialDimension(org.Id, "BUSINESS_UNIT", "Business Unit",
                "Business or operating unit responsible for the transaction"),
            new FinancialDimension(org.Id, "PROJECT", "Project",
                "Project, initiative, or internal work stream")
        };
        var operatingSet = new FinancialDimensionSet(
            org.Id,
            "Operating Dimensions",
            "Standard dimensions used for operating ledger entries",
            true,
            standardDimensions.Select((dimension, index) =>
                (dimension.Id, IsRequired: index == 0)));

        _db.Organizations.Add(org);
        _db.FinancialDimensions.AddRange(standardDimensions);
        _db.FinancialDimensionSets.Add(operatingSet);
        await _db.SaveChangesAsync(ct);

        return Map(org);
    }

    public async Task UpdateAsync(Guid id, UpdateOrganizationRequest req, CancellationToken ct = default)
    {
        var org = await _db.Organizations.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Organization not found.");

        org.Update(req.Name, req.Address, req.Phone, req.Email, req.TaxId, req.LogoUrl);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SuspendAsync(Guid id, CancellationToken ct = default)
    {
        var org = await _db.Organizations.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Organization not found.");
        org.Suspend();
        await _db.SaveChangesAsync(ct);
    }

    public async Task ActivateAsync(Guid id, CancellationToken ct = default)
    {
        var org = await _db.Organizations.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Organization not found.");
        org.Activate();
        await _db.SaveChangesAsync(ct);
    }

    private static OrganizationDto Map(Domain.Modules.Organization.Organization o) => new(
        o.Id, o.Code, o.Name, o.BaseCurrency, o.FiscalYearStartMonth,
        o.Address, o.Phone, o.Email, o.TaxId, o.LogoUrl,
        o.Status.ToString(), o.CreatedAt);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var org = await _db.Organizations.FindAsync(new object[] { id }, ct)
            ?? throw new InvalidOperationException("Organization not found.");
        if (org.Code == "DEFAULT" || org.Code == "DEMO01")
            throw new InvalidOperationException("The default organization cannot be deleted.");
        org.SoftDelete();
        await _db.SaveChangesAsync(ct);
    }
}
