using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
        public string PasswordHash { get; set; }
        public int? GroupId { get; set; }

        [Display(Name = "Группа")]
        public virtual Group Group { get; set; }
        [Display(Name = "Роли")]
        public virtual ICollection<UserRole> UserRoles { get; set; }

        [NotMapped]
        [Display(Name = "ФИО")]
        public string FullName
        {
            get
            {
                string fullName = LastName ?? string.Empty;
                if (FirstName != null)
                {
                    if (fullName.Length > 0)
                        fullName += " ";
                    fullName += FirstName;
                }
                if (MiddleName != null)
                {
                    if (fullName.Length > 0)
                        fullName += " ";
                    fullName += MiddleName;
                }
                return fullName;
            }
        }

        [NotMapped]
        public string ShortName
        {
            get
            {
                string shortName = LastName ?? string.Empty;
                if (!string.IsNullOrEmpty(FirstName))
                {
                    if (shortName.Length > 0)
                        shortName += " ";
                    shortName += FirstName[0] + ".";
                }
                if (!string.IsNullOrEmpty(MiddleName))
                {
                    if (shortName.Length > 0)
                        shortName += " ";
                    shortName += MiddleName[0] + ".";
                }
                return shortName;
            }
        }
    }
}
