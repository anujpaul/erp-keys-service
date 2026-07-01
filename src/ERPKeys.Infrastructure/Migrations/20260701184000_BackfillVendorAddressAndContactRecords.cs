using ERPKeys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260701184000_BackfillVendorAddressAndContactRecords")]
public class BackfillVendorAddressAndContactRecords : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            INSERT INTO vendor_addresses (
                id, organization_id, vendor_id, label, address_type, is_primary,
                line1, line2, city, state, postal_code, country,
                created_at, updated_at, is_deleted)
            SELECT
                gen_random_uuid(), v.organization_id, v.id, 'Billing Address', 'Billing',
                NOT EXISTS (
                    SELECT 1
                    FROM vendor_addresses primary_address
                    WHERE primary_address.vendor_id = v.id
                      AND primary_address.is_primary
                      AND NOT primary_address.is_deleted),
                LEFT(BTRIM(v.billing_address), 300),
                NULLIF(SUBSTRING(BTRIM(v.billing_address) FROM 301 FOR 300), ''),
                '', NULL, NULL, '',
                CURRENT_TIMESTAMP::timestamp without time zone,
                CURRENT_TIMESTAMP::timestamp without time zone,
                false
            FROM vendors v
            WHERE NOT v.is_deleted
              AND NULLIF(BTRIM(v.billing_address), '') IS NOT NULL
              AND NOT EXISTS (
                  SELECT 1
                  FROM vendor_addresses address
                  WHERE address.vendor_id = v.id
                    AND address.address_type = 'Billing'
                    AND NOT address.is_deleted);

            INSERT INTO vendor_addresses (
                id, organization_id, vendor_id, label, address_type, is_primary,
                line1, line2, city, state, postal_code, country,
                created_at, updated_at, is_deleted)
            SELECT
                gen_random_uuid(), v.organization_id, v.id, 'Shipping Address', 'Shipping',
                NOT EXISTS (
                    SELECT 1
                    FROM vendor_addresses primary_address
                    WHERE primary_address.vendor_id = v.id
                      AND primary_address.is_primary
                      AND NOT primary_address.is_deleted),
                LEFT(BTRIM(v.shipping_address), 300),
                NULLIF(SUBSTRING(BTRIM(v.shipping_address) FROM 301 FOR 300), ''),
                '', NULL, NULL, '',
                CURRENT_TIMESTAMP::timestamp without time zone,
                CURRENT_TIMESTAMP::timestamp without time zone,
                false
            FROM vendors v
            WHERE NOT v.is_deleted
              AND NULLIF(BTRIM(v.shipping_address), '') IS NOT NULL
              AND NOT EXISTS (
                  SELECT 1
                  FROM vendor_addresses address
                  WHERE address.vendor_id = v.id
                    AND address.address_type = 'Shipping'
                    AND NOT address.is_deleted);

            INSERT INTO vendor_contacts (
                id, organization_id, vendor_id, name, title, email, phone, mobile,
                is_primary, notes, created_at, updated_at, is_deleted)
            SELECT
                gen_random_uuid(), v.organization_id, v.id, v.name, 'Primary Contact',
                v.email, v.phone, NULL, true, NULL,
                CURRENT_TIMESTAMP::timestamp without time zone,
                CURRENT_TIMESTAMP::timestamp without time zone,
                false
            FROM vendors v
            WHERE NOT v.is_deleted
              AND (
                  NULLIF(BTRIM(v.email), '') IS NOT NULL
                  OR NULLIF(BTRIM(v.phone), '') IS NOT NULL)
              AND NOT EXISTS (
                  SELECT 1
                  FROM vendor_contacts contact
                  WHERE contact.vendor_id = v.id
                    AND NOT contact.is_deleted);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Intentionally retained: migrated records may have been edited or used after creation.
    }
}
