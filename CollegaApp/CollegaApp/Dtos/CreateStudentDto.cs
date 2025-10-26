using CollegaApp.CustomValidators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace CollegaApp.Dtos
{
    //Burda kullanilan attributeler Built-in Data Annotations attributeleridir.Cok daaha fazlasi vardir, ornegin CreditCard,Phone,Url,RegularExpression vs.And RemoteValidation attribute is also available for remote validation scenarios.Güvenlik duvarı değildir. [Remote] sadece kullanıcı deneyimi içindir. Form post edildiğinde sunucu tarafı doğrulamayı yine yapmalısın (aynı kuralı server’da da kontrol et).MVC de gecerlidir...WEBAPI DE KULLANILMAZ...Data Annotations attributeleri model property'lerine eklenerek kullanilirlar.Web API (JSON-only) senaryosunda [Remote] kullanılmaz. SPA (React/Angular/Vue) tarafında kendi AJAX doğrulamanı yaparsın; sunucuda normal endpoint ile kontrol edersin.
    //Ne işe yarar?Formda kullanıcı bir alanı(örn.Email, UserName) yazarken, tarayıcı arka planda belirttiğin action’a istek atar.Action true/false (veya hata mesajı) döner; alan anında geçerli/geçersiz görünür.
    //Yani login isleminde email daha onceden sistemde kayitli mi diye hemen kontrol edebiliriz...Ama Web API de kullanmayiz...Cunku Web API de genelde client-server ayrimi vardir ve client tarafinda bu tur kontroller yapilir...Server tarafinda ise normal validation kurallari kullanilir...
    //Bunlar model property'lerinin uzerine eklenerek validation kurallarini belirlerler.
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "Name is required")]
        //Required attribute validates that the property must have a value. If the property is null or an empty string, validation will fail and the specified error message will be returned.
        //Ve ErrorMessage parametresi ile de kullaniciya gosterilecek hata mesajini belirtiyoruz.
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]//StringLength attribute validates that the length of the string property does not exceed the specified maximum length. If the length exceeds the limit, validation will fail and the specified error message will be returned.
        public string Name { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Invalid email format")]
        //EmailAddress attribute validates that the property contains a valid email address format. If the value does not match the expected email format, validation will fail and the specified error message will be returned.

        public string Email { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Address is required")]
     //   [ValidateNever]//This property will be excluded from validation.If we don't want to validate this field..we can use ValidateNever attribute.
        //ValidateNever attribute indicates that the property should not be validated during model validation. This is useful when you want to exclude certain properties from validation checks.

      //  public string Password { get; set; } = string.Empty;
       // [Compare("Password", ErrorMessage = "Confirmation password does not match the password")]//Compare attribute validates that the value of the property matches the value of another specified property. In this case, it ensures that ConfirmationPassword matches Password. If they do not match, validation will fail and the specified error message will be returned.
    //    [Compare(nameof(Password), ErrorMessage = "Confirmation email does not match the email")] //Using nameof operator to refer to the Password property. This approach is safer because it avoids hardcoding the property name as a string, reducing the risk of errors during refactoring. As we use nameof, if we rename the Password property, the compiler will automatically update the reference in the Compare attribute.For this reason, this usage is right way and safer and preferred.
       // public string ConfirmationPassword { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        //      [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]//Range attribute validates that the numeric property falls within the specified range. If the value is outside the defined range, validation will fail and the specified error message will be returned.
        //        public int Age { get; set; }

        [DateCheck]
        public DateTime AdmissionDate {  get; set; }
    }
}