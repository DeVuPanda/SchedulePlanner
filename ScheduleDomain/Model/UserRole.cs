using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDomain.Model;

public partial class UserRole
{
    public int Id { get; set; }

    [Display(Name = "User role")]
    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
