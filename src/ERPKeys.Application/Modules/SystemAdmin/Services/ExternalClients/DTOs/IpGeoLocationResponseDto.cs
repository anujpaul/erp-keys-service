using System;
using System.Collections.Generic;
using System.Text;

namespace ERPKeys.Application.Modules.SystemAdmin.Services.ExternalClients.DTOs
{
    public record IpGeoLocationResponseDto
    (
        string? Status,
        string? country,
        string? countryCode,
        string? region,
        string? city,
        string? zip

    );
}
