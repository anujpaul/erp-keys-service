using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace ERPKeys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateSequence<int>(
                name: "variant_number_block_seq",
                startValue: 1000000L,
                incrementBy: 1000,
                maxValue: 9999000L);

            migrationBuilder.CreateTable(
                name: "account_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nature = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts_payable_parameters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allow_purchase_order_over_receipt = table.Column<bool>(type: "boolean", nullable: false),
                    maximum_over_receipt_percent = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts_payable_parameters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    old_values = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    new_values = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bank_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    account_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    account_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    account_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bank_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    bank_branch = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    routing_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    account_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    iban = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: true),
                    swift_code = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    gl_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    current_balance = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    last_reconciled_balance = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    last_reconciled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "batch_job_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    job_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    cron_expression = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    local_inbox_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    local_processed_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    local_error_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    local_export_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    file_format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    export_file_name_pattern = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    auto_confirm_sales_orders = table.Column<bool>(type: "boolean", nullable: false),
                    last_run_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_run_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    last_run_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    last_run_files_processed = table.Column<int>(type: "integer", nullable: false),
                    last_run_rows_promoted = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_batch_job_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    website = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "campaigns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_audience = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    budget = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    actual_spend = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    linked_promotion_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    reach_count = table.Column<int>(type: "integer", nullable: false),
                    conversion_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaigns", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    parent_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(8,4)", nullable: false, defaultValue: 0m),
                    tax_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_categories_categories_parent_category_id",
                        column: x => x.parent_category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "charts_of_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_charts_of_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coupon_redemptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    coupon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pos_transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    discount_applied = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    redeemed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coupon_redemptions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    decimal_places = table.Column<int>(type: "integer", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    is_base = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    numeric_code = table.Column<int>(type: "integer", nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    rate_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    billing_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    shipping_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    payment_terms_days = table.Column<int>(type: "integer", nullable: false),
                    credit_limit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "document_chunks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    content_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    required_permission = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    chunk_index = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1536)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_chunks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "expense_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    gl_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    limit_per_claim = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expense_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "expense_reports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    report_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    employee_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    employee_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    period_start = table.Column<DateTime>(type: "date", nullable: false),
                    period_end = table.Column<DateTime>(type: "date", nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    approved_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    paid_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    approval_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    submitted_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejected_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expense_reports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "export_job_rows",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_job_config_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    exported_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_export_job_rows", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "financial_dimension_sets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimension_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "financial_dimensions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimensions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fiscal_calendars",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    calendar_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fiscal_calendars", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fixed_assets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    asset_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    category = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    acquisition_date = table.Column<DateTime>(type: "date", nullable: false),
                    acquisition_cost = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    purchase_order_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    supplier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    serial_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    gl_asset_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    gl_accumulated_depreciation_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    gl_depreciation_expense_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    depreciation_method = table.Column<string>(type: "text", nullable: false),
                    useful_life_years = table.Column<int>(type: "integer", nullable: false),
                    salvage_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    depreciation_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    accumulated_depreciation = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    last_depreciation_date = table.Column<DateTime>(type: "date", nullable: true),
                    total_estimated_units = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    units_produced_to_date = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fixed_assets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "import_jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    file_format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    total_rows = table.Column<int>(type: "integer", nullable: false),
                    success_rows = table.Column<int>(type: "integer", nullable: false),
                    failed_rows = table.Column<int>(type: "integer", nullable: false),
                    error_summary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    triggered_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_import_jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_programs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    points_per_dollar = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    dollar_per_point = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    redemption_threshold = table.Column<int>(type: "integer", nullable: false),
                    silver_threshold = table.Column<int>(type: "integer", nullable: false),
                    gold_threshold = table.Column<int>(type: "integer", nullable: false),
                    platinum_threshold = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loyalty_programs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "number_sequences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    area = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    display_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    prefix = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    include_year = table.Column<bool>(type: "boolean", nullable: false),
                    separator = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    padding = table.Column<int>(type: "integer", nullable: false),
                    next_number = table.Column<long>(type: "bigint", nullable: false),
                    allow_manual_override = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_number_sequences", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "operational_sites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_retail_store = table.Column<bool>(type: "boolean", nullable: false),
                    is_fulfillment_center = table.Column<bool>(type: "boolean", nullable: false),
                    is_return_center = table.Column<bool>(type: "boolean", nullable: false),
                    is_warehouse = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operational_sites", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    base_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    fiscal_year_start_month = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tax_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    default_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    money_decimal_places = table.Column<int>(type: "integer", nullable: false),
                    money_rounding_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    money_rounding_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_processor_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    provider_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    environment = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    endpoint_base_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    merchant_account_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    credential_secret_reference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    timeout_seconds = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_processor_configurations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_proposals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proposal_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    proposal_date = table.Column<DateTime>(type: "date", nullable: false),
                    payment_date = table.Column<DateTime>(type: "date", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bank_account = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    processed_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_proposals", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pos_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    store_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    external_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cashier_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cashier_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fulfillment_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    customer_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    customer_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    delivery_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    external_order_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    channel_notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tendered_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    change_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    coupon_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    coupon_discount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ar_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    retail_statement_id = table.Column<Guid>(type: "uuid", nullable: true),
                    processing_error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    source_file = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pos_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    discount_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    discount_value = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    buy_quantity = table.Column<int>(type: "integer", nullable: true),
                    get_quantity = table.Column<int>(type: "integer", nullable: true),
                    minimum_order_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    max_uses_total = table.Column<int>(type: "integer", nullable: false),
                    max_uses_per_customer = table.Column<int>(type: "integer", nullable: false),
                    used_count = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    apply_to_all_products = table.Column<bool>(type: "boolean", nullable: false),
                    applicable_skus = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "purchase_requisitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requisition_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    requested_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    department_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cost_center_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    needed_by_date = table.Column<DateTime>(type: "date", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    converted_to_po_id = table.Column<Guid>(type: "uuid", nullable: true),
                    converted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_requisitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "retail_statements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    store_id = table.Column<Guid>(type: "uuid", nullable: false),
                    statement_number = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    business_date = table.Column<DateTime>(type: "date", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transaction_count = table.Column<int>(type: "integer", nullable: false),
                    net_sales = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    cost_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ar_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ar_credit_note_id = table.Column<Guid>(type: "uuid", nullable: true),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    posted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    posting_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retail_statements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "retail_stores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    store_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    manager_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retail_stores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "retail_tender_settlements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    retail_statement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    processor_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    bank_transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    settled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retail_tender_settlements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "retail_transaction_staging",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_file = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    source_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    raw_xml = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    store_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transaction_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    operator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    business_date = table.Column<DateTime>(type: "date", nullable: false),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    is_return = table.Column<bool>(type: "boolean", nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    validation_message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    promoted_transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    retail_statement_id = table.Column<Guid>(type: "uuid", nullable: true),
                    validated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    promoted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retail_transaction_staging", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_system_role = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "variant_attribute_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_variant_attribute_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vendors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    billing_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    shipping_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    payment_terms_days = table.Column<int>(type: "integer", nullable: false),
                    tax_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    bank_account_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    bank_account_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    bank_routing_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_exported = table.Column<bool>(type: "boolean", nullable: false),
                    exported_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warehouse_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workflow_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    document_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    amount_threshold = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bank_reconciliations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reconciliation_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    statement_start_date = table.Column<DateTime>(type: "date", nullable: false),
                    statement_end_date = table.Column<DateTime>(type: "date", nullable: false),
                    statement_opening_balance = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    statement_closing_balance = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    system_opening_balance = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    reconciled_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_reconciliations", x => x.id);
                    table.ForeignKey(
                        name: "fk_bank_reconciliations_bank_accounts_bank_account_id",
                        column: x => x.bank_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bank_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transaction_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    counterparty_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ar_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ap_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    transfer_to_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reconciliation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reconciled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    posted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    posted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bank_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_bank_transactions_bank_accounts_bank_account_id",
                        column: x => x.bank_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cash_journals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    journal_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    total_debits = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total_credits = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    posted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    posted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash_journals", x => x.id);
                    table.ForeignKey(
                        name: "fk_cash_journals_bank_accounts_bank_account_id",
                        column: x => x.bank_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chart_of_accounts_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    account_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_header_account = table.Column<bool>(type: "boolean", nullable: false),
                    allow_manual_entry = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_accounts_account_types_account_type_id",
                        column: x => x.account_type_id,
                        principalTable: "account_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_accounts_accounts_parent_account_id",
                        column: x => x.parent_account_id,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_accounts_charts_of_accounts_chart_of_accounts_id",
                        column: x => x.chart_of_accounts_id,
                        principalTable: "charts_of_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer_addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    line1 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    line2 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_addresses_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mobile = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_contacts_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sales_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_date = table.Column<DateTime>(type: "date", nullable: false),
                    requested_ship_date = table.Column<DateTime>(type: "date", nullable: true),
                    actual_ship_date = table.Column<DateTime>(type: "date", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    customer_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ar_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    delivered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    delivery_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_exported = table.Column<bool>(type: "boolean", nullable: false),
                    exported_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sales_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_sales_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sales_quotations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_date = table.Column<DateTime>(type: "date", nullable: false),
                    valid_until = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    customer_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    converted_to_so_id = table.Column<Guid>(type: "uuid", nullable: true),
                    converted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sales_quotations", x => x.id);
                    table.ForeignKey(
                        name: "fk_sales_quotations_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "expense_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_report_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    expense_date = table.Column<DateTime>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    merchant = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    receipt_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_reimbursable = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expense_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_expense_lines_expense_reports_expense_report_id",
                        column: x => x.expense_report_id,
                        principalTable: "expense_reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "financial_dimension_set_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimension_set_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_financial_dimension_set_members_financial_dimension_sets_fi",
                        column: x => x.financial_dimension_set_id,
                        principalTable: "financial_dimension_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_financial_dimension_set_members_financial_dimensions_financ",
                        column: x => x.financial_dimension_id,
                        principalTable: "financial_dimensions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "financial_dimension_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_dimension_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_financial_dimension_values_financial_dimensions_financial_d",
                        column: x => x.financial_dimension_id,
                        principalTable: "financial_dimensions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fiscal_years",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_calendar_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    calendar_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    period_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fiscal_years", x => x.id);
                    table.ForeignKey(
                        name: "fk_fiscal_years_fiscal_calendars_fiscal_calendar_id",
                        column: x => x.fiscal_calendar_id,
                        principalTable: "fiscal_calendars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ledgers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    functional_currency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reporting_currency_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fiscal_calendar_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chart_of_accounts_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ledgers", x => x.id);
                    table.ForeignKey(
                        name: "fk_ledgers_charts_of_accounts_chart_of_accounts_id",
                        column: x => x.chart_of_accounts_id,
                        principalTable: "charts_of_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledgers_currencies_functional_currency_id",
                        column: x => x.functional_currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledgers_currencies_reporting_currency_id",
                        column: x => x.reporting_currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ledgers_fiscal_calendars_fiscal_calendar_id",
                        column: x => x.fiscal_calendar_id,
                        principalTable: "fiscal_calendars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_depreciations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    running_nbv = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    posted_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_depreciations", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_depreciations_fixed_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "fixed_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asset_disposals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    disposal_date = table.Column<DateTime>(type: "date", nullable: false),
                    disposal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    disposal_proceeds = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    net_book_value_at_disposal = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    disposed_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    buyer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    gl_gain_loss_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_disposals", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_disposals_fixed_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "fixed_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asset_maintenances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    maintenance_date = table.Column<DateTime>(type: "date", nullable: false),
                    maintenance_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cost = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    capitalize_cost = table.Column<bool>(type: "boolean", nullable: false),
                    vendor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    performed_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    next_maintenance_due = table.Column<DateTime>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_maintenances", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_maintenances_fixed_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "fixed_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asset_transfers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transfer_date = table.Column<DateTime>(type: "date", nullable: false),
                    from_location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    to_location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    from_department = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    to_department = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    transferred_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_transfers", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_transfers_fixed_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "fixed_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "import_job_rows",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    import_job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_number = table.Column<int>(type: "integer", nullable: false),
                    raw_payload = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    promoted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    promoted_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_import_job_rows", x => x.id);
                    table.ForeignKey(
                        name: "fk_import_job_rows_import_jobs_import_job_id",
                        column: x => x.import_job_id,
                        principalTable: "import_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer_loyalty_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    loyalty_program_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    customer_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    total_points = table.Column<int>(type: "integer", nullable: false),
                    redeemed_points = table.Column<int>(type: "integer", nullable: false),
                    tier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    last_activity_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_loyalty_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_loyalty_accounts_loyalty_programs_loyalty_program_",
                        column: x => x.loyalty_program_id,
                        principalTable: "loyalty_programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    preferred_organization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    employee_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    job_title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    department = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    locale = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    header_theme_id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    sidebar_theme_id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    address_line1 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    address_line2 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    city = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    state = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false),
                    locked_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refresh_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_app_users_organizations_preferred_organization_id",
                        column: x => x.preferred_organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pos_payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pos_transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pos_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_pos_payments_pos_transactions_pos_transaction_id",
                        column: x => x.pos_transaction_id,
                        principalTable: "pos_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pos_transaction_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pos_transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    product_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_pct = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    line_sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    is_return = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pos_transaction_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_pos_transaction_lines_pos_transactions_pos_transaction_id",
                        column: x => x.pos_transaction_id,
                        principalTable: "pos_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    promotion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    max_uses = table.Column<int>(type: "integer", nullable: false),
                    used_count = table.Column<int>(type: "integer", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coupons", x => x.id);
                    table.ForeignKey(
                        name: "fk_coupons_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchase_requisition_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    requisition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estimated_unit_cost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    suggested_vendor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    gl_account_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_requisition_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_requisition_lines_purchase_requisitions_requisitio",
                        column: x => x.requisition_id,
                        principalTable: "purchase_requisitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "retail_transaction_staging_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    retail_transaction_staging_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pos_item_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    product_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    line_sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_return = table.Column<bool>(type: "boolean", nullable: false),
                    matched_product_variant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    validation_message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retail_transaction_staging_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_retail_transaction_staging_lines_retail_transaction_staging",
                        column: x => x.retail_transaction_staging_id,
                        principalTable: "retail_transaction_staging",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "retail_transaction_staging_tenders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    retail_transaction_staging_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retail_transaction_staging_tenders", x => x.id);
                    table.ForeignKey(
                        name: "fk_retail_transaction_staging_tenders_retail_transaction_stagi",
                        column: x => x.retail_transaction_staging_id,
                        principalTable: "retail_transaction_staging",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "variant_attribute_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    variant_attribute_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_variant_attribute_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_variant_attribute_values_variant_attribute_definitions_vari",
                        column: x => x.variant_attribute_definition_id,
                        principalTable: "variant_attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catalog_products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    long_description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    product_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    gender_target = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    base_cost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_rate_override = table.Column<decimal>(type: "numeric(8,4)", nullable: true),
                    sales_tax_group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    preferred_vendor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    variant_attribute_definition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    variant_number_base = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('variant_number_block_seq')"),
                    next_variant_number_offset = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_exported = table.Column<bool>(type: "boolean", nullable: false),
                    exported_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_catalog_products_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_catalog_products_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_catalog_products_variant_attribute_definitions_variant_attr",
                        column: x => x.variant_attribute_definition_id,
                        principalTable: "variant_attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_catalog_products_vendors_preferred_vendor_id",
                        column: x => x.preferred_vendor_id,
                        principalTable: "vendors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vendor_addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    line1 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    line2 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_vendor_addresses_vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vendor_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mobile = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_vendor_contacts_vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    warehouse_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warehouses", x => x.id);
                    table.ForeignKey(
                        name: "fk_warehouses_operational_sites_site_id",
                        column: x => x.site_id,
                        principalTable: "operational_sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_warehouses_warehouse_types_warehouse_type_id",
                        column: x => x.warehouse_type_id,
                        principalTable: "warehouse_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workflow_instances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    document_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    current_step_index = table.Column<int>(type: "integer", nullable: false),
                    total_steps = table.Column<int>(type: "integer", nullable: false),
                    submitted_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    rejected_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_instances", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_instances_workflow_templates_template_id",
                        column: x => x.template_id,
                        principalTable: "workflow_templates",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "workflow_template_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    step_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    approver_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    approver_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_template_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_template_steps_workflow_templates_workflow_templat",
                        column: x => x.workflow_template_id,
                        principalTable: "workflow_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cash_journal_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gl_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    debit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    credit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cash_journal_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_cash_journal_lines_cash_journals_journal_id",
                        column: x => x.journal_id,
                        principalTable: "cash_journals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accounts_receivable_parameters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allow_sales_order_invoice_variance = table.Column<bool>(type: "boolean", nullable: false),
                    maximum_invoice_variance_percent = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    trade_receivable_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sales_revenue_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sales_tax_payable_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cash_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    bank_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cost_of_goods_sold_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inventory_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts_receivable_parameters", x => x.id);
                    table.ForeignKey(
                        name: "fk_accounts_receivable_parameters_accounts_bank_account_id",
                        column: x => x.bank_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_receivable_parameters_accounts_cash_account_id",
                        column: x => x.cash_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_receivable_parameters_accounts_cost_of_goods_sold_",
                        column: x => x.cost_of_goods_sold_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_receivable_parameters_accounts_inventory_account_id",
                        column: x => x.inventory_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_receivable_parameters_accounts_sales_revenue_accou",
                        column: x => x.sales_revenue_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_receivable_parameters_accounts_sales_tax_payable_a",
                        column: x => x.sales_tax_payable_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounts_receivable_parameters_accounts_trade_receivable_ac",
                        column: x => x.trade_receivable_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "charge_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    module = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    calculation_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    default_value = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    is_taxable = table.Column<bool>(type: "boolean", nullable: false),
                    posting_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_charge_codes", x => x.id);
                    table.ForeignKey(
                        name: "fk_charge_codes_accounts_posting_account_id",
                        column: x => x.posting_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "methods_of_payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    usage = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tender_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    processor_configuration_id = table.Column<Guid>(type: "uuid", nullable: true),
                    settlement_bank_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    clearing_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fee_expense_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    requires_external_authorization = table.Column<bool>(type: "boolean", nullable: false),
                    auto_capture = table.Column<bool>(type: "boolean", nullable: false),
                    allow_refunds = table.Column<bool>(type: "boolean", nullable: false),
                    allow_manual_entry = table.Column<bool>(type: "boolean", nullable: false),
                    settlement_mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    settlement_delay_days = table.Column<int>(type: "integer", nullable: false),
                    settlement_cutoff_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_methods_of_payment", x => x.id);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_accounts_clearing_account_id",
                        column: x => x.clearing_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_accounts_fee_expense_account_id",
                        column: x => x.fee_expense_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_bank_accounts_settlement_bank_account_id",
                        column: x => x.settlement_bank_account_id,
                        principalTable: "bank_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_methods_of_payment_payment_processor_configurations_process",
                        column: x => x.processor_configuration_id,
                        principalTable: "payment_processor_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ar_invoices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sales_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_date = table.Column<DateTime>(type: "date", nullable: false),
                    due_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    paid_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ar_invoices", x => x.id);
                    table.ForeignKey(
                        name: "fk_ar_invoices_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ar_invoices_sales_orders_sales_order_id",
                        column: x => x.sales_order_id,
                        principalTable: "sales_orders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sales_quotation_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    variant_description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    discount_pct = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sales_quotation_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_sales_quotation_lines_sales_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalTable: "sales_quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fiscal_periods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_year_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_number = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fiscal_periods", x => x.id);
                    table.CheckConstraint("ck_fiscal_periods_period_number_range", "period_number BETWEEN 1 AND 13");
                    table.CheckConstraint("ck_fiscal_periods_status", "status IN ('Open', 'Closed', 'PermanentlyClosed')");
                    table.ForeignKey(
                        name: "fk_fiscal_periods_fiscal_years_fiscal_year_id",
                        column: x => x.fiscal_year_id,
                        principalTable: "fiscal_years",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accrual_schemes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ledger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    debit_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credit_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    allocation_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    default_period_count = table.Column<int>(type: "integer", nullable: false),
                    financial_dimension_set_id = table.Column<Guid>(type: "uuid", nullable: true),
                    financial_dimension_value_ids_json = table.Column<string>(type: "jsonb", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_schemes", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_accounts_credit_account_id",
                        column: x => x.credit_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_accounts_debit_account_id",
                        column: x => x.debit_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_financial_dimension_sets_financial_dimensio",
                        column: x => x.financial_dimension_set_id,
                        principalTable: "financial_dimension_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_schemes_ledgers_ledger_id",
                        column: x => x.ledger_id,
                        principalTable: "ledgers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "general_journal_voucher_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ledger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    journal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    lines_json = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_general_journal_voucher_templates", x => x.id);
                    table.ForeignKey(
                        name: "fk_general_journal_voucher_templates_ledgers_ledger_id",
                        column: x => x.ledger_id,
                        principalTable: "ledgers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "general_ledger_parameters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_ledger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    default_financial_dimension_set_id = table.Column<Guid>(type: "uuid", nullable: true),
                    retained_earnings_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rounding_difference_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    realized_gain_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    realized_loss_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unrealized_gain_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unrealized_loss_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    allow_posting_to_closed_periods = table.Column<bool>(type: "boolean", nullable: false),
                    require_dimensions_on_journal_lines = table.Column<bool>(type: "boolean", nullable: false),
                    maximum_penny_difference = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    default_journal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_general_ledger_parameters", x => x.id);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_accounts_realized_gain_account_id",
                        column: x => x.realized_gain_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_accounts_realized_loss_account_id",
                        column: x => x.realized_loss_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_accounts_retained_earnings_accoun",
                        column: x => x.retained_earnings_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_accounts_rounding_difference_acco",
                        column: x => x.rounding_difference_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_accounts_unrealized_gain_account_",
                        column: x => x.unrealized_gain_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_accounts_unrealized_loss_account_",
                        column: x => x.unrealized_loss_account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_financial_dimension_sets_default_",
                        column: x => x.default_financial_dimension_set_id,
                        principalTable: "financial_dimension_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_general_ledger_parameters_ledgers_default_ledger_id",
                        column: x => x.default_ledger_id,
                        principalTable: "ledgers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_roles_app_users_user_id",
                        column: x => x.user_id,
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_variants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    variant_number = table.Column<int>(type: "integer", nullable: false),
                    sku = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    size = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    material = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    additional_attributes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    price_override = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    cost_override = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    weight = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_variants", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_variants_catalog_products_product_id",
                        column: x => x.product_id,
                        principalTable: "catalog_products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inbound_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vendor_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    expected_date = table.Column<DateTime>(type: "date", nullable: false),
                    received_date = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbound_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_inbound_orders_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "outbound_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sales_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ship_to_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    requested_date = table.Column<DateTime>(type: "date", nullable: false),
                    shipped_date = table.Column<DateTime>(type: "date", nullable: true),
                    tracking_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    carrier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbound_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_outbound_orders_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchase_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    po_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_date = table.Column<DateTime>(type: "date", nullable: false),
                    expected_date = table.Column<DateTime>(type: "date", nullable: true),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    invoice_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    invoiced_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "text", nullable: true),
                    is_exported = table.Column<bool>(type: "boolean", nullable: false),
                    exported_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_orders_vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_purchase_orders_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transfer_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    from_warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_date = table.Column<DateTime>(type: "date", nullable: false),
                    shipped_date = table.Column<DateTime>(type: "date", nullable: true),
                    received_date = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transfer_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_transfer_orders_warehouses_from_warehouse_id",
                        column: x => x.from_warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transfer_orders_warehouses_to_warehouse_id",
                        column: x => x.to_warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    zone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    aisle = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    bay = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    bin = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_pickable = table.Column<bool>(type: "boolean", nullable: false),
                    is_receivable = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warehouse_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_warehouse_locations_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workflow_approval_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    step_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    approver_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    approver_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    decision = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    acted_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    acted_by_comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    acted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_approval_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_approval_steps_workflow_instances_workflow_instanc",
                        column: x => x.workflow_instance_id,
                        principalTable: "workflow_instances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ar_payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ar_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_date = table.Column<DateTime>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ar_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_ar_payments_ar_invoices_ar_invoice_id",
                        column: x => x.ar_invoice_id,
                        principalTable: "ar_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ar_payments_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer_credit_notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credit_note_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ar_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sales_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    credit_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    customer_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    applied_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reason = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_credit_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_credit_notes_ar_invoices_ar_invoice_id",
                        column: x => x.ar_invoice_id,
                        principalTable: "ar_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_customer_credit_notes_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dunning_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ar_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dunning_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sent_date = table.Column<DateTime>(type: "date", nullable: false),
                    follow_up_date = table.Column<DateTime>(type: "date", nullable: false),
                    outstanding_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    assigned_to = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    resolution_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dunning_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_dunning_records_ar_invoices_ar_invoice_id",
                        column: x => x.ar_invoice_id,
                        principalTable: "ar_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dunning_records_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journal_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ledger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    entry_date = table.Column<DateTime>(type: "date", nullable: false),
                    fiscal_period_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    journal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    total_debit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total_credit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    reversal_of_journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reversed_by_journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_entries_fiscal_periods_fiscal_period_id",
                        column: x => x.fiscal_period_id,
                        principalTable: "fiscal_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_journal_entries_journal_entries_reversal_of_journal_entry_id",
                        column: x => x.reversal_of_journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_entries_journal_entries_reversed_by_journal_entry_id",
                        column: x => x.reversed_by_journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_entries_ledgers_ledger_id",
                        column: x => x.ledger_id,
                        principalTable: "ledgers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accrual_posting_runs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    accrual_scheme_id = table.Column<Guid>(type: "uuid", nullable: false),
                    posted_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    start_fiscal_period_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    posted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_posting_runs", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_posting_runs_accrual_schemes_accrual_scheme_id",
                        column: x => x.accrual_scheme_id,
                        principalTable: "accrual_schemes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accrual_scheme_allocations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accrual_scheme_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_offset = table.Column<int>(type: "integer", nullable: false),
                    percentage = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_scheme_allocations", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_scheme_allocations_accrual_schemes_accrual_scheme_id",
                        column: x => x.accrual_scheme_id,
                        principalTable: "accrual_schemes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity_on_hand = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    quantity_reserved = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    quantity_on_order = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    reorder_point = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    minimum_stock = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    maximum_stock = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    average_cost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_count_date = table.Column<DateTime>(type: "date", nullable: true),
                    last_received_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_records_product_variants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    reference_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    reference_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    transaction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_transactions_product_variants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "price_agreements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    variant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    price_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_price_agreements", x => x.id);
                    table.ForeignKey(
                        name: "fk_price_agreements_catalog_products_product_id",
                        column: x => x.product_id,
                        principalTable: "catalog_products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_price_agreements_product_variants_variant_id",
                        column: x => x.variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sales_order_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sales_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    variant_description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    quantity_shipped = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    discount_pct = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sales_order_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_sales_order_lines_product_variants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sales_order_lines_sales_orders_sales_order_id",
                        column: x => x.sales_order_id,
                        principalTable: "sales_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ap_invoices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    invoice_date = table.Column<DateTime>(type: "date", nullable: false),
                    due_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    vendor_invoice_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    paid_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    prepayment_applied = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    linked_prepayment_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    match_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    match_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    bypass_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    workflow_instance_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ap_invoices", x => x.id);
                    table.ForeignKey(
                        name: "fk_ap_invoices_ap_invoices_linked_prepayment_invoice_id",
                        column: x => x.linked_prepayment_invoice_id,
                        principalTable: "ap_invoices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ap_invoices_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ap_invoices_vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchase_order_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ordered_qty = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    received_qty = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_order_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_order_lines_product_variants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_purchase_order_lines_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "outbound_order_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    outbound_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    product_sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    from_location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    requested_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    picked_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    shipped_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    lot_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbound_order_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_outbound_order_lines_outbound_orders_outbound_order_id",
                        column: x => x.outbound_order_id,
                        principalTable: "outbound_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_outbound_order_lines_warehouse_locations_from_location_id",
                        column: x => x.from_location_id,
                        principalTable: "warehouse_locations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "purchase_order_receipts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receipt_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    received_date = table.Column<DateTime>(type: "date", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    warehouse_location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_order_receipts", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_order_receipts_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_purchase_order_receipts_warehouse_locations_warehouse_locat",
                        column: x => x.warehouse_location_id,
                        principalTable: "warehouse_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_purchase_order_receipts_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transfer_order_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transfer_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    product_sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    from_location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    to_location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    requested_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    shipped_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    lot_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transfer_order_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_transfer_order_lines_transfer_orders_transfer_order_id",
                        column: x => x.transfer_order_id,
                        principalTable: "transfer_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfer_order_lines_warehouse_locations_from_location_id",
                        column: x => x.from_location_id,
                        principalTable: "warehouse_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_transfer_order_lines_warehouse_locations_to_location_id",
                        column: x => x.to_location_id,
                        principalTable: "warehouse_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_inventory_balances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity_on_hand = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    quantity_reserved = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warehouse_inventory_balances", x => x.id);
                    table.ForeignKey(
                        name: "fk_warehouse_inventory_balances_product_variants_product_varia",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_warehouse_inventory_balances_warehouse_locations_warehouse_",
                        column: x => x.warehouse_location_id,
                        principalTable: "warehouse_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_warehouse_inventory_balances_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    debit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    credit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    line_order = table.Column<int>(type: "integer", nullable: false),
                    financial_dimension_set_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_lines_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_journal_lines_financial_dimension_sets_financial_dimension_",
                        column: x => x.financial_dimension_set_id,
                        principalTable: "financial_dimension_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_lines_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accrual_posting_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accrual_posting_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_period_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_offset = table.Column<int>(type: "integer", nullable: false),
                    percentage = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accrual_posting_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_accrual_posting_lines_accrual_posting_runs_accrual_posting_",
                        column: x => x.accrual_posting_run_id,
                        principalTable: "accrual_posting_runs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_accrual_posting_lines_fiscal_periods_fiscal_period_id",
                        column: x => x.fiscal_period_id,
                        principalTable: "fiscal_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accrual_posting_lines_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ap_payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ap_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_date = table.Column<DateTime>(type: "date", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ap_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_ap_payments_ap_invoices_ap_invoice_id",
                        column: x => x.ap_invoice_id,
                        principalTable: "ap_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ap_payments_vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vendor_credit_notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credit_note_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ap_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    purchase_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    credit_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    vendor_cn_ref = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sub_total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    applied_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    reason = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_credit_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_vendor_credit_notes_ap_invoices_ap_invoice_id",
                        column: x => x.ap_invoice_id,
                        principalTable: "ap_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_vendor_credit_notes_vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ap_invoice_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ap_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(8,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ap_invoice_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_ap_invoice_lines_ap_invoices_ap_invoice_id",
                        column: x => x.ap_invoice_id,
                        principalTable: "ap_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ap_invoice_lines_purchase_order_lines_purchase_order_line_id",
                        column: x => x.purchase_order_line_id,
                        principalTable: "purchase_order_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inbound_order_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inbound_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    product_sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ordered_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    lot_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    expiry_date = table.Column<DateTime>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbound_order_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_inbound_order_lines_inbound_orders_inbound_order_id",
                        column: x => x.inbound_order_id,
                        principalTable: "inbound_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inbound_order_lines_purchase_order_lines_purchase_order_lin",
                        column: x => x.purchase_order_line_id,
                        principalTable: "purchase_order_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_inbound_order_lines_warehouse_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "warehouse_locations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "purchase_order_receipt_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    receipt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purchase_order_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    qty = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase_order_receipt_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_order_receipt_lines_purchase_order_lines_purchase_",
                        column: x => x.purchase_order_line_id,
                        principalTable: "purchase_order_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_purchase_order_receipt_lines_purchase_order_receipts_receip",
                        column: x => x.receipt_id,
                        principalTable: "purchase_order_receipts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journal_line_dimension_values",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    financial_dimension_value_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_line_dimension_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_line_dimension_values_financial_dimension_values_fi",
                        column: x => x.financial_dimension_value_id,
                        principalTable: "financial_dimension_values",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_line_dimension_values_journal_lines_journal_line_id",
                        column: x => x.journal_line_id,
                        principalTable: "journal_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_proposal_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    proposal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ap_invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    proposed_amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    invoice_due_date = table.Column<DateTime>(type: "date", nullable: false),
                    ap_payment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_proposal_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_proposal_lines_ap_invoices_ap_invoice_id",
                        column: x => x.ap_invoice_id,
                        principalTable: "ap_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payment_proposal_lines_ap_payments_ap_payment_id",
                        column: x => x.ap_payment_id,
                        principalTable: "ap_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_payment_proposal_lines_payment_proposals_proposal_id",
                        column: x => x.proposal_id,
                        principalTable: "payment_proposals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_account_type_id",
                table: "accounts",
                column: "account_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_chart_of_accounts_id_account_number",
                table: "accounts",
                columns: new[] { "chart_of_accounts_id", "account_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounts_parent_account_id",
                table: "accounts",
                column: "parent_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_payable_parameters_organization_id",
                table: "accounts_payable_parameters",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_bank_account_id",
                table: "accounts_receivable_parameters",
                column: "bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_cash_account_id",
                table: "accounts_receivable_parameters",
                column: "cash_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_cost_of_goods_sold_account_id",
                table: "accounts_receivable_parameters",
                column: "cost_of_goods_sold_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_inventory_account_id",
                table: "accounts_receivable_parameters",
                column: "inventory_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_organization_id",
                table: "accounts_receivable_parameters",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_sales_revenue_account_id",
                table: "accounts_receivable_parameters",
                column: "sales_revenue_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_sales_tax_payable_account_id",
                table: "accounts_receivable_parameters",
                column: "sales_tax_payable_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_receivable_parameters_trade_receivable_account_id",
                table: "accounts_receivable_parameters",
                column: "trade_receivable_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_lines_accrual_posting_run_id_period_offset",
                table: "accrual_posting_lines",
                columns: new[] { "accrual_posting_run_id", "period_offset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_lines_fiscal_period_id",
                table: "accrual_posting_lines",
                column: "fiscal_period_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_lines_journal_entry_id",
                table: "accrual_posting_lines",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_runs_accrual_scheme_id",
                table: "accrual_posting_runs",
                column: "accrual_scheme_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_posting_runs_organization_id_accrual_scheme_id_refe",
                table: "accrual_posting_runs",
                columns: new[] { "organization_id", "accrual_scheme_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accrual_scheme_allocations_accrual_scheme_id_period_offset",
                table: "accrual_scheme_allocations",
                columns: new[] { "accrual_scheme_id", "period_offset" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_credit_account_id",
                table: "accrual_schemes",
                column: "credit_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_debit_account_id",
                table: "accrual_schemes",
                column: "debit_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_financial_dimension_set_id",
                table: "accrual_schemes",
                column: "financial_dimension_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_ledger_id",
                table: "accrual_schemes",
                column: "ledger_id");

            migrationBuilder.CreateIndex(
                name: "ix_accrual_schemes_organization_id_code",
                table: "accrual_schemes",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoice_lines_ap_invoice_id_purchase_order_line_id",
                table: "ap_invoice_lines",
                columns: new[] { "ap_invoice_id", "purchase_order_line_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoice_lines_purchase_order_line_id",
                table: "ap_invoice_lines",
                column: "purchase_order_line_id");

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoices_linked_prepayment_invoice_id",
                table: "ap_invoices",
                column: "linked_prepayment_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoices_organization_id_invoice_number",
                table: "ap_invoices",
                columns: new[] { "organization_id", "invoice_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoices_purchase_order_id",
                table: "ap_invoices",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_ap_invoices_vendor_id",
                table: "ap_invoices",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_ap_payments_ap_invoice_id",
                table: "ap_payments",
                column: "ap_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_ap_payments_vendor_id",
                table: "ap_payments",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_app_users_organization_id_employee_id",
                table: "app_users",
                columns: new[] { "organization_id", "employee_id" },
                unique: true,
                filter: "employee_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_app_users_organization_id_username",
                table: "app_users",
                columns: new[] { "organization_id", "username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_preferred_organization_id",
                table: "app_users",
                column: "preferred_organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_ar_invoices_customer_id",
                table: "ar_invoices",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_ar_invoices_organization_id_invoice_number",
                table: "ar_invoices",
                columns: new[] { "organization_id", "invoice_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ar_invoices_sales_order_id",
                table: "ar_invoices",
                column: "sales_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_ar_payments_ar_invoice_id",
                table: "ar_payments",
                column: "ar_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_ar_payments_customer_id",
                table: "ar_payments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_depreciations_asset_id",
                table: "asset_depreciations",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_disposals_asset_id",
                table: "asset_disposals",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_maintenances_asset_id",
                table: "asset_maintenances",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_transfers_asset_id",
                table: "asset_transfers",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_organization_id_occurred_at",
                table: "audit_logs",
                columns: new[] { "organization_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_bank_accounts_organization_id_account_code",
                table: "bank_accounts",
                columns: new[] { "organization_id", "account_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bank_reconciliations_bank_account_id",
                table: "bank_reconciliations",
                column: "bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_bank_reconciliations_organization_id_reconciliation_number",
                table: "bank_reconciliations",
                columns: new[] { "organization_id", "reconciliation_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bank_transactions_bank_account_id_transaction_date",
                table: "bank_transactions",
                columns: new[] { "bank_account_id", "transaction_date" });

            migrationBuilder.CreateIndex(
                name: "ix_bank_transactions_organization_id_transaction_number",
                table: "bank_transactions",
                columns: new[] { "organization_id", "transaction_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bank_transactions_transaction_status",
                table: "bank_transactions",
                column: "transaction_status");

            migrationBuilder.CreateIndex(
                name: "ix_batch_job_configs_organization_id",
                table: "batch_job_configs",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_batch_job_configs_organization_id_is_enabled",
                table: "batch_job_configs",
                columns: new[] { "organization_id", "is_enabled" });

            migrationBuilder.CreateIndex(
                name: "ix_brands_organization_id_code",
                table: "brands",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_campaigns_organization_id",
                table: "campaigns",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_campaigns_status",
                table: "campaigns",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_cash_journal_lines_journal_id",
                table: "cash_journal_lines",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "ix_cash_journals_bank_account_id",
                table: "cash_journals",
                column: "bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_cash_journals_organization_id_journal_number",
                table: "cash_journals",
                columns: new[] { "organization_id", "journal_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cash_journals_status",
                table: "cash_journals",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_products_brand_id",
                table: "catalog_products",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_products_category_id",
                table: "catalog_products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_products_organization_id_sku",
                table: "catalog_products",
                columns: new[] { "organization_id", "sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_catalog_products_preferred_vendor_id",
                table: "catalog_products",
                column: "preferred_vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_products_variant_attribute_definition_id",
                table: "catalog_products",
                column: "variant_attribute_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_organization_id_code",
                table: "categories",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_categories_parent_category_id",
                table: "categories",
                column: "parent_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_charge_codes_organization_id_module_code",
                table: "charge_codes",
                columns: new[] { "organization_id", "module", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_charge_codes_posting_account_id",
                table: "charge_codes",
                column: "posting_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_charts_of_accounts_organization_id",
                table: "charts_of_accounts",
                column: "organization_id",
                unique: true,
                filter: "is_default = TRUE AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_charts_of_accounts_organization_id_code",
                table: "charts_of_accounts",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_coupon_redemptions_coupon_id",
                table: "coupon_redemptions",
                column: "coupon_id");

            migrationBuilder.CreateIndex(
                name: "ix_coupon_redemptions_pos_transaction_id",
                table: "coupon_redemptions",
                column: "pos_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_coupons_organization_id_code",
                table: "coupons",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_coupons_promotion_id",
                table: "coupons",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "ix_currencies_organization_id_code",
                table: "currencies",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customer_addresses_customer_id",
                table: "customer_addresses",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_contacts_customer_id",
                table: "customer_contacts",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_credit_notes_ar_invoice_id",
                table: "customer_credit_notes",
                column: "ar_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_credit_notes_customer_id",
                table: "customer_credit_notes",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_credit_notes_organization_id_credit_note_number",
                table: "customer_credit_notes",
                columns: new[] { "organization_id", "credit_note_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customer_loyalty_accounts_loyalty_program_id",
                table: "customer_loyalty_accounts",
                column: "loyalty_program_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_loyalty_accounts_organization_id_customer_id",
                table: "customer_loyalty_accounts",
                columns: new[] { "organization_id", "customer_id" });

            migrationBuilder.CreateIndex(
                name: "ix_customers_organization_id_customer_number",
                table: "customers",
                columns: new[] { "organization_id", "customer_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_embedding_hnsw",
                table: "document_chunks",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_content_hash",
                table: "document_chunks",
                columns: new[] { "organization_id", "content_hash" });

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_document_id",
                table: "document_chunks",
                columns: new[] { "organization_id", "document_id" });

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_document_id_chunk_index",
                table: "document_chunks",
                columns: new[] { "organization_id", "document_id", "chunk_index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_required_permission",
                table: "document_chunks",
                columns: new[] { "organization_id", "required_permission" });

            migrationBuilder.CreateIndex(
                name: "ix_document_chunks_organization_id_uploaded_by_user_id",
                table: "document_chunks",
                columns: new[] { "organization_id", "uploaded_by_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_dunning_records_ar_invoice_id",
                table: "dunning_records",
                column: "ar_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_dunning_records_customer_id",
                table: "dunning_records",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_dunning_records_organization_id_dunning_number",
                table: "dunning_records",
                columns: new[] { "organization_id", "dunning_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_expense_lines_expense_report_id",
                table: "expense_lines",
                column: "expense_report_id");

            migrationBuilder.CreateIndex(
                name: "ix_expense_reports_organization_id_report_number",
                table: "expense_reports",
                columns: new[] { "organization_id", "report_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_export_job_rows_batch_job_config_id_exported_at",
                table: "export_job_rows",
                columns: new[] { "batch_job_config_id", "exported_at" });

            migrationBuilder.CreateIndex(
                name: "ix_export_job_rows_entity_type_entity_id",
                table: "export_job_rows",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_export_job_rows_organization_id",
                table: "export_job_rows",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_set_members_financial_dimension_id",
                table: "financial_dimension_set_members",
                column: "financial_dimension_id");

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_set_members_financial_dimension_set_id_",
                table: "financial_dimension_set_members",
                columns: new[] { "financial_dimension_set_id", "financial_dimension_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_sets_organization_id",
                table: "financial_dimension_sets",
                column: "organization_id",
                unique: true,
                filter: "is_default = TRUE AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_sets_organization_id_name",
                table: "financial_dimension_sets",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimension_values_financial_dimension_id_code",
                table: "financial_dimension_values",
                columns: new[] { "financial_dimension_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_financial_dimensions_organization_id_code",
                table: "financial_dimensions",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_calendars_organization_id",
                table: "fiscal_calendars",
                column: "organization_id",
                unique: true,
                filter: "is_default = TRUE AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_calendars_organization_id_name",
                table: "fiscal_calendars",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_periods_fiscal_year_id_period_number",
                table: "fiscal_periods",
                columns: new[] { "fiscal_year_id", "period_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_years_fiscal_calendar_id_name",
                table: "fiscal_years",
                columns: new[] { "fiscal_calendar_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fixed_assets_organization_id_asset_code",
                table: "fixed_assets",
                columns: new[] { "organization_id", "asset_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_general_journal_voucher_templates_ledger_id",
                table: "general_journal_voucher_templates",
                column: "ledger_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_journal_voucher_templates_organization_id_user_id_n",
                table: "general_journal_voucher_templates",
                columns: new[] { "organization_id", "user_id", "name" },
                unique: true,
                filter: "is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_default_financial_dimension_set_id",
                table: "general_ledger_parameters",
                column: "default_financial_dimension_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_default_ledger_id",
                table: "general_ledger_parameters",
                column: "default_ledger_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_organization_id",
                table: "general_ledger_parameters",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_realized_gain_account_id",
                table: "general_ledger_parameters",
                column: "realized_gain_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_realized_loss_account_id",
                table: "general_ledger_parameters",
                column: "realized_loss_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_retained_earnings_account_id",
                table: "general_ledger_parameters",
                column: "retained_earnings_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_rounding_difference_account_id",
                table: "general_ledger_parameters",
                column: "rounding_difference_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_unrealized_gain_account_id",
                table: "general_ledger_parameters",
                column: "unrealized_gain_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_ledger_parameters_unrealized_loss_account_id",
                table: "general_ledger_parameters",
                column: "unrealized_loss_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_import_job_rows_import_job_id",
                table: "import_job_rows",
                column: "import_job_id");

            migrationBuilder.CreateIndex(
                name: "ix_import_job_rows_import_job_id_status",
                table: "import_job_rows",
                columns: new[] { "import_job_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_import_jobs_organization_id",
                table: "import_jobs",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_import_jobs_status",
                table: "import_jobs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_inbound_order_lines_inbound_order_id",
                table: "inbound_order_lines",
                column: "inbound_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_inbound_order_lines_location_id",
                table: "inbound_order_lines",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_inbound_order_lines_purchase_order_line_id",
                table: "inbound_order_lines",
                column: "purchase_order_line_id");

            migrationBuilder.CreateIndex(
                name: "ix_inbound_orders_organization_id_order_number",
                table: "inbound_orders",
                columns: new[] { "organization_id", "order_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inbound_orders_purchase_order_id",
                table: "inbound_orders",
                column: "purchase_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inbound_orders_warehouse_id",
                table: "inbound_orders",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_records_product_variant_id",
                table: "inventory_records",
                column: "product_variant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_organization_id_product_variant_id_t",
                table: "inventory_transactions",
                columns: new[] { "organization_id", "product_variant_id", "transaction_date" });

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_product_variant_id",
                table: "inventory_transactions",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_fiscal_period_id",
                table: "journal_entries",
                column: "fiscal_period_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_ledger_id",
                table: "journal_entries",
                column: "ledger_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_organization_id_entry_number",
                table: "journal_entries",
                columns: new[] { "organization_id", "entry_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_reversal_of_journal_entry_id",
                table: "journal_entries",
                column: "reversal_of_journal_entry_id",
                unique: true,
                filter: "reversal_of_journal_entry_id IS NOT NULL AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_reversed_by_journal_entry_id",
                table: "journal_entries",
                column: "reversed_by_journal_entry_id",
                unique: true,
                filter: "reversed_by_journal_entry_id IS NOT NULL AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_journal_line_dimension_values_financial_dimension_value_id",
                table: "journal_line_dimension_values",
                column: "financial_dimension_value_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_line_dimension_values_journal_line_id_financial_dim",
                table: "journal_line_dimension_values",
                columns: new[] { "journal_line_id", "financial_dimension_value_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_lines_account_id",
                table: "journal_lines",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_lines_financial_dimension_set_id",
                table: "journal_lines",
                column: "financial_dimension_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_lines_journal_entry_id",
                table: "journal_lines",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_chart_of_accounts_id",
                table: "ledgers",
                column: "chart_of_accounts_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_fiscal_calendar_id",
                table: "ledgers",
                column: "fiscal_calendar_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_functional_currency_id",
                table: "ledgers",
                column: "functional_currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_organization_id",
                table: "ledgers",
                column: "organization_id",
                unique: true,
                filter: "is_default = TRUE AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_organization_id_code",
                table: "ledgers",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ledgers_reporting_currency_id",
                table: "ledgers",
                column: "reporting_currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_loyalty_programs_organization_id",
                table: "loyalty_programs",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_clearing_account_id",
                table: "methods_of_payment",
                column: "clearing_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_fee_expense_account_id",
                table: "methods_of_payment",
                column: "fee_expense_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_organization_id_code",
                table: "methods_of_payment",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_processor_configuration_id",
                table: "methods_of_payment",
                column: "processor_configuration_id");

            migrationBuilder.CreateIndex(
                name: "ix_methods_of_payment_settlement_bank_account_id",
                table: "methods_of_payment",
                column: "settlement_bank_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_number_sequences_organization_id_area",
                table: "number_sequences",
                columns: new[] { "organization_id", "area" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_number_sequences_organization_id_display_name",
                table: "number_sequences",
                columns: new[] { "organization_id", "display_name" });

            migrationBuilder.CreateIndex(
                name: "ix_operational_sites_organization_id_code",
                table: "operational_sites",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organizations_code",
                table: "organizations",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbound_order_lines_from_location_id",
                table: "outbound_order_lines",
                column: "from_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_outbound_order_lines_outbound_order_id",
                table: "outbound_order_lines",
                column: "outbound_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_outbound_orders_organization_id_order_number",
                table: "outbound_orders",
                columns: new[] { "organization_id", "order_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbound_orders_warehouse_id",
                table: "outbound_orders",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_processor_configurations_organization_id_code",
                table: "payment_processor_configurations",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_proposal_lines_ap_invoice_id",
                table: "payment_proposal_lines",
                column: "ap_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_proposal_lines_ap_payment_id",
                table: "payment_proposal_lines",
                column: "ap_payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_proposal_lines_proposal_id",
                table: "payment_proposal_lines",
                column: "proposal_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_proposals_organization_id_proposal_number",
                table: "payment_proposals",
                columns: new[] { "organization_id", "proposal_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pos_payments_pos_transaction_id",
                table: "pos_payments",
                column: "pos_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_pos_transaction_lines_pos_transaction_id",
                table: "pos_transaction_lines",
                column: "pos_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_pos_transactions_organization_id_external_ref",
                table: "pos_transactions",
                columns: new[] { "organization_id", "external_ref" },
                unique: true,
                filter: "external_ref IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_pos_transactions_organization_id_transaction_number",
                table: "pos_transactions",
                columns: new[] { "organization_id", "transaction_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pos_transactions_retail_statement_id",
                table: "pos_transactions",
                column: "retail_statement_id");

            migrationBuilder.CreateIndex(
                name: "ix_pos_transactions_status",
                table: "pos_transactions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_price_agreements_organization_id_is_active_start_date_end_d",
                table: "price_agreements",
                columns: new[] { "organization_id", "is_active", "start_date", "end_date" });

            migrationBuilder.CreateIndex(
                name: "ix_price_agreements_product_id_price_type_is_active",
                table: "price_agreements",
                columns: new[] { "product_id", "price_type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_price_agreements_variant_id_price_type_is_active",
                table: "price_agreements",
                columns: new[] { "variant_id", "price_type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_barcode",
                table: "product_variants",
                column: "barcode",
                filter: "barcode IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_organization_id_sku",
                table: "product_variants",
                columns: new[] { "organization_id", "sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_organization_id_variant_number",
                table: "product_variants",
                columns: new[] { "organization_id", "variant_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_product_id",
                table: "product_variants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_promotions_organization_id",
                table: "promotions",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_lines_product_variant_id",
                table: "purchase_order_lines",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_lines_purchase_order_id",
                table: "purchase_order_lines",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_receipt_lines_purchase_order_line_id",
                table: "purchase_order_receipt_lines",
                column: "purchase_order_line_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_receipt_lines_receipt_id",
                table: "purchase_order_receipt_lines",
                column: "receipt_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_receipts_purchase_order_id",
                table: "purchase_order_receipts",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_receipts_receipt_number",
                table: "purchase_order_receipts",
                column: "receipt_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_receipts_warehouse_id",
                table: "purchase_order_receipts",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_order_receipts_warehouse_location_id",
                table: "purchase_order_receipts",
                column: "warehouse_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_orders_organization_id_order_date_created_at_id",
                table: "purchase_orders",
                columns: new[] { "organization_id", "order_date", "created_at", "id" },
                descending: new[] { false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_purchase_orders_organization_id_po_number",
                table: "purchase_orders",
                columns: new[] { "organization_id", "po_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_purchase_orders_organization_id_status_order_date_created_a",
                table: "purchase_orders",
                columns: new[] { "organization_id", "status", "order_date", "created_at", "id" },
                descending: new[] { false, false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_purchase_orders_organization_id_vendor_id_order_date_create",
                table: "purchase_orders",
                columns: new[] { "organization_id", "vendor_id", "order_date", "created_at", "id" },
                descending: new[] { false, false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_purchase_orders_vendor_id",
                table: "purchase_orders",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_orders_warehouse_id",
                table: "purchase_orders",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_requisition_lines_requisition_id",
                table: "purchase_requisition_lines",
                column: "requisition_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_requisitions_organization_id_requisition_number",
                table: "purchase_requisitions",
                columns: new[] { "organization_id", "requisition_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_retail_statements_organization_id_statement_number",
                table: "retail_statements",
                columns: new[] { "organization_id", "statement_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_retail_statements_organization_id_store_id_business_date_cu",
                table: "retail_statements",
                columns: new[] { "organization_id", "store_id", "business_date", "currency", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_retail_stores_organization_id_store_code",
                table: "retail_stores",
                columns: new[] { "organization_id", "store_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_retail_tender_settlements_organization_id_status",
                table: "retail_tender_settlements",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_retail_tender_settlements_retail_statement_id",
                table: "retail_tender_settlements",
                column: "retail_statement_id");

            migrationBuilder.CreateIndex(
                name: "ix_retail_transaction_staging_organization_id_source_hash",
                table: "retail_transaction_staging",
                columns: new[] { "organization_id", "source_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_retail_transaction_staging_organization_id_status_created_at",
                table: "retail_transaction_staging",
                columns: new[] { "organization_id", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_retail_transaction_staging_organization_id_transaction_numb",
                table: "retail_transaction_staging",
                columns: new[] { "organization_id", "transaction_number" });

            migrationBuilder.CreateIndex(
                name: "ix_retail_transaction_staging_lines_retail_transaction_staging",
                table: "retail_transaction_staging_lines",
                columns: new[] { "retail_transaction_staging_id", "line_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_retail_transaction_staging_tenders_retail_transaction_stagi",
                table: "retail_transaction_staging_tenders",
                columns: new[] { "retail_transaction_staging_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id_module_action",
                table: "role_permissions",
                columns: new[] { "role_id", "module", "action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roles_organization_id_name",
                table: "roles",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_lines_product_variant_id",
                table: "sales_order_lines",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_lines_sales_order_id",
                table: "sales_order_lines",
                column: "sales_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_customer_id",
                table: "sales_orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_organization_id_customer_id_order_date_created",
                table: "sales_orders",
                columns: new[] { "organization_id", "customer_id", "order_date", "created_at", "id" },
                descending: new[] { false, false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_organization_id_order_date_created_at_id",
                table: "sales_orders",
                columns: new[] { "organization_id", "order_date", "created_at", "id" },
                descending: new[] { false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_organization_id_order_number",
                table: "sales_orders",
                columns: new[] { "organization_id", "order_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_organization_id_status_order_date_created_at_id",
                table: "sales_orders",
                columns: new[] { "organization_id", "status", "order_date", "created_at", "id" },
                descending: new[] { false, false, true, true, true });

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotation_lines_quotation_id",
                table: "sales_quotation_lines",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotations_customer_id",
                table: "sales_quotations",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotations_organization_id_quotation_number",
                table: "sales_quotations",
                columns: new[] { "organization_id", "quotation_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_order_lines_from_location_id",
                table: "transfer_order_lines",
                column: "from_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_order_lines_to_location_id",
                table: "transfer_order_lines",
                column: "to_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_order_lines_transfer_order_id",
                table: "transfer_order_lines",
                column: "transfer_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_orders_from_warehouse_id",
                table: "transfer_orders",
                column: "from_warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_orders_organization_id_order_number",
                table: "transfer_orders",
                columns: new[] { "organization_id", "order_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_orders_to_warehouse_id",
                table: "transfer_orders",
                column: "to_warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_variant_attribute_definitions_organization_id_code",
                table: "variant_attribute_definitions",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_variant_attribute_values_variant_attribute_definition_id_at",
                table: "variant_attribute_values",
                columns: new[] { "variant_attribute_definition_id", "attribute_type", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vendor_addresses_vendor_id",
                table: "vendor_addresses",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_contacts_vendor_id",
                table: "vendor_contacts",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_credit_notes_ap_invoice_id",
                table: "vendor_credit_notes",
                column: "ap_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_credit_notes_organization_id_credit_note_number",
                table: "vendor_credit_notes",
                columns: new[] { "organization_id", "credit_note_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vendor_credit_notes_vendor_id",
                table: "vendor_credit_notes",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendors_organization_id_vendor_number",
                table: "vendors",
                columns: new[] { "organization_id", "vendor_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_inventory_balances_organization_id_product_varian",
                table: "warehouse_inventory_balances",
                columns: new[] { "organization_id", "product_variant_id", "warehouse_id", "warehouse_location_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_inventory_balances_product_variant_id",
                table: "warehouse_inventory_balances",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_inventory_balances_warehouse_id",
                table: "warehouse_inventory_balances",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_inventory_balances_warehouse_location_id",
                table: "warehouse_inventory_balances",
                column: "warehouse_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_locations_warehouse_id_code",
                table: "warehouse_locations",
                columns: new[] { "warehouse_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_types_organization_id_name",
                table: "warehouse_types",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_warehouses_organization_id_code",
                table: "warehouses",
                columns: new[] { "organization_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_warehouses_site_id",
                table: "warehouses",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouses_warehouse_type_id",
                table: "warehouses",
                column: "warehouse_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_approval_steps_workflow_instance_id",
                table: "workflow_approval_steps",
                column: "workflow_instance_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_organization_id_document_type_document_id",
                table: "workflow_instances",
                columns: new[] { "organization_id", "document_type", "document_id" });

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_organization_id_status",
                table: "workflow_instances",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_workflow_instances_template_id",
                table: "workflow_instances",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_template_steps_workflow_template_id",
                table: "workflow_template_steps",
                column: "workflow_template_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts_payable_parameters");

            migrationBuilder.DropTable(
                name: "accounts_receivable_parameters");

            migrationBuilder.DropTable(
                name: "accrual_posting_lines");

            migrationBuilder.DropTable(
                name: "accrual_scheme_allocations");

            migrationBuilder.DropTable(
                name: "ap_invoice_lines");

            migrationBuilder.DropTable(
                name: "ar_payments");

            migrationBuilder.DropTable(
                name: "asset_depreciations");

            migrationBuilder.DropTable(
                name: "asset_disposals");

            migrationBuilder.DropTable(
                name: "asset_maintenances");

            migrationBuilder.DropTable(
                name: "asset_transfers");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "bank_reconciliations");

            migrationBuilder.DropTable(
                name: "bank_transactions");

            migrationBuilder.DropTable(
                name: "batch_job_configs");

            migrationBuilder.DropTable(
                name: "campaigns");

            migrationBuilder.DropTable(
                name: "cash_journal_lines");

            migrationBuilder.DropTable(
                name: "charge_codes");

            migrationBuilder.DropTable(
                name: "coupon_redemptions");

            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "customer_addresses");

            migrationBuilder.DropTable(
                name: "customer_contacts");

            migrationBuilder.DropTable(
                name: "customer_credit_notes");

            migrationBuilder.DropTable(
                name: "customer_loyalty_accounts");

            migrationBuilder.DropTable(
                name: "document_chunks");

            migrationBuilder.DropTable(
                name: "dunning_records");

            migrationBuilder.DropTable(
                name: "expense_categories");

            migrationBuilder.DropTable(
                name: "expense_lines");

            migrationBuilder.DropTable(
                name: "export_job_rows");

            migrationBuilder.DropTable(
                name: "financial_dimension_set_members");

            migrationBuilder.DropTable(
                name: "general_journal_voucher_templates");

            migrationBuilder.DropTable(
                name: "general_ledger_parameters");

            migrationBuilder.DropTable(
                name: "import_job_rows");

            migrationBuilder.DropTable(
                name: "inbound_order_lines");

            migrationBuilder.DropTable(
                name: "inventory_records");

            migrationBuilder.DropTable(
                name: "inventory_transactions");

            migrationBuilder.DropTable(
                name: "journal_line_dimension_values");

            migrationBuilder.DropTable(
                name: "methods_of_payment");

            migrationBuilder.DropTable(
                name: "number_sequences");

            migrationBuilder.DropTable(
                name: "outbound_order_lines");

            migrationBuilder.DropTable(
                name: "payment_proposal_lines");

            migrationBuilder.DropTable(
                name: "pos_payments");

            migrationBuilder.DropTable(
                name: "pos_transaction_lines");

            migrationBuilder.DropTable(
                name: "price_agreements");

            migrationBuilder.DropTable(
                name: "purchase_order_receipt_lines");

            migrationBuilder.DropTable(
                name: "purchase_requisition_lines");

            migrationBuilder.DropTable(
                name: "retail_statements");

            migrationBuilder.DropTable(
                name: "retail_stores");

            migrationBuilder.DropTable(
                name: "retail_tender_settlements");

            migrationBuilder.DropTable(
                name: "retail_transaction_staging_lines");

            migrationBuilder.DropTable(
                name: "retail_transaction_staging_tenders");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "sales_order_lines");

            migrationBuilder.DropTable(
                name: "sales_quotation_lines");

            migrationBuilder.DropTable(
                name: "transfer_order_lines");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "variant_attribute_values");

            migrationBuilder.DropTable(
                name: "vendor_addresses");

            migrationBuilder.DropTable(
                name: "vendor_contacts");

            migrationBuilder.DropTable(
                name: "vendor_credit_notes");

            migrationBuilder.DropTable(
                name: "warehouse_inventory_balances");

            migrationBuilder.DropTable(
                name: "workflow_approval_steps");

            migrationBuilder.DropTable(
                name: "workflow_template_steps");

            migrationBuilder.DropTable(
                name: "accrual_posting_runs");

            migrationBuilder.DropTable(
                name: "fixed_assets");

            migrationBuilder.DropTable(
                name: "cash_journals");

            migrationBuilder.DropTable(
                name: "promotions");

            migrationBuilder.DropTable(
                name: "loyalty_programs");

            migrationBuilder.DropTable(
                name: "ar_invoices");

            migrationBuilder.DropTable(
                name: "expense_reports");

            migrationBuilder.DropTable(
                name: "import_jobs");

            migrationBuilder.DropTable(
                name: "inbound_orders");

            migrationBuilder.DropTable(
                name: "financial_dimension_values");

            migrationBuilder.DropTable(
                name: "journal_lines");

            migrationBuilder.DropTable(
                name: "payment_processor_configurations");

            migrationBuilder.DropTable(
                name: "outbound_orders");

            migrationBuilder.DropTable(
                name: "ap_payments");

            migrationBuilder.DropTable(
                name: "payment_proposals");

            migrationBuilder.DropTable(
                name: "pos_transactions");

            migrationBuilder.DropTable(
                name: "purchase_order_lines");

            migrationBuilder.DropTable(
                name: "purchase_order_receipts");

            migrationBuilder.DropTable(
                name: "purchase_requisitions");

            migrationBuilder.DropTable(
                name: "retail_transaction_staging");

            migrationBuilder.DropTable(
                name: "sales_quotations");

            migrationBuilder.DropTable(
                name: "transfer_orders");

            migrationBuilder.DropTable(
                name: "app_users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "workflow_instances");

            migrationBuilder.DropTable(
                name: "accrual_schemes");

            migrationBuilder.DropTable(
                name: "bank_accounts");

            migrationBuilder.DropTable(
                name: "sales_orders");

            migrationBuilder.DropTable(
                name: "financial_dimensions");

            migrationBuilder.DropTable(
                name: "journal_entries");

            migrationBuilder.DropTable(
                name: "ap_invoices");

            migrationBuilder.DropTable(
                name: "product_variants");

            migrationBuilder.DropTable(
                name: "warehouse_locations");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropTable(
                name: "workflow_templates");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "financial_dimension_sets");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "fiscal_periods");

            migrationBuilder.DropTable(
                name: "ledgers");

            migrationBuilder.DropTable(
                name: "purchase_orders");

            migrationBuilder.DropTable(
                name: "catalog_products");

            migrationBuilder.DropTable(
                name: "account_types");

            migrationBuilder.DropTable(
                name: "fiscal_years");

            migrationBuilder.DropTable(
                name: "charts_of_accounts");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.DropTable(
                name: "warehouses");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "variant_attribute_definitions");

            migrationBuilder.DropTable(
                name: "vendors");

            migrationBuilder.DropTable(
                name: "fiscal_calendars");

            migrationBuilder.DropTable(
                name: "operational_sites");

            migrationBuilder.DropTable(
                name: "warehouse_types");

            migrationBuilder.DropSequence(
                name: "variant_number_block_seq");
        }
    }
}
