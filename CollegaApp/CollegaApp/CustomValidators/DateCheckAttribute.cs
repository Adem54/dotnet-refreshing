using System;
using System.ComponentModel.DataAnnotations;

namespace CollegaApp.CustomValidators
{
    //Custom validation attribute to check date-related conditions inherited from ValidationAttribute base class comes from System.ComponentModel.DataAnnotations namespace.
    public class DateCheckAttribute:ValidationAttribute
    {
        //IsValid method is overridden to implement custom validation logic.
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            //içinde value object olduğu için, gerçek tipini bulup kullanman gerekiyor..bu onemli
            var date = (DateTime?)value;
            if(date < DateTime.Now)
            {
                return new ValidationResult("The date cannot be in the past.");
            }

        
            return new ValidationResult("The date is valid.");
            //return new ValidationResult("The date is valid.");Bu, başarısızlık demek. ValidationResult.Success (ya da null) dönmediğin sürece ModelState’e hata eklenir. [ApiController] varsa otomatik 400 BadRequest gelir.
            //aşarılı durumda ValidationResult.Success döneceksin.


            //if (value is DateTime dateValue)//value is not null and is of type DateTime demektir bu...
            //{
            //    if (dateValue > DateTime.Now)
            //    {
            //        return new ValidationResult("Date cannot be in the future.");
            //    }
            //    return ValidationResult.Success;
            //}
            //return new ValidationResult("Invalid date format.");
        }

        //BU DAHA DOGRU YAKLASIMDIR..
        /*
         Neden hep 400 alıyordun?
        [ApiController] varken, model validasyonu başarısız ise framework otomatik 400 döndürür.
        Sen başarılı durumda bile new ValidationResult("The date is valid.") döndürdüğün için ModelState hatalı oluyor → 400.
        Başarı için ValidationResult.Success (veya null) dönmelisin.
         */
        public class DateCheckAttribute2 : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext context)
            {
                // Boş değer bu attribute’un konusu değilse (zorunluluğu [Required] belirler)
                if (value is null)
                    return ValidationResult.Success;

                if (value is DateTime dt)
                {
                    // Sadece gün bazında karşılaştırma (saatleri eliyoruz)
                    if (dt.Date < DateTime.Today)
                        return new ValidationResult("The date cannot be in the past.");

                    return ValidationResult.Success; // ✅ başarı
                }

                return new ValidationResult("Invalid date format.");
            }
        }

    }
}
