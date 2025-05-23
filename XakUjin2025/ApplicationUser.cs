﻿using System.ComponentModel.DataAnnotations;

namespace XakUjin2025
{
    public class ApplicationUser
    {
        [Key]
        public int UserId { get; set; }
        public string? Username { get; set; }
        [MaxLength(255)]
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? EmailConfirmationCode { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }
        public List<Apartment> Apartments { get; set; }
    }
}
