﻿using System;
using System.Collections.Generic;

namespace ProjectPRN212.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public int TeacherId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual User Teacher { get; set; } = null!;
}
