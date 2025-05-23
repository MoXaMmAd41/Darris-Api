﻿using System.ComponentModel.DataAnnotations;

namespace Darris_Api.Models.Dto
{
    public class CourseCreateDto
    {
        [Required]
        public string CourseName { get; set; }
        public string? StudentAdvice { get; set; }
        public List<int> MajorIds { get; set; }
    }
}
