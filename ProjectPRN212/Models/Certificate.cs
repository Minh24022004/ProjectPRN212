﻿using System;
using System.Collections.Generic;

namespace ProjectPRN212.Models;

public partial class Certificate
{
    public int CertificateId { get; set; }

    public int UserId { get; set; }

    public DateOnly IssuedDate { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public string? CertificateCode { get; set; }
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
