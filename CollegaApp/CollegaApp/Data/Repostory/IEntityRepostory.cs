using System.Linq.Expressions;

namespace CollegaApp.Data.Repostory
{
    //Isimlendirme konusunda cok iyi olmaliyiz ve tecrube kazanmaliyz...
    //Ornegin burda IEntityRepostory iyidir ama, bu uygulama geneli olacak bir interface degilmi uygulamamiz da ne uzerine Collega, o zaman ICollegaRepostory yde olabilir...ok..
    //Generate types kullanacagiz her turlu yeni tip icin kullanabilelm..
    public interface IEntityRepostory<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetAsync(Expression<Func<T,bool>> filter, bool isNoTracking = false);
        //Task<T?> GetByNameAsync(Expression<Func<T, bool>> filter);
        //Task<T?> GetByEmailAsync(Expression<Func<T, bool>> filter);
        Task<T> CreateAsync(T entity);//entity yrine, dbRecord,college...vs olablirdi..
        Task<T> UpdateAsync(T entity);//
        Task<bool> DeleteAsync(T entityToDelete);
        //Biz bu int return etmek yerine hepsini daha common ve thorogout-along the application..yapabilmeki icin artik int imzalarda T common type i return edecegiz, bool return type ini birakabilirz....
    }
}
