using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LB2and3.Models.ViewModels
{ 
    public class StudentVM
    {
        public System.Guid StudentId { get; set; }

        [Required]
        [DisplayName("Фамилия")]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        [DisplayName("Пол")]
        public string Gender { get; set; }

        [Required]
        [DisplayName("Дата рождения")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime BIrthDate { get; set; }
        [DisplayName("Дата и время добавления")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddThh:mm}", ApplyFormatInEditMode = true)]
        public System.DateTime InsertDateTime { get; set; }

        [DisplayName("Время подъёма")]
        public System.TimeSpan WakeUpTime { get; set; }
    }
}
