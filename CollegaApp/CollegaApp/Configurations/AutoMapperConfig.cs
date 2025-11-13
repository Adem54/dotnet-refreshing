using AutoMapper;
using CollegaApp.Data;
using CollegaApp.Dtos;

namespace CollegaApp.Configurations
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            //CreateMap<Student, StudentDto>();//GetAll da parametrede studenDto gelir sonra, dbContext.Studens dan veritabaindan tumn STudents cekiliyor ve .Select ile StudentDto olarak return ediliyor yani Student aliniyor=>source, studentDto return ediliyr=> destination
            //Ama StudentDto dan datayi alip da Student olarak donmek istedgimzde de o zaman da asagidaki gibi yapariz..
            //CreateMap<StudentDto, Student>();
            //Ama yukardaki gibi ayri ayri 2 satir yazmak yerine, biz tek satir da da bu islemi yapabiliriz asagidaki gibi
            //CreateMap<StudentDto, Student>().ReverseMap();
            //ForMember-for different name of property
            // CreateMap<StudentDto, Student>().ForMember(n=>n.Name, opt=>opt.MapFrom(x=>x.StudentName)).ReverseMap();
            // CreateMap<StudentDto, Student>().ReverseMap().ForMember(n=>n.StudentName, opt=>opt.MapFrom(x=>x.Name));
            //CreateMap<StudentDto, Student>().ReverseMap().ForMember(n=>n.StudentName, opt=>opt.MapFrom(x=>x.Name));
            //CreateMap<TSource , TDestination>
            //Student=>Name, StudentDto=>StudentName, iken bu sekilde 
            //For StudenDto - StudentName Ignore mapping...egerki tum propertyslerden StudetnName i ignore etmesini istersek..
            //CreateMap<StudentDto, Student>().ReverseMap().ForMember(sdto
            //    =>sdto.StudentName, opt=>opt.Ignore());
            CreateMap<CreateStudentDto, Student>().ReverseMap();
            CreateMap<UpdateStudentDto, Student>().ReverseMap();
            //Artik bundan sonra tum dto-entity mappinglerimizi buraya yerlestirebiliriz...
            // CreateMap<CourseDto, Course>().ReverseMap();

            //CreateMap<StudentDto, Student>().ReverseMap().AddTransform<string>(n=>string.IsNullOrEmpty(n)?"No address found": n);
            //Bu biraz riskli birsey bu tum kolonlardaki null olan alanlar icin calisir ama biz spesifik olarak address alani icin calissin istiyorum mesela...
            CreateMap<StudentDto, Student>().ReverseMap().ForMember(sdto=>sdto.Address, opt=>opt.MapFrom(x=>x.Address)).AddTransform<string>(n=>string.IsNullOrEmpty(n) ? "No address found" : n);
           // CreateMap<StudentDto, Student>().ReverseMap().AddTransform<string>(n =>Convert.To );
        }
    }
}
