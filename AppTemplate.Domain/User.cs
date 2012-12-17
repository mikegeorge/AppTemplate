using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace AppTemplate.Domain {
  public class User {
    public User() {
    }

    public int Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required,Email]
    public string Email { get; set; }
    public string PasswordSalt { get; set; }
    public string PasswordHash { get; set; }
    public UserRole UserRole { get; set; }
    public bool Disabled { get; set; }
    public string Comment { get; set; }
    public bool Agreement { get; set; }
    public bool PasswordNeedsUpdating { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateLastLogin { get; set; }
    public DateTime DateLastPasswordChange { get; set; }
  }

}