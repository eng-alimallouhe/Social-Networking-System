using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Application.DTOs.Education.Responses;

public class FacultyLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
